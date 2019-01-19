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

public class PlayerShip : Ship
{
    //Constructor setting default values
	public PlayerShip()
    {
        realSpriteHeight = 50;
        realSpriteWidth = 50;
        origeonalRealSpriteHeight = realSpriteHeight;
        origeonalRealSpriteWidth = realSpriteWidth;
        remakeRec();

        maxHealth = 200;
        health = maxHealth;
        shieldValMax = 100;
        shieldVal = shieldValMax;
        weaponSelected = 0;
        mass = 5;
        direction = 0;
        velocity = 0;
        objectID = 1;
	}
    //Overrided Step
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
}
