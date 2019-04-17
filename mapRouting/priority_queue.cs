using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mapRouting
{

    class PriorityQueue<T> where T : pair
    {
        private List<T> list;
        public int Count { get { return list.Count; } }
        public readonly bool IsDescending;

        public PriorityQueue()
        {
            list = new List<T>();
        }

        public PriorityQueue(bool isdesc)
            : this()
        {
            IsDescending = isdesc;
        }
                                               
        public int CompareTo(T first , T sec) //O(1).
        {
            if (first.time< sec.time)
                return -1;
            else if (first.time == sec.time)
                return 0;
            else
                return 1;
        }

        public void Enqueue(T x)   //O(log n).
        {
            list.Add(x);
            int i = Count - 1;

            while (i > 0)
            {
                int p = (i - 1) / 2; //parent
                if ((IsDescending ? -1 : 1) *CompareTo(list[p],x) <= 0) break;

                list[i] = list[p];
                i = p;
            }

            if (Count > 0) list[i] = x;
        }

        public T Dequeue()    //O(log n).
        {
            T target = Peek();
            T root = list[Count - 1];
            list.RemoveAt(Count - 1);

            int i = 0;
            while (i * 2 + 1 < Count)
            {
                int a = i * 2 + 1;
                int b = i * 2 + 2;
                int c = b < Count && (IsDescending ? -1 : 1) *CompareTo(list[b],list[a]) < 0 ? b : a;

                if ((IsDescending ? -1 : 1) * CompareTo(list[c],root) >= 0) break;
                list[i] = list[c];
                i = c;
            }

            if (Count > 0) list[i] = root;
            return target;
        }

        public T Peek() //O(1).
        {
            if (Count == 0) throw new InvalidOperationException("Queue is empty.");
            return list[0];
        }

        public void Clear()   //O(1).
        {
            list.Clear();
        }
    }
}
