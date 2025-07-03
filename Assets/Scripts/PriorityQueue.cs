using UnityEngine;
using System;
using System.Collections.Generic;

public class PriorityQueue<T> where T : IComparable<T>
{
        private List<(T item, double priority)> heap = new List<(T, double)>();
        private Dictionary<T, int> indexMap = new Dictionary<T, int>();

        public int Count => heap.Count;

        public void Enqueue(T item, double priority)
        {
            heap.Add((item, priority));
            int i = heap.Count - 1;
            indexMap[item] = i;
            HeapifyUp(i);
        }

        public T Dequeue()
        {
            if (heap.Count == 0) throw new InvalidOperationException("Queue is empty.");
            var item = heap[0].item;

            indexMap.Remove(item);
            Swap(0, heap.Count - 1);
            heap.RemoveAt(heap.Count - 1);
            HeapifyDown(0);

            return item;
        }

        public bool Contains(T item)
        {
            return indexMap.ContainsKey(item);
        }

        public void UpdatePriority(T item, double newPriority)
        {
            if (!indexMap.ContainsKey(item)) return;

            int i = indexMap[item];
            double oldPriority = heap[i].priority;
            heap[i] = (item, newPriority);

            if (newPriority < oldPriority)
                HeapifyUp(i);
            else
                HeapifyDown(i);
        }

        private void HeapifyUp(int i)
        {
            while (i > 0)
            {
                int parent = (i - 1) / 2;
                if (heap[i].priority >= heap[parent].priority) break;

                Swap(i, parent);
                i = parent;
            }
        }

        private void HeapifyDown(int i)
        {
            int count = heap.Count;
            while (true)
            {
                int smallest = i;
                int left = 2 * i + 1;
                int right = 2 * i + 2;

                if (left < count && heap[left].priority < heap[smallest].priority)
                    smallest = left;
                if (right < count && heap[right].priority < heap[smallest].priority)
                    smallest = right;

                if (smallest == i) break;

                Swap(i, smallest);
                i = smallest;
            }
        }

        private void Swap(int i, int j)
        {
            var temp = heap[i];
            heap[i] = heap[j];
            heap[j] = temp;

            indexMap[heap[i].item] = i;
            indexMap[heap[j].item] = j;
        }
}
