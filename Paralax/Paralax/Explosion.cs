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

public class Explosion : Projectile
{
    //Constructor with default Values
    public Explosion(SpaceObject SO, Texture2D texIn, float size)
    {
        objectSprite = texIn;
        setX(SO.getX());
        setY(SO.getY());
        maxLifeTime = (int)(25 * size);
        lifeTime = maxLifeTime;
        velocity = (float)0.15 * size;
        objectID = 7;
        mass = (float)0.5;
    }
    //Overwritten Step
    public override void step()
    {
        if (lifeTime < maxLifeTime / 4)
            alphaVal = (float)((float)lifeTime / (float)((float)maxLifeTime / (float)4));
        else
            alphaVal = 1;

        if (lifeTime > 0)
            lifeTime -= 1;

        size += (float)0.25;
        realSpriteHeight = (int)(origeonalRealSpriteHeight * size);
        realSpriteWidth = (int)(origeonalRealSpriteWidth * size);
        remakeRec();
    }
    //Overwritten backstep (removed) Forcepush should move regardless of collisions
    public override void backStep() { }
    //Overwritten readyColission (removed) Forcepush should move regardless of collisions
    public override void readyColission(SpaceObject collider) { }
    //Overwritten setColission (removed) Forcepush should move regardless of collisions
    public override void setColission() { }
};
