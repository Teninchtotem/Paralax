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

public class Chaser : EnemyShip
{
    //Constructor with default values
    public Chaser()
    {
        path = null;
        turnPush = (float)0.01;
        turnSpeed = 5;
        scoreVal = 200;
        objectID = 2;
    }
    //Overwritten Step
    public override void step()
    {
        //Clipping velocity
        if (velocity > 2.5)
        {
            velocity = (float)2.5;
            calculateXYBasedOnDirVel();
        }

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
    //Chaser ship AI
    public override void AI()
    {
        //The chaser ship moves almost the same as the player moves their ship
        //The chaser ship turns and thrusts
        //The AI controlls how, when why and where
        if (path != null)
        {
            if (path.Count > 0)
            {
                if (path.Count > 1)
                {
                    //Removes current node if the ship is already there
                    if (path.ElementAt(0).rec.Contains((int)x, (int)y))
                        path.RemoveAt(0);
                }

                targetX = path.ElementAt(0).rec.X + (path.ElementAt(0).rec.Width / 2);
                targetY = path.ElementAt(0).rec.Y + (path.ElementAt(0).rec.Height / 2);

                //Averaging a max of 3 nodes ahead (3 is best for averaging diagonals)
                if (path.Count > 2)
                {
                    targetX += path.ElementAt(1).rec.X + (path.ElementAt(1).rec.Width / 2);
                    targetY += path.ElementAt(1).rec.Y + (path.ElementAt(1).rec.Height / 2);
                    targetX += path.ElementAt(2).rec.X + (path.ElementAt(1).rec.Width / 2);
                    targetY += path.ElementAt(2).rec.Y + (path.ElementAt(1).rec.Height / 2);
                    targetX /= 3;
                    targetY /= 3;
                }
                else if (path.Count > 1)
                {
                    targetX += path.ElementAt(1).rec.X + (path.ElementAt(1).rec.Width / 2);
                    targetY += path.ElementAt(1).rec.Y + (path.ElementAt(1).rec.Height / 2);
                    targetX /= 2;
                    targetY /= 2;
                }

                //Allows the ship to see through the looping map
                float tempXDiff = x - targetX;
                //Left
                if (Math.Abs(tempXDiff) > Math.Abs(x - (targetX + (float)levelWidth)))
                {
                    tempXDiff = x - (targetX  + (float)levelWidth);
                }
                //Right
                else if (Math.Abs(tempXDiff) > Math.Abs(x - (targetX - (float)levelWidth)))
                {
                    tempXDiff = x - (targetX - (float)levelWidth);
                }
                //Up
                float tempYDiff = y - targetY;
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
                if (tempXDiff > 0)
                {
                    speedX -= turnPush;
                }
                //Right
                else if (tempXDiff < 0)
                {
                    speedX += turnPush;
                }
                //Up

                if (tempYDiff > 0)
                {
                    speedY -= turnPush;
                }
                //Down
                else if (tempYDiff < 0)
                {
                    speedY += turnPush;
                }

                float tempAngleToPlayer = (float)(Math.Atan2(tempYDiff, tempXDiff) * multiplyForDegrees);
                if (tempAngleToPlayer < 0)
                    tempAngleToPlayer = (180 + (180 + tempAngleToPlayer));
                tempAngleToPlayer += 180;
                if (tempAngleToPlayer > 360)
                    tempAngleToPlayer -= 360;

                float directionDifference = tempAngleToPlayer - spriteDirection;
                if (directionDifference < -180)
                    directionDifference *= -1;
                else if (directionDifference > 180)
                    directionDifference *= -1;
                float turnGive = 5;

                if (directionDifference < -turnGive)
                {
                    turningLeft = true;
                    turningRight = false;
                }
                else if (directionDifference > turnGive)
                {
                    turningLeft = false;
                    turningRight = true;
                }
                else
                {
                    turningRight = false;
                    turningLeft = false;
                }
                if (turningLeft == false && turningRight == false)
                    thrust = true;
                else
                    thrust = false;
            }
        }
    }
}
