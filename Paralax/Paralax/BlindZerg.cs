using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

public class BlindZerg : EnemyShip
{
    bool charging = false;
    bool loading = false;
    int loadingMax = 25;
    int loadingLevel = 0;
    int targetingMax = 100;
    int targeting = 0;

    //Constructor with default values
    public BlindZerg()
    {
        speedX = (float)0.0001;
        speedY = (float)0.0001;
        direction = 0;
        velocity = (float)0.001;
        health = 1;
        chasing = false;
        path = null;
        scoreVal = 200;
        objectID = 3;
    }
    //Overrided AI
    //This enemy will drift until it detects the player
    //At which point it will plot a path, then some time later, charge down it and explode
    public override void AI()
    {
        if(loading)
            loadingLevel++;
        if (loadingLevel >= loadingMax)
        {
            loading = false;
            charging = true;
        }

        if(charging)
        {
            if (path != null)
            {
                turnPush = path.ElementAt(0).rec.Width / 4;
                if (path.Count > 1)
                {
                    //Removes current node if the ship is already there
                    if (path.ElementAt(0).rec.Contains((int)x, (int)y))
                        path.RemoveAt(0);

                    targetX = path.ElementAt(0).rec.X + (path.ElementAt(0).rec.Width / 2);
                    targetY = path.ElementAt(0).rec.Y - (path.ElementAt(0).rec.Height / 2);

                    //Allows the ship to see through the looping map
                    float tempXDiff = x - targetX;
                    //Left
                    if (Math.Abs(tempXDiff) > Math.Abs(x - (targetX + (float)levelWidth)))
                    {
                        tempXDiff = x - (targetX + (float)levelWidth);
                    }
                    //Right
                    else if (Math.Abs(tempXDiff) > Math.Abs(x - (targetX - (float)levelWidth)))
                    {
                        tempXDiff = x - (targetX - (float)levelWidth);
                    }
                    //Up
                    float tempYDiff = y - (targetY + (path.ElementAt(0).rec.Height));
                    if (Math.Abs(tempYDiff) > Math.Abs(y - (targetY + (float)levelHeight)))
                    {
                        tempYDiff = y - (targetY + (float)levelHeight);
                    }
                    //Down
                    else if (Math.Abs(tempYDiff) > Math.Abs(y - (targetY - (float)levelHeight)))
                    {
                        tempYDiff = y - (targetY - (float)levelHeight);
                    }


                    //Correcting Speed Variables
                    //Left
                    if (tempXDiff - turnPush > 0)
                    {
                        x -= turnPush;
                        speedX = -turnPush;
                    }
                    //Right
                    else if (tempXDiff + turnPush < 0)
                    {
                        x += turnPush;
                        speedX = turnPush;
                    }
                    else
                    {
                        speedX = 0;
                    }

                    //Up
                    if (tempYDiff - turnPush > 0)
                    {
                        y -= turnPush;
                        speedY = -turnPush;
                    }
                    //Down
                    else if (tempYDiff + turnPush < 0)
                    {
                        speedY = turnPush;
                        y += turnPush;
                    }
                    else
                    {
                        speedY = 0;
                    }
                    updateRec();
                    float tempAngleToPlayer = (float)(Math.Atan2(tempXDiff, tempYDiff) * multiplyForDegrees);
                    if (tempAngleToPlayer < 0)
                        tempAngleToPlayer = (180 + (180 + tempAngleToPlayer));
                    tempAngleToPlayer += 180;
                    if (tempAngleToPlayer > 360)
                        tempAngleToPlayer -= 360;

                    spriteDirection = tempAngleToPlayer;
                }
                else
                    health = 0;
            }
        }
    }
    //Overrided ready collision (empty as mine always explodes)
    public override void readyColission(SpaceObject collider) { }
    //Overrided set collision (makes mine always explode)
    public override void setColission()
    {
        lifeTime = 0;
        health = 0;
    }
    //Overrided set Path
    public override void setPath(List<SearchNode> pathIn)
    {
        if (pathIn != null)
        {
            if (chaseMode && loading == false && loadingLevel == 0)
            {
                loading = true;
                path = pathIn;
                path.Reverse();
            }
        }
    }
    //Overrided Step
    public override void step()
    {
        AI();

        if (lifeTime > 0)
            lifeTime--;

        collissionDelay -= 1;
        if (collissionDelay < 0)
            collissionDelay = 0;

        if (spriteDirection < 0)
            spriteDirection += 360;
        else if (spriteDirection > 360)
            spriteDirection -= 360;

        if (direction < 0)
            direction += 360;
        else if (direction > 360)
            direction -= 360;

        calculateVelAndDirBasedOnSpeedXY();

        updatePos();
    }
    //Overrided updatePos
    protected override void updatePos()
    {
        if (invuntability > 0)
            invuntability -= 1;
        if (chaseMode == false)
        {
            x += speedX;
            y += speedY;
        }
        updateRec();
    }
}
