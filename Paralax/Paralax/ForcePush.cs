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

public class ForcePush : Projectile
{
    //Constructor with default values
    public ForcePush(SpaceObject SO, Texture2D texIn)
    {
        setX(SO.getX());
        setY(SO.getY());
        setDirection(SO.getSpriteDirection());
        setSpriteDirection(SO.getSpriteDirection());
        setSprite(texIn);
        maxLifeTime = 30;
        lifeTime = maxLifeTime;
        velocity = 7;
        objectID = 6;
        mass = (float)0.5;

        calculateXYBasedOnDirVel();
    }
    //Overwritten Step
    public override void step()
    {
        if (lifeTime > 0)
            lifeTime -= 1;
        collissionDelay -= 1;

        if (lifeTime < maxLifeTime / 4)
            alphaVal = (float)((float)lifeTime / (float)((float)maxLifeTime / (float)4));
        else
            alphaVal = 1;

        size += (float)0.05;
        realSpriteHeight = (int)(origeonalRealSpriteHeight * size);
        realSpriteWidth = (int)(origeonalRealSpriteWidth * size);
        remakeRec();

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

        updatePos();
    }
    //Overwritten backstep (removed) Forcepush should move regardless of collisions
    public override void backStep() { }
    //Overwritten readyColission (removed) Forcepush should move regardless of collisions
    public override void readyColission(SpaceObject collider) { }
    //Overwritten setColission (removed) Forcepush should move regardless of collisions
    public override void setColission(){}
};
