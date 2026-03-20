using System.Collections;

namespace ComputerScience.DataStructures
{
    public class MyLinkedList<T> : IEnumerable<T>, ICollection<T>, IList<T>
    {

        private Node? first;
        private Node? last;
        public T? First
        {
            get => first != null ? first.Data : throw new NullReferenceException();
            set
            {
                if (first == null)
                {
                    throw new NullReferenceException();
                }
                first.Data = value;
            }
        }
        public T? Last
        {
            get => last != null ? last.Data : throw new NullReferenceException();
            set
            {
                if (last == null || value == null)
                {
                    throw new NullReferenceException();
                }
                last.Data = value;
            }
        }
        public int Count { get; set; }

        public bool IsReadOnly { get; }

        public MyLinkedList(int length)
        {
            if (length < 0)
            {
                throw new ArgumentException("Length cannot be negative!");
            }
            else if (length == 0)
            {
                return;
            }
            else if (length == 1)
            {
                first = new Node(data: default);
                First = default;
                Last = First;
                last = first;
            }
            else
            {
                first = new Node(data: default);
                First = default;
                Node current = first;
                Node next;
                for (int i = 1; i < length; i++)
                {
                    next = new Node(data: default);
                    current.Next = next;
                    next.Previous = current;
                    current = next;
                }
                last = current;
            }
            Count = length;
        }

        public void Add(T value)
        {
            Node newNode = new(value);
            if (Count == 0)
            {
                first = newNode;
                last = newNode;
            }
            else
            {
                last!.Next = newNode;
                newNode.Previous = last;
                last = newNode;
            }
            Count++;
        }

        public void Insert(int index, T value)
        {
            if (index < 0 || index >= Count)
            {
                throw new IndexOutOfRangeException();
            }
            Node newNode = new(value);
            if (Count == 1)
            {
                first = newNode;
                last = newNode;
            }
            else if (index == 0)
            {
                first!.Next!.Previous = newNode;
                newNode.Next = first;
                first!.Previous = newNode;
                first = newNode;
            }
            else if (index == Count - 1)
            {
                last!.Previous!.Next = newNode;
                newNode.Previous = last.Previous;
                newNode!.Next = last;
                last.Previous = newNode;
                last = newNode;
            }
            else
            {
                Node target = GetNodeByIndex(index);
                target.Previous!.Next = newNode;
                newNode.Previous = target.Previous;
                target.Previous = newNode;
                newNode.Next = target;
            }
            Count++;
        }

        public bool Remove(T value)
        {
            if (Count == 0)
            {
                throw new IndexOutOfRangeException();
            }
            else if (First!.Equals(value))
            {
                first = first!.Next;
                first!.Previous = null;
                Count--;
                return true;
            }
            else if (Last!.Equals(value))
            {
                last = last!.Previous;
                last!.Next = null;
                Count--;
                return true;
            }
            else
            {
                Node current = first!;
                while (current.Next != null)
                {
                    current = current.Next;
                    if (current.Data!.Equals(value))
                    {
                        current.Previous!.Next = current.Next;
                        current.Next!.Previous = current.Previous;
                        Count--;
                        return true;
                    }
                }
            }
            return false;
        }

        private Node GetNodeByIndex(int index)
        {
            if (index < 0 || index >= Count)
            {
                throw new IndexOutOfRangeException();
            }
            if (index < Count / 2)
            {
                Node current = first!;
                for (int i = 0; i < index; i++)
                {
                    current = current.Next!;
                }
                return current;
            }
            else
            {
                Node current = last!;
                for (int i = Count - 1; i > index; i--)
                {
                    current = current.Previous!;
                }
                return current;
            }
        }

        public void Clear()
        {
            if (Count == 0)
            {
                return;
            }
            else if (Count == 1)
            {
                first = null;
                last = null;
                return;
            }
            Node current = first!;
            for (int i = 0; i < Count - 1; i++)
            {
                current = current.Next!;
                current.Previous = null;
            }
            if (last != null)
            {
                last = null;
            }
            Count = 0;
        }

        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= Count)
                {
                    throw new IndexOutOfRangeException();
                }
                if (index < Count / 2)
                {
                    Node current = first!;
                    for (int i = 0; i < index; i++)
                    {
                        current = current.Next!;
                    }
                    return current.Data ?? throw new NullReferenceException();
                }
                else
                {
                    Node current = last!;
                    for (int i = Count - 1; i > index; i--)
                    {
                        current = current.Previous!;
                    }
                    return current.Data ?? throw new NullReferenceException();
                }
            }
            set
            {
                if (index < 0 || index >= Count)
                {
                    throw new IndexOutOfRangeException();
                }
                Node current = first!;
                for (int i = 0; i < index; i++)
                {
                    current = current.Next!;
                }
                current.Data = value;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            Node? current = first;
            while (current != null)
            {
                yield return current.Data!;
                current = current.Next;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class Node(T? data)
        {
            public T? Data { get; set; } = data;
            public Node? Next { get; set; }
            public Node? Previous { get; set; }
        }
    }
}
