using System.Collections.Generic;
using System.Linq;

public class PriorityQueue<T> {
    private SortedList<float, Queue<T>> elements = new SortedList<float, Queue<T>>();
    private HashSet<T> elementSet = new HashSet<T>();

    public int Count => elementSet.Count;

    public void Enqueue(T item, float priority) {
        if (!elements.ContainsKey(priority)) {
            elements[priority] = new Queue<T>();
        }
        elements[priority].Enqueue(item);
        elementSet.Add(item);
    }

    public T Dequeue() {
        var firstPair = elements.First();
        var item = firstPair.Value.Dequeue();
        if (firstPair.Value.Count == 0) {
            elements.Remove(firstPair.Key);
        }
        elementSet.Remove(item);
        return item;
    }

    public bool Contains(T item) {
        return elementSet.Contains(item);
    }
}
