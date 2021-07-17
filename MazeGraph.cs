using System;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Collections.Generic;

namespace MazeSolver
{
    class MazeGraph
    {
        private int leftDistance, rightDistance, upDistance, downDistance;
        private int xPosition, yPosition;
        private bool visited;
        private MazeGraph upNeighbor, downNeighbor, leftNeighbor, rightNeighbor;
        private static MazeGraph startNode;
        private static MazeGraph finishNode;
        private static List<MazeGraph> visitedNodes = new List<MazeGraph>();

        public int LeftDistance { get => leftDistance; }
        public int RightDistance { get => rightDistance; }
        public int UpDistance { get => upDistance; }
        public int DownDistance { get => downDistance; }
        public int XPosition { get => xPosition; }
        public int YPosition { get => yPosition; }
        public int HeuristicValue
        {
            get
            {
                if (finishNode == null)
                    return -1;
                return (int) Math.Round(Math.Sqrt(Math.Pow(xPosition - finishNode.xPosition, 2) + Math.Pow(yPosition - finishNode.yPosition, 2)));
            }
        }
        public static MazeGraph StartNode { get => startNode; }
        public static MazeGraph FinishNode { get => finishNode; }
        public bool Visited 
        { 
            get => visited;
            set
            {
                if (value)
                    visitedNodes.Add(this);
                visited = value;
            }
        }
        public MazeGraph UpNeighbor { get => upNeighbor; }
        public MazeGraph RightNeighbor { get => rightNeighbor; }
        public MazeGraph DownNeighbor { get => downNeighbor; }
        public MazeGraph LeftNeighbor { get => leftNeighbor; }

        private MazeGraph()
        {
            yPosition = xPosition = -1;
            //distance between two pixels will always be at least 1 so 0 is used to represent infinity 
            leftDistance = rightDistance = upDistance = downDistance = 0;
            //a graph node is always created with no neighbors
            upNeighbor = downNeighbor = leftNeighbor = rightNeighbor = null;
            visited = false;
        }

        public static void CreateGraph(Bitmap img)
        {
            if (img == null)
                return;
            Reset();
            Size imgSize = img.Size;
            Dictionary<int, Dictionary<int, MazeGraph>> nodesPerLevel = new Dictionary<int, Dictionary<int, MazeGraph>>();
            //creating graph nodes
            for (int i = 0; i < imgSize.Height; i++)
            {
                Dictionary<int, MazeGraph> nodesOnLevel = new Dictionary<int, MazeGraph>();
                for (int j = 0; j < imgSize.Width; j++)
                {
                    if (img.GetPixel(j, i).ToArgb().Equals(Color.White.ToArgb()))
                    {
                        MazeGraph node = null;
                        //the beginning and end of the maze are always on the first and last row
                        if (i == 0 || i == (imgSize.Height - 1))
                        {
                            node = new MazeGraph();
                            if (i == 0)
                                startNode = node;
                            else
                                finishNode = node;
                        }
                        //if a pixel is an intersection or a sharp turn create node
                        else if ((img.GetPixel(j, i - 1).ToArgb().Equals(Color.White.ToArgb()) || img.GetPixel(j, i + 1).ToArgb().Equals(Color.White.ToArgb())) && (img.GetPixel(j + 1, i).ToArgb().Equals(Color.White.ToArgb()) || img.GetPixel(j - 1, i).ToArgb().Equals(Color.White.ToArgb())))
                        {
                            node = new MazeGraph();
                        }
                        //check if a pixel is a dead end
                        else
                        {
                            int tmp = 0;
                            tmp += img.GetPixel(j, i - 1).ToArgb().Equals(Color.Black.ToArgb()) ? 1 : 0;
                            tmp += img.GetPixel(j, i + 1).ToArgb().Equals(Color.Black.ToArgb()) ? 1 : 0;
                            tmp += img.GetPixel(j - 1, i).ToArgb().Equals(Color.Black.ToArgb()) ? 1 : 0;
                            tmp += img.GetPixel(j + 1, i).ToArgb().Equals(Color.Black.ToArgb()) ? 1 : 0;
                            //a pixel is a dead end if 3 of possible 4 directions are walls
                            if (tmp == 3)
                                node = new MazeGraph();
                        }
                        if (node != null)
                        {
                            node.xPosition = j;
                            node.yPosition = i;
                            nodesOnLevel.Add(j, node);
                        }
                    }
                }
                if (nodesOnLevel.Keys.Count != 0)
                    nodesPerLevel.Add(i, nodesOnLevel);
            }
            var listOfLevels = nodesPerLevel.Keys.ToList();
            //connecting all graph nodes
            foreach (var level in nodesPerLevel.Keys)
            {
                var nodesOnLevel = nodesPerLevel[level];
                var listOfKeysOnLevel = nodesOnLevel.Keys.ToList();
                foreach (var column in nodesOnLevel.Keys)
                {
                    var tmpNode = nodesOnLevel[column];
                    //a node is guaranteed to have a neighbor in a direction if the next pixel in that direction is white(a traversable pixel)
                    //check if the current node has a left neighbor and if it does connects them
                    if (img.GetPixel(tmpNode.xPosition - 1, tmpNode.yPosition).ToArgb().Equals(Color.White.ToArgb()))
                    {
                        int leftNeighborKey = listOfKeysOnLevel[listOfKeysOnLevel.IndexOf(column) - 1];
                        tmpNode.leftNeighbor = nodesOnLevel[leftNeighborKey];
                        tmpNode.leftDistance = tmpNode.xPosition - leftNeighborKey;
                    }
                    //check if the current node has a right neighbor and if it does connects them
                    if (img.GetPixel(tmpNode.xPosition + 1, tmpNode.yPosition).ToArgb().Equals(Color.White.ToArgb()))
                    {
                        int rightNeighborKey = listOfKeysOnLevel[listOfKeysOnLevel.IndexOf(column) + 1];
                        tmpNode.rightNeighbor = nodesOnLevel[rightNeighborKey];
                        tmpNode.rightDistance = rightNeighborKey - tmpNode.xPosition;
                    }
                    //check if the current node has a neighbor above it and if it does connects them
                    if (tmpNode.yPosition != 0 && img.GetPixel(tmpNode.xPosition, tmpNode.yPosition - 1).ToArgb().Equals(Color.White.ToArgb()))
                    {
                        int upNeighborIndex = listOfLevels.IndexOf(level) - 1;
                        while (upNeighborIndex >= 0 && !nodesPerLevel[listOfLevels[upNeighborIndex]].ContainsKey(column))
                            upNeighborIndex--;
                        tmpNode.upNeighbor = nodesPerLevel[listOfLevels[upNeighborIndex]][column];
                        tmpNode.upDistance = tmpNode.yPosition - listOfLevels[upNeighborIndex];
                    }
                    //check if the current node has a neighbor below it and if it does connects them
                    if (tmpNode.yPosition < (imgSize.Height - 1) && img.GetPixel(tmpNode.xPosition, tmpNode.yPosition + 1).ToArgb().Equals(Color.White.ToArgb()))
                    {
                        int downNeighborIndex = listOfLevels.IndexOf(level) + 1;
                        while (downNeighborIndex < listOfLevels.Count && !nodesPerLevel[listOfLevels[downNeighborIndex]].ContainsKey(column))
                            downNeighborIndex++;
                        tmpNode.downNeighbor = nodesPerLevel[listOfLevels[downNeighborIndex]][column];
                        tmpNode.downDistance = listOfLevels[downNeighborIndex] - tmpNode.yPosition;
                    }
                }
            }
        }

        //saves the answer from a MazeSolver function as a new image
        public static void SaveSolved(Image img, Stack<MazeGraph> answer, string location)
        {
            if (img == null || answer == null || location.Length == 0)
                return;
            Bitmap saveImg = new Bitmap(img);
            var prev = answer.Pop();
            while (answer.Count != 0)
            {
                var tmp = answer.Pop();
                if (prev.upNeighbor == tmp)
                {
                    for (int i = prev.yPosition; i != tmp.yPosition; i--)
                        saveImg.SetPixel(prev.xPosition, i, Color.Green);
                }
                else if (prev.downNeighbor == tmp)
                {
                    for (int i = prev.yPosition; i != tmp.yPosition; i++)
                        saveImg.SetPixel(prev.xPosition, i, Color.Green);
                }
                else if (prev.leftNeighbor == tmp)
                {
                    for (int i = prev.xPosition; i != tmp.xPosition; i--)
                        saveImg.SetPixel(i, prev.yPosition, Color.Green);
                }
                else
                {
                    for (int i = prev.xPosition; i != tmp.xPosition; i++)
                        saveImg.SetPixel(i, prev.yPosition, Color.Green);
                }
                prev = tmp;
            }
            saveImg.SetPixel(prev.xPosition, prev.yPosition, Color.Green);
            saveImg.Save(location);
            saveImg.Dispose();
        }

        public static void Reset()
        {
            foreach (var tmp in visitedNodes)
                tmp.Visited = false;
            visitedNodes.Clear();
        }
    }
}
