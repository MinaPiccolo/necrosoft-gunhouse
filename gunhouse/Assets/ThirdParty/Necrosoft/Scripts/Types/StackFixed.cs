namespace Necrosoft.Types
{
    public class StackFixed<T>
    {
        T[] nodes;
        int count;

        public int Size { get { return count; } }

        public StackFixed(int size)
        {
            nodes = new T[size];
            count = 0;
        }

        public void Push(T item)
        {
            if (count >= nodes.Length) return;
            nodes[count++] = item;
        }

        public T Pop()
        {
            if (count <= 0) return default(T);
            return nodes[--count];
        }

        public T Peek()
        {
            if (count <= 0) return default(T);
            return nodes[count - 1];
        }
    }
}
