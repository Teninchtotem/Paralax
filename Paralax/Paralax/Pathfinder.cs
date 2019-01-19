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

class PathFinder
{
    //Variables
    private double maxVal;
    private int nodesX;
    private int voidZone;
    private int nodesY;
    private int startX;
    private int startY;
    private int nodeWidth;
    private int nodeHeight;
    List<SpaceObject> spaceObjects;
    public SearchNode[,] searchNodes;
    public double[,] tempSearchNodeWeight;
    public int levelWidth;
    public int levelHeight;

    //Paramatised constructor setting variables and initialising searchNodes
    public void Pathfinder(GraphicsDevice map, int voidzoneIn, float percentCoverageDecimal, List<SpaceObject> spaceObjectsIn)
    {
        spaceObjects = spaceObjectsIn;
        voidZone = voidzoneIn;
        startX = 1 - voidZone;
        startY = 1 - voidZone;
        levelWidth = map.Viewport.Width + (voidZone * 2);
        levelHeight = map.Viewport.Height + (voidZone * 2);

        InitializeSearchNodes(map, percentCoverageDecimal);
    }
    //Creating, Initialising and then linking nodes 
    private void InitializeSearchNodes(GraphicsDevice map, double percentCoverageDecimal)
    {
        nodesX = (int)(levelWidth * percentCoverageDecimal);
        nodesY = (int)(levelHeight * percentCoverageDecimal);
        if (nodesX < 3)
            nodesX = 3;
        if (nodesY < 3)
            nodesY = 3;

        nodeWidth = levelWidth / nodesX;
        nodeHeight = levelHeight / nodesY;

        searchNodes = new SearchNode[nodesX, nodesY];
        tempSearchNodeWeight = new double[nodesX, nodesY];

        //For each of the tiles in our map, we
        // will create a search node for it.
        for (int x = 0; x < nodesX; x++)
        {
            for (int y = 0; y < nodesY; y++)
            {
                SearchNode node = new SearchNode();

                node.rec = new Rectangle(((x * nodeWidth) + startX), ((y * nodeHeight) + 1) + startY, nodeWidth, nodeHeight);
                node.weight = 0;
                node.scenarioWeight = 0;
                node.inUse = false;
                node.Neighbors = new SearchNode[8];
                searchNodes[x, y] = node;
            }

        }
        linkNodes();
    }
    //Resets node information for each update loop
    public void resetNodeInfo()
    {
        for (int x = 0; x < nodesX; x++)
        {
            for (int y = 0; y < nodesY; y++)
            {
                searchNodes[x, y].currentPath = false;
            }
        }
        softResetNodeInfo();
    }
    //Resets node information for each path find
    public void softResetNodeInfo()
    {
        for (int x = 0; x < nodesX; x++)
        {
            for (int y = 0; y < nodesY; y++)
            {
                searchNodes[x, y].scenarioWeight = 0;
                searchNodes[x, y].inUse = false;
                searchNodes[x, y].parentNode = null;
                searchNodes[x, y].calculatingBestParent = false;
            }
        }
    }
    //Returns the node at which the passed in Space Object lies
    public SearchNode spaceObjectLoc(SpaceObject SO)
    {
        SearchNode returnVal = searchNodes[0, 0];
        for (int i = 0; i < nodesX; i++)
        {
            for (int j = 0; j < nodesY; j++)
            {
                if (SO != null)
                    if (searchNodes[i, j].rec.Contains((int)SO.getX(), (int)SO.getY()))
                    {
                        returnVal = searchNodes[i, j];
                        //Calculated Escape Plan (muhahahaaaa)
                        i = nodesX;
                        j = nodesY;
                    }
            }
        }
        return returnVal;
    }
    //Updates all the nodes based on weights of space objects
    public virtual void updateNodes()
    {
        resetNodeInfo();
        double asteroidWeightVal = 10;
        double maxDistance = Math.Sqrt(Math.Pow(levelHeight/2, 2) + Math.Pow(levelWidth/2, 2));
        double UnitsPerSpace = (nodeWidth + nodeHeight) / 0.005;

        List <SpaceObject> tempList = spaceObjects.ToList();

        double tempMaxVal = 0;

        foreach (SearchNode SN in searchNodes)
        {
            double tempWeight = 0.1;
            double thisX = (SN.rec.X + (SN.rec.Width / 2));
            double thisY = (SN.rec.Y + (SN.rec.Height / 2));
            foreach (SpaceObject SO in tempList)
            {
                if (SO != null)
                {
                    if (SO.getObjectId() != 6 && SO.getObjectId() != 7)
                    {
                        double unitX = (SO.getCurrentNode().rec.X + (SO.getCurrentNode().rec.Width / 2));
                        double unitY = (SO.getCurrentNode().rec.Y + (SO.getCurrentNode().rec.Height / 2));
                        double xDifference = (thisX - unitX);
                        double yDifference = (thisY - unitY);

                        //Getting minimum distance through the loops
                        if (xDifference < -levelWidth / 2)
                            thisX += levelWidth;
                        else if (xDifference > levelWidth / 2)
                            thisX -= levelWidth;
                        if (yDifference < -levelHeight / 2)
                            thisY += levelHeight;
                        else if (yDifference > levelHeight / 2)
                            thisY -= levelHeight;

                        double distanceToUnit = Math.Sqrt(Math.Pow(thisX - unitX, 2) + Math.Pow(thisY - unitY, 2));
                        distanceToUnit /= UnitsPerSpace;
                        ////Player Ship
                        //if (SO.getObjectId() == 1)
                        //{
                        //    tempWeight += 1-1/(distanceToUnit/10);
                        //}
                        if (SO.getObjectId() == 4)
                        {
                            if (distanceToUnit == 0)
                                distanceToUnit = 0.01;
                            tempWeight += (1 / Math.Pow(distanceToUnit, 2)) * SO.getSize() * asteroidWeightVal;
                        }
                        else if (SO.getObjectId() == 3)
                        {
                            if (distanceToUnit == 0)
                                distanceToUnit = 0.01;
                            tempWeight += (1 / Math.Pow(distanceToUnit, 2)) * 60;
                        }
                        else if (SO.getObjectId() == 2)
                        {
                            if (distanceToUnit < 0.01)
                                distanceToUnit = 0.01;
                            tempWeight += (1 / Math.Pow(distanceToUnit, 2));
                        }
                    }
                }
            }
            if (tempWeight < 0)
                tempWeight = 0;
            SN.weight = tempWeight;
            if (tempWeight > tempMaxVal)
                tempMaxVal = tempWeight;
        }
        maxVal = tempMaxVal;
        //foreach (SearchNode SN in searchNodes)
        //{
        //    SN.weight /= maxVal;
        //}
    }
    //Returns a list of nodes, or a "path" of nodes found by the algorythm from "start" to "dest"
    //Uses "canUseDiagonals" for weather or not it searches diagonals
    public List<SearchNode> findRouteDijkstra(SearchNode start, SearchNode dest, bool canUseDiagonals)
    {		
        //Soft resets node info before a pathfind
        softResetNodeInfo();

        bool abort = false;
        List<SearchNode> finished = new List<SearchNode>();
        List<SearchNode> working = new List<SearchNode>();

        //Setting weather or not it uses diagonal neighbors
        int neighborCount = 4;
        if (canUseDiagonals)
            neighborCount = 8;

        //Starting point has 0 weight
        start.scenarioWeight = 0;
        start.inUse = true;
        start.calculatingBestParent = true;
        working.Add(start);

        SearchNode node = working.ElementAt(0);

        //ALGORYTHM STUUUUFFF
        if (start != dest)
        {
            int counter = 0;
            while (node != dest && abort == false)
            {
                counter++;
                if (working.Count > 0 && abort == false && counter < nodesX*nodesY && counter >= 0)
                {
                    working = working.OrderBy(n => n.scenarioWeight).ToList();
                    node = working.ElementAt(0);

                    if (node != dest)
                    {
                        //Adding neighbors to "working"
                        for (int k = 0; k < neighborCount; k++)
                        {
                            // If we have already processed this node ignore it.
                            if (node.Neighbors[k].inUse == false)
                            {
                                if (node.Neighbors[k].calculatingBestParent)
                                {
                                    if (node.Neighbors[k].scenarioWeight > node.Neighbors[k].weight + node.scenarioWeight)
                                    {
                                        node.Neighbors[k].parentNode = node;
                                        node.Neighbors[k].scenarioWeight = node.Neighbors[k].weight + node.scenarioWeight;
                                    }
                                }
                                else
                                {
                                    node.Neighbors[k].calculatingBestParent = true;
                                    node.Neighbors[k].parentNode = node;
                                    node.Neighbors[k].scenarioWeight = node.Neighbors[k].weight + node.scenarioWeight;
                                    working.Add(node.Neighbors[k]);
                                }
                            }
                        }
                        node.inUse = true;
                        working.RemoveAt(0);
                        finished.Add(node);
                    }
                }
                else
                    abort = true;
            }
        }
        List<SearchNode> returnVal = null;
        if (abort == false)
        {
            dest.setUsageTrue();

            //Outputting path as a list of nodes
            returnVal = new List<SearchNode>();
            SearchNode tempSearchNode = dest;

            while (tempSearchNode.parentNode != null)
            {
                returnVal.Add(tempSearchNode);
                tempSearchNode = tempSearchNode.parentNode;
            }
            returnVal.Add(tempSearchNode);
        }
        return returnVal;
    }
    //Returns the neaest node of lowest value to the input node (default left)
    public List<SearchNode> findSafestNearestNode(SearchNode start)
    {
        List<SearchNode> returnVal = new List<SearchNode>();
        //Starting lowest as [0] Neighbor (left)
        SearchNode lowestNeighbor = start.Neighbors[0];
        for (int k = 1; k < 8; k++)
        {
            //Checking to see if there is a new lowest neighbor
            if (lowestNeighbor.weight > start.Neighbors[k].weight) 
                lowestNeighbor = start.Neighbors[k];
        }
        returnVal.Add(lowestNeighbor);
        return returnVal;
    }
    //Links neighboring nodes together
    private void linkNodes()
    {
        // Now for each of the search nodes, we will
        // connect it to each of its neighbours
        for (int x = 0; x < nodesX; x++)
        {

            for (int y = 0; y < nodesY; y++)
            {
                // An array of all of the possible neighbors this 
                // node could have (including diagonals)

                for (int i = 0; i < 8; i++)
                {
                    int tempX = x;
                    int tempY = y;

                    switch (i)
                    {
                        //first 4 are not diagonals (this allows for easy toggling between sides and diagonals)
                        case 0: tempX -= 1; break;  //Left
                        case 1: tempX += 1; break;  //Right
                        case 2: tempY += 1; break;  //Up
                        case 3: tempY -= 1; break;  //Down

                        case 4: tempY -= 1; tempX -= 1; break;  //Left-Down
                        case 5: tempY -= 1; tempX += 1; break;  //Right-Down
                        case 6: tempY += 1; tempX -= 1; break;  //Up-Down
                        case 7: tempY += 1; tempX += 1; break;  //Up-Down
                    }

                    //Looping map allows for 2 edge nodes to be linked on other sides
                    //Looping X
                    if (tempX < 0)
                    {
                        tempX = nodesX - 1;
                    }
                    else if (tempX > nodesX - 1)
                    {
                        tempX = 0;
                    }
                    //Looping Y
                    if (tempY < 0)
                    {
                        tempY = nodesY - 1;
                    }
                    else if (tempY > nodesY - 1)
                    {
                        tempY = 0;
                    }

                    // Store a reference to the neighbor.
                    searchNodes[x, y].Neighbors[i] = searchNodes[tempX, tempY];
                }

            }

        }
    }

    //Setters and Getters
    public double getMaxVal()
    {
        return maxVal;
    }
}
