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

public class SearchNode
{
    //Variables
    public Rectangle rec;
    public SearchNode parentNode;
    public double weight;
    public double scenarioWeight;
    public double distance;
    public bool inUse;
    public bool calculatingBestParent;
    public bool currentPath;
    public SearchNode[] Neighbors;

    //Constructor Setting Default Values
    public SearchNode()
    {
        distance = 0;
        weight = 1;
        parentNode = null;
    }
    //Recursively sets usage to true
    public void setUsageTrue()
    {
        if (parentNode != null)
        {
            parentNode.setUsageTrue();
        }
        currentPath = true;
    }
}
