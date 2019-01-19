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

public class Particle
{
    public Texture2D tex;
    public float x;
    public float y;
    public float spin;
    public int lifeTime;
    public int maxLifeTime;
    public float speedX;
    public float speedY;
    public Rectangle rec;
    public float direction;
    public float velocity;
    public float alphaVal;
    public float spriteDirection;
    public int realSpriteHeight;
    public int realSpriteWidth;
    public int origeonalRealSpriteHeight;
    public int origeonalRealSpriteWidth;

    public Particle()
    {
        rec = new Rectangle();
    }
    public void calculateVelAndDirBasedOnSpeedXY()
    {
        float tempSpeedX = speedX;
        float tempSpeedY = speedY;
        if (tempSpeedX < 0)
            tempSpeedX *= -1;
        if (tempSpeedY < 0)
            tempSpeedY *= -1;

        direction = (float)(Math.Atan(tempSpeedY / tempSpeedX)) * (180 / 3.14159f);

        if (speedX < 0 && speedY >= 0)
            direction = 180 + direction;
        else if (speedX < 0 && speedY < 0)
            direction = 180 - direction;
        else if (speedX >= 0 && speedY >= 0)
            direction = -direction;

        direction = 360 - direction;

        velocity = (float)Math.Sqrt((speedX * speedX) + (speedY * speedY));
    }
    public void calculateXYBasedOnDirVel()
    {
        speedX = (float)Math.Cos(direction * (3.14159f / 180)) * velocity;
        speedY = (float)Math.Sin(direction * (3.14159f / 180)) * velocity;
    }
    public virtual void step()
    {
        if (lifeTime > 0)
            lifeTime -= 1;

        if (lifeTime < maxLifeTime / 4)
            alphaVal = (float)((float)lifeTime / (float)((float)maxLifeTime / (float)4));
        else
            alphaVal = 1;

        if (velocity > 10)
            velocity = 10;

        spriteDirection += spin;

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
    public void remakeRec()
    {
        rec = new Rectangle((int)x, (int)y, realSpriteWidth, realSpriteHeight);
    }
    virtual protected void updatePos()
    {
        x += speedX;
        y += speedY;
        updateRec();
    }
    public void updateRec()
    {
        rec.X = (int)(x - realSpriteWidth / 2);
        rec.Y = (int)(y - realSpriteHeight / 2);
    }
    public void prepareRecForDraw()
    {
        rec.X = (int)(x);
        rec.Y = (int)(y);
    }
    public void fixRecAfterDraw()
    {
        updateRec();
    }
}
