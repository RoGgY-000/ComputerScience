using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ComputerScience.DataStructures
{
    public class MyDynamicArray<T> : ICollection<T>
    {
        private const int defaultCapacity = 4;
        private T?[] array;

        public int Count { get; private set;  }
        public int Capacity => array.Length;

        public bool IsReadOnly => throw new NotImplementedException();

        public MyDynamicArray () : this (defaultCapacity) { }

        public MyDynamicArray (int capacity)
        {
            if (capacity < 0)
            {
                throw new ArgumentOutOfRangeException();
            }
            if ( capacity == 0 )
            {
                array = Array.Empty<T>();
            }
            array = new T[capacity];
            Count = capacity;
        }

        public MyDynamicArray (IEnumerable<T> source)
        {
            Count = source.Count();
            array = new T?[Count];
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
            array = new T?[capacity];
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

        public void CopyTo (T[] dest, int arrayIndex)
        {
            if ( arrayIndex < 0
                || arrayIndex + Count > dest.Length )
            {
                throw new IndexOutOfRangeException();
            }
            if ( dest == null )
            {
                throw new ArgumentNullException();
            }
            for ( int i = 0; i < Count; i++ )
            {
                if ( array[i] != null )
                {
                    dest[i] = array[i + arrayIndex]!;
                }
            }
        }

        public void Remove (T item)
        {
            for ( int i = 0; i < Count; i++ )
            {
                if ( array[i] is not null
                    && item.Equals(array[i]) )
                {
                    array[i] = default;
                }
            }
        }
        public void SizeUp ()
        {
            T[] newArr = new T[Capacity*2];
            array.CopyTo(newArr, 0);
            array = newArr;
        }

        bool ICollection<T>.Remove (T item) => throw new NotImplementedException();
        public IEnumerator<T> GetEnumerator () => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
    }
}
