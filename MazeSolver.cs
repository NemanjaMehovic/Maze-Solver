using System;
using System.Collections.Generic;
using System.Text;

namespace MazeSolver
{
    class MazeSolver
    {

        private MazeSolver() { }

        //basic dfs implementation using a stack
        public static Stack<MazeGraph> DFS()
        {
            if (MazeGraph.StartNode == null)
                return null;
            Stack<MazeGraph> answer = new Stack<MazeGraph>();
            answer.Push(MazeGraph.StartNode);
            while (answer.Count != 0)
            {
                MazeGraph tmp = answer.Pop();
                tmp.Visited = true;
                if (tmp == MazeGraph.FinishNode)
                {
                    answer.Push(tmp);
                    return answer;
                }
                if (tmp.DownNeighbor != null && !tmp.DownNeighbor.Visited)
                {
                    answer.Push(tmp);
                    answer.Push(tmp.DownNeighbor);
                }
                else if (tmp.RightNeighbor != null && !tmp.RightNeighbor.Visited)
                {
                    answer.Push(tmp);
                    answer.Push(tmp.RightNeighbor);
                }
                else if (tmp.LeftNeighbor != null && !tmp.LeftNeighbor.Visited)
                {
                    answer.Push(tmp);
                    answer.Push(tmp.LeftNeighbor);
                }
                else if (tmp.UpNeighbor != null && !tmp.UpNeighbor.Visited)
                {
                    answer.Push(tmp);
                    answer.Push(tmp.UpNeighbor);
                }
            }
            return null;
        }

        //bfs implementation using a queue to store paths that are represented with a stack
        public static Stack<MazeGraph> BFS()
        {
            if (MazeGraph.StartNode == null)
                return null;
            Queue<Stack<MazeGraph>> paths = new Queue<Stack<MazeGraph>>();
            Stack<MazeGraph> stack = new Stack<MazeGraph>();
            stack.Push(MazeGraph.StartNode);
            paths.Enqueue(stack);
            while (paths.Count != 0)
            {
                stack = paths.Dequeue();
                MazeGraph curr = stack.Peek();
                //we have to create a temporary array and reverse it because the function for drawing the answer requires the stack in a certain format
                MazeGraph[] tmpArray = stack.ToArray();
                Array.Reverse(tmpArray);
                curr.Visited = true;
                if (curr.DownNeighbor != null && !curr.DownNeighbor.Visited)
                {
                    Stack<MazeGraph> tmp = new Stack<MazeGraph>(tmpArray);
                    tmp.Push(curr.DownNeighbor);
                    if (curr.DownNeighbor == MazeGraph.FinishNode)
                    {
                        curr.DownNeighbor.Visited = true;
                        return tmp;
                    }
                    paths.Enqueue(tmp);
                }
                if (curr.RightNeighbor != null && !curr.RightNeighbor.Visited)
                {
                    Stack<MazeGraph> tmp = new Stack<MazeGraph>(tmpArray);
                    tmp.Push(curr.RightNeighbor);
                    if (curr.RightNeighbor == MazeGraph.FinishNode)
                    {
                        curr.RightNeighbor.Visited = true;
                        return tmp;
                    }
                    paths.Enqueue(tmp);
                }
                if (curr.UpNeighbor != null && !curr.UpNeighbor.Visited)
                {
                    Stack<MazeGraph> tmp = new Stack<MazeGraph>(tmpArray);
                    tmp.Push(curr.UpNeighbor);
                    if (curr.UpNeighbor == MazeGraph.FinishNode)
                    {
                        curr.UpNeighbor.Visited = true;
                        return tmp;
                    }
                    paths.Enqueue(tmp);
                }
                if (curr.LeftNeighbor != null && !curr.LeftNeighbor.Visited)
                {
                    Stack<MazeGraph> tmp = new Stack<MazeGraph>(tmpArray);
                    tmp.Push(curr.LeftNeighbor);
                    if (curr.LeftNeighbor == MazeGraph.FinishNode)
                    {
                        curr.LeftNeighbor.Visited = true;
                        return tmp;
                    }
                    paths.Enqueue(tmp);
                }
            }
            return null;
        }

        private static Stack<MazeGraph> solveWithPriorityQueue(Func<uint,uint,uint,uint,uint> priorityFunction)
        {
            if (MazeGraph.StartNode == null)
                return null;
            PriorityQueue<Stack<MazeGraph>> paths = new PriorityQueue<Stack<MazeGraph>>();
            Stack<MazeGraph> stack = new Stack<MazeGraph>();
            stack.Push(MazeGraph.StartNode);
            paths.Insert(stack, 0);
            while (paths.Count != 0)
            {
                Tuple<Stack<MazeGraph>, uint> currPath = paths.Pop();
                stack = currPath.Item1;
                MazeGraph curr = stack.Peek();
                //we have to create a temporary array and reverse it because the function for drawing the answer requires the stack in a certain format
                MazeGraph[] tmpArray = stack.ToArray();
                Array.Reverse(tmpArray);
                curr.Visited = true;
                if (curr.DownNeighbor != null && !curr.DownNeighbor.Visited)
                {
                    Stack<MazeGraph> tmp = new Stack<MazeGraph>(tmpArray);
                    tmp.Push(curr.DownNeighbor);
                    if (curr.DownNeighbor == MazeGraph.FinishNode)
                    {
                        curr.DownNeighbor.Visited = true;
                        return tmp;
                    }
                    paths.Insert(tmp, priorityFunction(currPath.Item2, (uint)curr.DownDistance, (uint)curr.HeuristicValue, (uint)curr.DownNeighbor.HeuristicValue));
                }
                if (curr.RightNeighbor != null && !curr.RightNeighbor.Visited)
                {
                    Stack<MazeGraph> tmp = new Stack<MazeGraph>(tmpArray);
                    tmp.Push(curr.RightNeighbor);
                    if (curr.RightNeighbor == MazeGraph.FinishNode)
                    {
                        curr.RightNeighbor.Visited = true;
                        return tmp;
                    }
                    paths.Insert(tmp, priorityFunction(currPath.Item2, (uint)curr.RightDistance, (uint)curr.HeuristicValue, (uint)curr.RightNeighbor.HeuristicValue));
                }
                if (curr.UpNeighbor != null && !curr.UpNeighbor.Visited)
                {
                    Stack<MazeGraph> tmp = new Stack<MazeGraph>(tmpArray);
                    tmp.Push(curr.UpNeighbor);
                    if (curr.UpNeighbor == MazeGraph.FinishNode)
                    {
                        curr.UpNeighbor.Visited = true;
                        return tmp;
                    }
                    paths.Insert(tmp, priorityFunction(currPath.Item2, (uint)curr.UpDistance, (uint)curr.HeuristicValue, (uint)curr.UpNeighbor.HeuristicValue));
                }
                if (curr.LeftNeighbor != null && !curr.LeftNeighbor.Visited)
                {
                    Stack<MazeGraph> tmp = new Stack<MazeGraph>(tmpArray);
                    tmp.Push(curr.LeftNeighbor);
                    if (curr.LeftNeighbor == MazeGraph.FinishNode)
                    {
                        curr.LeftNeighbor.Visited = true;
                        return tmp;
                    }
                    paths.Insert(tmp, priorityFunction(currPath.Item2, (uint)curr.LeftDistance, (uint)curr.HeuristicValue, (uint)curr.LeftNeighbor.HeuristicValue));
                }
            }
            return null;
        }

        public static Stack<MazeGraph> BranchAndBound()
        {
            return solveWithPriorityQueue((element1, element2, element3, element4) =>
            {
                return element1 + element2;
            });
        }

        public static Stack<MazeGraph> BestFirst()
        {
            return solveWithPriorityQueue((element1, element2, element3, element4) =>
            {
                return element4;
            });
        }

        public static Stack<MazeGraph> AStar()
        {
            return solveWithPriorityQueue((element1, element2, element3, element4) =>
            {
                return element1 + element2 - element3 + element4;
            });
        }
    }
}
