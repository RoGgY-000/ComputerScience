using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerScience.DataStructures
{
    public class MyDynamicArray<T> : ICollection<T>
    {
        private const int defaultSize = 16;
        private T?[] array;

        public int Count { get; private set;  }
        public int Capacity => array.Length;

        public MyDynamicArray () : this (defaultSize) { }

        public MyDynamicArray (int capacity)
        {
            array = new T[capacity];
            Count = capacity;
        }

        public MyDynamicArray (IEnumerable<T> source)
        {
            Count = source.Count();
            array = new T[Count];
            int i = 0;
            foreach ( T item in source )
            {
                array[i] = item;
            }
        }

        public MyDynamicArray (int capacity, IEnumerable<T> source)
        {
            Count = source.Count();
            if ( capacity < 0
                || capacity > Count )
            {
                throw new IndexOutOfRangeException();
            }
            array = new T[capacity];
            int i = 0;
            foreach ( T item in source )
            {
                array[i] = item;
            }
        }

        public void Add (T item)
        {
            if ( Count == Capacity )
            {
                SizeUp();
            }
            array[Count++] = item;
        }

        public void Clear ()
        {
            for ( int i = 0; i < Count; i++ )
            {
                array[i] = default;
            }
        }

        public bool Contains (T item)
        {
            if ( item == null )
            {
                throw new ArgumentNullException();
            }
            for ( int i = 0; i < Count; i++ )
            {
                if ( item.Equals(array[i]) )
                {
                    return true;
                }
            }
            return false;
        }

        public void SizeUp ()
        {
            T[] newArr = new T[Capacity*2];
            array.CopyTo(newArr, 0);
            array = newArr;
        }
    }
}
