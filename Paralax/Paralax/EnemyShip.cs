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

public class EnemyShip : Ship
{
    //Variables
    protected List<SearchNode> path;
	protected int scoreVal ;
    protected float turnPush;
    protected bool chasing;
    protected float distanceToPlayer;

    //Construction with default values
	public EnemyShip()
    {
        lifeTime = -99;
        path = null;
        turnPush = (float)0.01;
        turnSpeed = 5;
        scoreVal = 200;
        objectID = 2;
	}
    //AI will update everything to do with ship movement
    public virtual void AI()
    {

    }
    //Overwritten check awareness to check to see if the player is in sight
    public override bool checkAwareness(SpaceObject player)
    {
        bool returnVal = false;
        if (chaseMode == false)
        {
            float tempTargetX = player.getRec().X;
            float tempTargetY = player.getRec().Y;

            //Allows the ship to see through the looping map
            float tempXDiff = x - tempTargetX;
            //Left
            if (Math.Abs(tempXDiff) > Math.Abs(x - (tempTargetX + (float)levelWidth)))
            {
                tempXDiff = x - (tempTargetX + (float)levelWidth);
            }
            //Right
            else if (Math.Abs(tempXDiff) > Math.Abs(x - (tempTargetX - (float)levelWidth)))
            {
                tempXDiff = x - (tempTargetX - (float)levelWidth);
            }
            //Up
            float tempYDiff = y - tempTargetY;
            if (Math.Abs(tempYDiff) > Math.Abs(y - (tempTargetY + (float)levelHeight)))
            {
                tempYDiff = y - (tempTargetY + (float)levelHeight);
            }
            //Down
            else if (Math.Abs(tempYDiff) > Math.Abs(y - (tempTargetY - (float)levelHeight)))
            {
                tempYDiff = y - (tempTargetY - (float)levelHeight);
            }
            distanceToPlayer = (float)Math.Sqrt(Math.Pow(tempXDiff, 2) + Math.Pow(tempYDiff, 2));



            // P(D) = C(d)*M(r)/M(u)*R^2
            float probability = difficulty * (float)((player.getMass()) / (mass * Math.Pow(distanceToPlayer, 2)));
            probability *= (float)250;

            float tempProb = probability * 100;
            float tempRand = rand.Next(1, 100);


            if (tempProb > tempRand)
            {
                returnVal = true;
                chaseMode = true;
            }
        }
        return returnVal;
    }
    //Overwritten step
    public override void step()
    {
        AI();

        if (lifeTime > 0)
            lifeTime--;

        collissionDelay -= 1;
        if (collissionDelay < 0)
            collissionDelay = 0;

        if (turningRight)
            spriteDirection += turnSpeed;
        if (turningLeft)
            spriteDirection -= turnSpeed;

        if (spriteDirection < 0)
            spriteDirection += 360;
        else if (spriteDirection > 360)
            spriteDirection -= 360;

        if (direction < 0)
            direction += 360;
        else if (direction > 360)
            direction -= 360;

        if (thrust)
        {
            speedX += (float)Math.Cos(getSpriteDirectionRadians()) * thrustSpeed;
            speedY += (float)Math.Sin(getSpriteDirectionRadians()) * thrustSpeed;
        }

        calculateVelAndDirBasedOnSpeedXY();

        updatePos();
    }

    //Getters and Setters
    public override List<SearchNode> getPath()
    {
        return path;
    }
    public override void setPath(List<SearchNode> pathIn)
    {
        if (pathIn != null)
        {
            path = pathIn;
            path.Reverse();
        }
    }
}
