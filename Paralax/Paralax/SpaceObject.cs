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


public class SpaceObject
{
    protected enum weapon { Force_Blast };

    //ALL OF THE VARIABLES!!!!
    protected List<SpaceObject> map;
    protected int levelWidth;
    protected int levelHeight;
    protected bool chaseMode;
    protected float targetX;
    protected float targetY;
    protected SearchNode currentNode;
    protected float multiplyForRadians;
    protected Random rand;
    protected float multiplyForDegrees;
    protected float x;
    protected float y;
    protected float spin;
    protected int maxHealth;
    protected int invuntability;
    protected float mass;
    protected float size;
    protected int health;
    protected float turnSpeed;
    protected float massPerSize;
    protected bool thrust;
    protected float thrustSpeed;
    protected int collissionDelay;
    protected float speedX;
    protected bool collissionClose;
    protected float shieldAlpha;
    protected float shieldValMax;
    protected float shieldVal;
    protected bool collided;
    protected bool turningRight;
    protected bool turningLeft;
    protected SpaceObject targetCol;
    protected Rectangle rec;
    protected float alphaVal;
    protected float speedY;
    protected float direction;
    protected float velocity;
    protected float spriteDirection;
    protected int lifeTime;
    protected float difficulty;
    protected int maxLifeTime;
    protected int objectID;
    protected int realSpriteHeight;
    protected int realSpriteWidth;
    protected int origeonalRealSpriteHeight;
    protected int origeonalRealSpriteWidth;
    protected weapon weaponSelected;
    protected List<Texture2D> weaponTextures;
    protected float tempColissionVelocity;
    protected float tempColissionDirection;
    protected float tempColissionSpeedX;
    protected float tempColissionSpeedY;
    protected Texture2D objectSprite;

    //Setting Default Values in the Constructor
	public SpaceObject()
    {
        targetX = 0;
        targetY = 0;

        shieldAlpha = 0;
        shieldVal = 0;
        shieldValMax = 0;

        invuntability = 0;
        chaseMode = false;
        alphaVal = 1;

        lifeTime = -999;
        x = 0;
        y = 0;
        speedX = 0;
        speedY = 0;
        maxHealth = 100;
        health = maxHealth;

        rand = new Random();
        weaponTextures = new List<Texture2D>();

        collissionClose = false;
        collissionDelay = 0;
        collided = false;

        massPerSize = (float)1.5;
        mass = 1;
        size = 1;

        realSpriteHeight = 30;
        realSpriteWidth = 30;
        origeonalRealSpriteHeight = realSpriteHeight;
        origeonalRealSpriteWidth = realSpriteWidth;

        remakeRec();
        
        turnSpeed = 4f;
        turningLeft = false;
        turningRight = false;

        multiplyForRadians = (3.14159f / 180);
        multiplyForDegrees = (180 / 3.14159f);

        thrustSpeed = 0.12f;
        thrust = false;
        direction = 0;
        velocity = 0;
        spriteDirection = 0;
        objectID = 0;
	}
    //Checks to see if the space object can see the player (false by default)
    public virtual bool checkAwareness(SpaceObject player)
    {
        return false;
    }
    //Inititate levelSize information
    public void initiateLevelSize(int widthIn, int HeightIn)
    {
        levelWidth = widthIn;
        levelHeight = HeightIn;
    }
    //Resets the target to "this" (esentially nullifying it)
    public void resetTargetCol()
    {
        targetCol = this;
    }
    //Makes the space object invunerable for the number of steps input
    public void makeInvunerable(int stepsToBeInvunerable)
    {
        invuntability = stepsToBeInvunerable;
    }
    //Prepares variables to be implamented in "set collision"
    public virtual void readyColission(SpaceObject collider)
    {
        if (collider.getObjectId() != 6 && collider.getObjectId() != 7)
        {
            //MAAAAAAAATHS
            //Reference: http://geekswithblogs.net/robp/archive/2008/05/15/adding-collision-detection.aspx
            //To be fair I edidted it a decent amount!
            Vector3 tempObjectOne = new Vector3(x, y, 0);
            Vector3 tempObjectTwo = new Vector3(collider.getX(), collider.getY(), 0);
            Vector3 xV = tempObjectOne - tempObjectTwo;
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
                health -= (int)(tempVel * combinedMass);
            }
        }
        else if (collider.getObjectId() == 7)
        {
            health -= 5;

            float tempSpeedX = x - collider.getX();
            float tempSpeedY = y - collider.getY();
            float tempDiffX = x - collider.getX();
            float tempDiffY = y - collider.getY();

            if (tempDiffX < 0)
                tempDiffX *= -1;
            if (tempDiffY < 0)
                tempDiffY *= -1;
            float tempDirection = (float)(Math.Atan(tempDiffY / tempDiffX)) * multiplyForDegrees;
            if (tempDiffX == 0 && tempDiffY == 0)
                tempDirection = 0;
            else
            {
                if (tempSpeedX < 0 && tempSpeedY >= 0)
                    tempDirection = 180 + tempDirection;
                else if (tempSpeedX < 0 && tempSpeedY < 0)
                    tempDirection = 180 - tempDirection;
                else if (tempSpeedX >= 0 && tempSpeedY >= 0)
                    tempDirection = -tempDirection;

                tempDirection = 360 - tempDirection;
            }


            tempColissionSpeedX = speedX + ((float)Math.Cos(tempDirection * multiplyForRadians) * collider.getVelocity()) / size;
            tempColissionSpeedY = speedY + ((float)Math.Sin(tempDirection * multiplyForRadians) * collider.getVelocity()) / size;
        }
        else
        {
            bool accelX = false;
            bool accelY = false;

            if ((collider.getSpeedX() - speedX) < -2 || (collider.getSpeedX() - speedX) > 2)
                accelX = true;
            if ((collider.getSpeedY() - speedY) < -2 || (collider.getSpeedY() - speedY) > 2)
                accelY = true;


            if(accelX)
                tempColissionSpeedX = speedX + ((collider.getSpeedX() / 6)/(size*2));
            if(accelY)
                tempColissionSpeedY = speedY + ((collider.getSpeedY() / 6)/(size*2));
        }
        
    }
    //This is called when then object is destroyed and prepares the object for what comes next
    public virtual void destroyed()
    {
        //Alows things to spawn in its place
        rec.X += 10000;
        rec.Y += 10000;
    }
    //Varies health based on input
    public void alterHealth(int healthAlterIn)
    {
        health += healthAlterIn;
    }
    //Sets speedX and speedY based on Direction and Velocity
    public void calculateXYBasedOnDirVel()
    {
        speedX = (float)Math.Cos(getDirectionRadians()) * velocity;
        speedY = (float)Math.Sin(getDirectionRadians()) * velocity;
    }
    //Sets valocity and direction based on speedX speedY
    public void calculateVelAndDirBasedOnSpeedXY()
    {
        float tempSpeedX = speedX;
        float tempSpeedY = speedY;
        if (tempSpeedX < 0)
            tempSpeedX *= -1;
        if (tempSpeedY < 0)
            tempSpeedY *= -1;
        direction = (float)(Math.Atan(tempSpeedY / tempSpeedX)) * multiplyForDegrees;
        if (tempSpeedX == 0 && tempSpeedY == 0)
            direction = 0;
        else
        {
            if (speedX < 0 && speedY >= 0)
                direction = 180 + direction;
            else if (speedX < 0 && speedY < 0)
                direction = 180 - direction;
            else if (speedX >= 0 && speedY >= 0)
                direction = -direction;

            direction = 360 - direction;
        }
        velocity = (float)Math.Sqrt((speedX * speedX) + (speedY * speedY));
    }
    //Steps the object back to where it was a turn ago (aprox)
    public virtual void backStep()
    {
        x -= speedX;
        y -= speedY;
        updateRec();
    }
    //Steps forward without updating
    public virtual void babyStep()
    {
        x += speedX;
        y += speedY;
        updateRec();
    }
    //Updates the Space Object based on its variables
    public virtual void step()
    {
        if (lifeTime > 0)
            lifeTime -= 1;

        if (velocity > 10)
            velocity = 10;

        collissionDelay -= 1;
        if (collissionDelay < 0)
            collissionDelay = 0;

        spriteDirection += spin;

        if (spriteDirection < 0)
            spriteDirection += 360;
        else if (spriteDirection > 360)
            spriteDirection -= 360;

        if (direction < 0)
            direction += 360;
        else if (direction > 360)
            direction -= 360;

        calculateXYBasedOnDirVel();

        updatePos();

        //Updates a space object's x, y, direction, velocity and sprite direction variables
    }
    //Updaes the objects x y by its speedX ad speedY
    virtual protected void updatePos()
    {
        if (invuntability > 0)
            invuntability -= 1;

        x += speedX;
        y += speedY;
        updateRec();
    }
    //updates the collision/draw rectangle with the space objects location
    public void updateRec()
    {
        rec.X = (int)(x - realSpriteWidth / 2);
        rec.Y = (int)(y - realSpriteHeight / 2);
    }
    //Changes the x y so that the object draws correctly
    public void prepareRecForDraw()
    {
        rec.X = (int)(x);
        rec.Y = (int)(y);
    }
    //Chages the x y back for everything else
    public void fixRecAfterDraw()
    {
        updateRec();
    }
    //Fires the selected weapon
    public virtual void fireWeapon(List<SpaceObject> SO)
    {

    }
    //Remakes the rectangle based on x,y,width,height parameters
    public void remakeRec()
    {
        rec = new Rectangle((int)x, (int)y, realSpriteWidth, realSpriteHeight);
    }



    //Getters and setters
    public float getMass()
    {
        return mass;
    }
    public void setMass(float massIn)
    {
        mass = massIn;
    }
    public int getInvunrability()
    {
        return invuntability;
    }
    public int getLifeTime()
    {
        return lifeTime;
    }
    public void setLifeTime(int lifeTimeIn)
    {
        lifeTime = lifeTimeIn;
    }
    public int getMaxLifeTime()
    {
        return maxLifeTime;
    }
    public float getAlpha()
    {
        return alphaVal;
    }
    public void setMaxLifeTime(int maxLifeTimeIn)
    {
        maxLifeTime = maxLifeTimeIn;
        lifeTime = maxLifeTime;
    }
    public float getHealthPercent()
    {
        return (float)health / maxHealth;
    }
    public int getCollisionDelay()
    {
        return collissionDelay;
    }
    public virtual void setColission()
    {
        speedX = tempColissionSpeedX;
        speedY = tempColissionSpeedY;
        calculateVelAndDirBasedOnSpeedXY();
    }
    public bool getCollisionClose()
    {
        return collissionClose;
    }
    public void setCollisionClose(bool collisionCloseIn)
    {
        collissionClose = collisionCloseIn;
    }
    public bool getCollided()
    {
        return collided;
    }
    public virtual void setCollided(bool collisionIn)
    {
        if (collisionIn)
        {
            shieldAlpha = 1;
            collissionDelay = 10;
        }
        collided = collisionIn;
    }
    public virtual int getHealth()
    {
        return health;
    }
    public virtual void setHealth(int healthIn)
    {
        health = healthIn;
    }
    public int getObjectId()
    {
        return objectID;
    }
    public float getX()
    {
        return x;
    }
    public float getY()
    {
        return y;
    }
    public void setThrust(bool thrustIn)
    {
        thrust = thrustIn;
    }
    public void setTurningRight(bool turningRightIn)
    {
        turningRight = turningRightIn;
    }
    public void setTurningLeft(bool turningLeftIn)
    {
        turningLeft = turningLeftIn;
    }
    public void setX(float xIn)
    {
        x = xIn;
        updateRec();
    }
    public void setY(float yIn)
    {
        y = yIn;
        updateRec();
    }
    public float getDirection()
    {
        return direction;
    }
    public float getDirectionRadians()
    {
        return direction * multiplyForRadians;
    }
    public void setDirection(float directionIn)
    {
        direction = directionIn;
    }
    public void setSpin(float spinIn)
    {
        spin = spinIn;
    }
    public float getSpin()
    {
        return spin;
    }
    public float getVelocity()
    {
        return velocity;
    }
    public virtual void setVelocity(float velocityIn)
    {
        velocity = velocityIn;
    }
    public float getSpriteDirection()
    {
        return spriteDirection;
    }
    public float getSpriteDirectionRadians()
    {
        return spriteDirection * multiplyForRadians;
    }
    public void setSpriteDirection(float spriteDirectionIn)
    {
        spriteDirection = spriteDirectionIn;
    }
    public void setSpeedX(float speedXIn)
    {
        speedX = speedXIn;
    }
    public void setSpeedY(float speedYIn)
    {
        speedY = speedYIn;
    }
    public Rectangle getRec()
    {
        return rec;
    }
    public float getSize()
    {
        return size;
    }
    public virtual void setSize(float sizeIn)
    {
        size = sizeIn;
        realSpriteHeight = (int)(origeonalRealSpriteHeight * size);
        realSpriteWidth = (int)(origeonalRealSpriteWidth * size);
        mass = (float)Math.Pow(size, massPerSize);
        maxHealth = (int)(50 * (size * (size * 0.5)));
        health = maxHealth;

        remakeRec();
    }
    public Texture2D getSprite()
    {
        return objectSprite;
    }
    public void setSprite(Texture2D spriteIn)
    {
        objectSprite = spriteIn;
    }
    public int getSpriteHeight()
    {
        return objectSprite.Height;
    }
    public int getSpriteWidth()
    {
        return objectSprite.Width;
    }
    public int getRealSpriteHeight()
    {
        return realSpriteHeight;
    }
    public int getRealSpriteWidth()
    {
        return realSpriteWidth;
    }
    public float getSpeedX()
    {
        return speedX;
    }
    public float getSpeedY()
    {
        return speedY;
    }
    public float getMaxHealth()
    {
        return maxHealth;
    }
    public float getMaxShieldVal()
    {
        return shieldValMax;
    }
    public SearchNode getCurrentNode()
    {
        return currentNode;
    }
    public virtual void setPath(List<SearchNode> pathIn)
    {

    }
    public float getTargetX()
    {
        return targetX;
    }
    public float getTargetY()
    {
        return targetY;
    }
    public bool getChaseMode()
    {
        return chaseMode;
    }
    public void setChaseMode(bool modeIn)
    {
        chaseMode = modeIn;
    }
    public virtual List<SearchNode> getPath()
    {
        return null;
    }
    public void setCurrentNode(SearchNode nodeIn)
    {
        currentNode = nodeIn;
    }
    public float getShieldAlpha()
    {
        return shieldAlpha;
    }
    public float getShieldvalPercent()
    {
        return shieldVal / shieldValMax;
    }
    public void setShieldVal(int shieldValIn)
    {
        if (shieldValIn > shieldValMax)
            shieldValMax = shieldValIn;
        shieldVal = shieldValIn;
    }
    public bool getThrust()
    {
        return thrust;
    }
    public void init(List<SpaceObject> mapIn, float difficultyIn)
    {
        map = mapIn;
        difficulty = difficultyIn;
    }
    public float getShieldVal()
    {
        return shieldVal;
    }
    public void setTargetCol(SpaceObject SO)
    {
        targetCol = SO;
    }
    public SpaceObject getTargetCol()
    {
        return targetCol;
    }
}
