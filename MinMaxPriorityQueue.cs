using System;
using System.Collections.Generic;

namespace CCVMinMaxPriorityQueue
{
	public class MinMaxPriorityQueue
	{
		// My implementation of a double-ended priority queue
		// I chose to use a min-max heap as the internal representation of the queue because it allows both insertion and removal of nodes in O(log2 n) time
		// A min-max heap is a complete binary tree where it alternates between min and max levels, starting with a min level
		// The priority of any node on a min level is less than the priority of any of its descendants
		// The priority of any node on a max level is greater than the priority of any of its descendants
		// This class allows objects of any type to be the value for a given priority
		private class PriorityNode
		{
			public PriorityNode(Object value, int p) {
				data = value;
				priority = p;
			}

			private int priority;
			public int Priority
			{
				get { return priority; }
				set { priority = value; }
			}

			private Object data;
			public Object Data
			{
				get { return data; }
				set { data = value; }
			}
		}

		private List<PriorityNode> storage;

		public MinMaxPriorityQueue()
		{
			storage = new List<PriorityNode>();
			storage.Add(null);	// Dummy value since this works better with a list that starts with index 1
		}

		private void Swap(int a, int b)
		{
			PriorityNode temp = storage[a];
			storage[a] = storage[b];
			storage[b] = temp;
		}

		private int GetParent(int index)
		{
			return index / 2;
		}

		private int GetGrandparent(int index)
		{
			return index / 4;
		}

		private int GetLeftChild(int index)
		{
			return index * 2;
		}

		private int GetRightChild(int index)
		{
			return index * 2 + 1;
		}

		private void BubbleUpMax(int index)
		{
			if (index > 3 && storage[index].Priority > storage[GetGrandparent(index)].Priority)
			{
				Swap(index, GetGrandparent(index));
				BubbleUpMax(GetGrandparent(index));
			}
		}

		private void BubbleUpMin(int index)
		{
			if (index > 3 && storage[index].Priority < storage[GetGrandparent(index)].Priority)
			{
				Swap(index, GetGrandparent(index));
				BubbleUpMin(GetGrandparent(index));
			}
		}

		private void BubbleUp(int index)
		{
			var level = (int)Math.Log(index, 2);

			if (level % 2 == 0)	// The node is on a min-level
			{
				if (index != 1 && storage[index].Priority > storage[GetParent(index)].Priority)
				{
					// The node should be on a max-level
					Swap(index, GetParent(index));
					BubbleUpMax(GetParent(index));
				}
				else BubbleUpMin(index);
			}
			else	// The node is on a max-level
			{
				if (index != 1 && storage[index].Priority < storage[GetParent(index)].Priority)
				{
					// The node should be on a min-level
					Swap(index, GetParent(index));
					BubbleUpMin(GetParent(index));
				}
				else BubbleUpMax(index);
			}
		}

		private void TrickleDownMin(int index)
		{
			// Swap with smallest child or grandchild
			int firstGrandchild = index * 4;
			if (firstGrandchild < storage.Count)	// This node has grandchildren, so check them
			{
				int minIndex = firstGrandchild;

				for (int i = 1; i < 4; i++)
				{
					if (firstGrandchild + i == storage.Count) break;

					if (storage[firstGrandchild + i].Priority < storage[minIndex].Priority) minIndex = firstGrandchild + i;
				}

				if (storage[minIndex].Priority < storage[index].Priority)
				{
					Swap(index, minIndex);
					TrickleDownMin(minIndex);
				}
			}
			else	// Check the children
			{
				int minIndex = GetLeftChild(index);
				if (minIndex < storage.Count)
				{
					if (GetRightChild(index) < storage.Count && storage[GetRightChild(index)].Priority < storage[minIndex].Priority) minIndex = GetRightChild(index);

					if (storage[minIndex].Priority < storage[index].Priority) Swap(index, minIndex);
				}
			}
		}

		private void TrickleDownMax(int index)
		{
			// Swap with largest child or grandchild
			int firstGrandchild = index * 4;
			if (firstGrandchild < storage.Count)	// This node has grandchildren, so check them
			{
				int maxIndex = firstGrandchild;

				for (int i = 1; i < 4; i++)
				{
					if (firstGrandchild + i == storage.Count) break;

					if (storage[firstGrandchild + i].Priority > storage[maxIndex].Priority) maxIndex = firstGrandchild + i;
				}

				if (storage[maxIndex].Priority > storage[index].Priority)
				{
					Swap(index, maxIndex);
					TrickleDownMax(maxIndex);
				}
			}
			else	// Check the children
			{
				int maxIndex = GetLeftChild(index);
				if (maxIndex < storage.Count)
				{
					if (GetRightChild(index) < storage.Count && storage[GetRightChild(index)].Priority > storage[maxIndex].Priority) maxIndex = GetRightChild(index);

					if (storage[maxIndex].Priority > storage[index].Priority) Swap(index, maxIndex);
				}
			}
		}

		public void Enqueue<T>(T value, int priority)
		{
			PriorityNode newNode = new PriorityNode(value, priority);

			// Add new node as the next available leaf
			storage.Add(newNode);
			if (storage.Count > 2)
			{
				// Maintain the min-max ordering
				var index = storage.Count - 1;
				BubbleUp(index);
			}
		}

		public bool DequeueMin<T>(out T value)
		{
			if (storage.Count < 2)
			{
				value = default(T);
				return false;
			}

			value = (T)storage[1].Data;

			if (storage.Count > 2)
			{
				storage[1] = storage[storage.Count - 1];
				storage.RemoveAt(storage.Count - 1);
				TrickleDownMin(1);
			}
			else storage.RemoveAt(storage.Count - 1);

			return true;
		}

		public bool DequeueMax<T>(out T value)
		{
			if (storage.Count < 2)
			{
				value = default(T);
				return false;
			}

			if (storage.Count <= 3)
			{
				value = (T)storage[storage.Count - 1].Data;
				storage.RemoveAt(storage.Count - 1);
			}
			else
			{
				if (storage[2].Priority > storage[3].Priority)
				{
					value = (T)storage[2].Data;
					storage[2] = storage[storage.Count - 1];
					storage.RemoveAt(storage.Count - 1);
					TrickleDownMax(2);
				}
				else
				{
					value = (T)storage[3].Data;
					if (storage.Count > 4)
					{
						storage[3] = storage[storage.Count - 1];
						storage.RemoveAt(storage.Count - 1);
						TrickleDownMax(3);
					}
					else storage.RemoveAt(storage.Count - 1);
				}
			}

			return true;
		}
	}
}

