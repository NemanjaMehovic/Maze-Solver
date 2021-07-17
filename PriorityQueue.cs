using System;
using System.Collections.Generic;
using System.Text;

namespace MazeSolver
{
    class PriorityQueue<T>
    {
        private List<Tuple<T, uint>> queue;

        public int Count { get => queue.Count; }

        public PriorityQueue()
        {
            queue = new List<Tuple<T, uint>>();
        }

        //If two elements have the same priority fifo is not guaranteed
        public void Insert(T element, uint priority)
        {
            Tuple<T, uint> tmpElement = new Tuple<T, uint>(element, priority);
            int index = queue.BinarySearch(tmpElement, Comparer<Tuple<T, uint>>.Create((a, b) =>
            {
                if (a.Item2 > b.Item2)
                    return 1;
                else if (a.Item2 < b.Item2)
                    return -1;
                return 0;
            }));
            if (index < 0)
                index = ~index;
            queue.Insert(index, tmpElement);
        }

        public Tuple<T, uint> Peek()
        {
            if (queue.Count == 0)
                return null;
            return queue[0];
        }

        public Tuple<T, uint> Pop()
        {
            if (queue.Count == 0)
                return null;
            Tuple<T, uint> tmp = queue[0];
            queue.RemoveAt(0);
            return tmp;
        }
    }
}
