using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace MazeSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Please enter picture location:");
            string file = Console.ReadLine();
            string location = Path.GetDirectoryName(file);
            string fileName = Path.GetFileNameWithoutExtension(file);
            try
            {
                Bitmap img = new Bitmap(file);
                MazeGraph.CreateGraph(img);
                Stack<MazeGraph> answer = MazeSolver.DFS();
                MazeGraph.SaveSolved(img, answer, Path.Combine(location, fileName + "DFS.png"));
                MazeGraph.Reset();
                answer = MazeSolver.BFS();
                MazeGraph.SaveSolved(img, answer, Path.Combine(location, fileName + "BFS.png"));
                MazeGraph.Reset();
                answer = MazeSolver.BranchAndBound();
                MazeGraph.SaveSolved(img, answer, Path.Combine(location, fileName + "BranchAndBound.png"));
                MazeGraph.Reset();
                answer = MazeSolver.BestFirst();
                MazeGraph.SaveSolved(img, answer, Path.Combine(location, fileName + "BestFirst.png"));
                MazeGraph.Reset();
                answer = MazeSolver.AStar();
                MazeGraph.SaveSolved(img, answer, Path.Combine(location, fileName + "A.png"));
                MazeGraph.Reset();
                img.Dispose();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }
    }
}
