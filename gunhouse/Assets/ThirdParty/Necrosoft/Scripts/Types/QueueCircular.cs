namespace Necrosoft.Types
{
    public class QueueCircular<T>
    {
        T[] nodes;
        int front;
        int back;
        public int length { get { return nodes.Length; } }

        public QueueCircular(int size)
        {
            nodes = new T[size];
        }

        public void Enqueue(T item)
        {
            nodes[back++] = item;
            back = back >= nodes.Length ? 0 : back;
        }

        public T Dequeue()
        {
            int i = front;
            front = ++front >= nodes.Length ? 0 : front;

            return nodes[i];
        }
    }
}
