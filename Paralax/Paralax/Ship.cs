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


public class Ship : SpaceObject
{
    //Ship Variables
    protected bool rechargingShields;
    protected int shieldRecoveryTime;
    protected int shieldRecoveryTimeCounter;
    protected float shieldRecoveryRate;

    //Constructor Setting Default Values
	public Ship()
    {
        rechargingShields = false;
        shieldRecoveryTime = 150;
        shieldRecoveryTimeCounter = 0;
        shieldRecoveryRate = (float)0.2;

        objectID = 1;
	}
    //Adds Weapon Texture
    public void addWeaponTexture(Texture2D texIn)
    {
        weaponTextures.Add(texIn);
    }
    //Updated fire weapon
    public override void fireWeapon(List<SpaceObject> SO)
    {
        switch ((int)weaponSelected)
        {
            case 0:
                SO.Add(new ForcePush(this, weaponTextures.ElementAt((int)weaponSelected)));
                break;
        }

        while (SO.Last().getRec().Intersects(rec))
        {
            SO.Last().babyStep();
        }
    }
    //Updated step
    public override void step()
    {
        if (rechargingShields == false)
        {
            shieldRecoveryTimeCounter++;
            if (shieldRecoveryTimeCounter >= shieldRecoveryTime)
            {
                shieldRecoveryTimeCounter = 0;
                rechargingShields = true;
            }
        }
        else
        {
            if (shieldVal < shieldValMax)
                shieldVal += shieldRecoveryRate;
        }

        if (shieldVal <= 0)
            shieldAlpha = 0;
        else if (shieldAlpha > 0)
            shieldAlpha -= (float)0.04;

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
    //Updated ready collision
    public override void readyColission(SpaceObject collider)
    {
        if (collider.getObjectId() != 6 && collider.getObjectId() != 7)
        {
            //MAAAAAAAATHS
            //Reference: http://geekswithblogs.net/robp/archive/2008/05/15/adding-collision-detection.aspx
            //To be fair I edidted it a decent amount!
            Vector3 tempBallOne = new Vector3(x, y, 0);
            Vector3 tempBallTwo = new Vector3(collider.getX(), collider.getY(), 0);
            Vector3 xV = tempBallOne - tempBallTwo;
            xV.Normalize();
            Vector3 v1 = new Vector3(speedX, speedY, 0);
            float x1 = Vector3.Dot(xV, v1);
            Vector3 v1x = xV * x1;
            Vector3 v1y = v1 - v1x;
            float m1 = mass;
            xV = -xV;
            Vector3 v2 = new Vector3(collider.getSpeedX(), collider.getSpeedY(), 0);
            float x2 = Vector3.Dot(xV, v2);
            Vector3 v2x = xV * x2;
            Vector3 v2y = v2 - v2x;
            float m2 = collider.getMass();
            float combinedMass = m1 + m2;
            Vector3 newVelA = (v1x * ((m1 - m2) / combinedMass)) + (v2x * ((2f * m2) / combinedMass)) + v1y;
            tempColissionSpeedX = newVelA.X;
            tempColissionSpeedY = newVelA.Y;

            float tempVel = (float)Math.Sqrt(Math.Pow(speedX - collider.getSpeedX(), 2) + Math.Pow(speedY - collider.getSpeedY(), 2));

            if (invuntability <= 0)
            {
                invuntability = 2;

                if (objectID == 1)
                    shieldVal -= (float)((tempVel * combinedMass) * difficulty);
                else
                    shieldVal -= (float)((tempVel * combinedMass) / difficulty);

                if (shieldVal < 0)
                {
                    health += (int)shieldVal;
                    shieldVal = 0;
                }
            }
        }
        else if (collider.getObjectId() == 7)
        {
            if (objectID == 1)
                shieldVal -= (float)(5 * difficulty);
            else
                shieldVal -= (float)(5 / difficulty);

            if (shieldVal < 0)
            {
                health += (int)shieldVal;
                shieldVal = 0;
            }

            float tempDiffX = x - collider.getX();
            float tempDiffY = y - collider.getY();
            float tempStoreDiffX = tempDiffX;
            float tempStoreDiffY = -tempDiffY;

            if (tempDiffX < 0)
                tempDiffX *= -1;
            if (tempDiffY < 0)
                tempDiffY *= -1;
            float tempDirection = (float)(Math.Atan(tempDiffY / tempDiffX)) * multiplyForDegrees;
            if (tempDiffX == 0 && tempDiffY == 0)
                tempDirection = 0;
            else
            {
                if (tempStoreDiffX < 0 && tempStoreDiffY >= 0)
                    tempDirection = 180 + tempDirection;
                else if (tempStoreDiffX < 0 && tempStoreDiffY < 0)
                    tempDirection = 180 - tempDirection;
                else if (tempStoreDiffX >= 0 && tempStoreDiffY >= 0)
                    tempDirection = -tempDirection;
            }

            tempColissionSpeedX = speedX + (float)Math.Cos(tempDirection * multiplyForRadians) * collider.getVelocity();
            tempColissionSpeedY = speedY + (float)Math.Sin(tempDirection * multiplyForRadians) * collider.getVelocity();
        }
        else
        {
            tempColissionSpeedX = speedX;
            tempColissionSpeedY = speedY;
        }
    }

    //Getters and Setters
    public override void setVelocity(float velocityIn)
    {
        velocity = velocityIn;

        speedX = (float)Math.Cos(getSpriteDirectionRadians()) * velocity;
        speedY = (float)Math.Sin(getSpriteDirectionRadians()) * velocity;
    }
    public override void setCollided(bool collisionIn)
    {
        if (collisionIn)
        {
            if (shieldVal > 0)
            {
                shieldAlpha = 1;
            }
            collissionDelay = 10;
            rechargingShields = false;
        }
        collided = collisionIn;
    }
}
