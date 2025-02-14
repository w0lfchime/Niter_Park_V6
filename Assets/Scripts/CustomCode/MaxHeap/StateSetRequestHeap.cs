using System;
using System.Collections.Generic;

public class StateSetRequestHeap<T>
{
	private List<(T item, int priority, int pushFrame)> heap = new();

	public int Count => heap.Count;

	public void Enqueue(T item, int priority, int pushFrame)
	{
		heap.Add((item, priority, pushFrame));
		HeapifyUp(heap.Count - 1);
	}

	public T Dequeue()
	{
		if (heap.Count == 0) throw new InvalidOperationException("Heap is empty");

		T top = heap[0].item;
		heap[0] = heap[^1];
		heap.RemoveAt(heap.Count - 1);
		HeapifyDown(0);
		return top;
	}

	public T Peek()
	{
		if (heap.Count == 0) throw new InvalidOperationException("Heap is empty");
		return heap[0].item;
	}

	private void HeapifyUp(int index)
	{
		while (index > 0)
		{
			int parent = (index - 1) / 2;
			if (heap[index].priority > heap[parent].priority ||
			   (heap[index].priority == heap[parent].priority && heap[index].pushFrame > heap[parent].pushFrame))
			{
				(heap[index], heap[parent]) = (heap[parent], heap[index]);
				index = parent;
			}
			else break;
		}
	}

	private void HeapifyDown(int index)
	{
		while (index < heap.Count)
		{
			int left = 2 * index + 1, right = 2 * index + 2, largest = index;

			if (left < heap.Count &&
				(heap[left].priority > heap[largest].priority ||
				(heap[left].priority == heap[largest].priority && heap[left].pushFrame > heap[largest].pushFrame)))
				largest = left;

			if (right < heap.Count &&
				(heap[right].priority > heap[largest].priority ||
				(heap[right].priority == heap[largest].priority && heap[right].pushFrame > heap[largest].pushFrame)))
				largest = right;

			if (largest == index) break;

			(heap[index], heap[largest]) = (heap[largest], heap[index]);
			index = largest;
		}
	}

	public void Clear() => heap.Clear();
}
