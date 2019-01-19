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

public class Asteroid : SpaceObject{
    public Asteroid()
    {
        //Standard defaults for an asteroid
        realSpriteHeight = 17;
        realSpriteWidth = 17;
        origeonalRealSpriteHeight = realSpriteHeight;
        origeonalRealSpriteWidth = realSpriteWidth;
        remakeRec();
        objectID = 4;
    }
}
