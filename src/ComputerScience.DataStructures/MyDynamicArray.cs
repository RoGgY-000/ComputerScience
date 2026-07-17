using System.Buffers;
using System.Collections;

namespace ComputerScience.DataStructures
{
	public class MyDynamicArray<T> : ICollection<T>
	{
		public int Count { get; private set; }
		public int Capacity => array.Length;

		public bool IsReadOnly => throw new NotImplementedException();

		private const int DefaultCapacity = 8;
		private readonly ArrayPool<T> _pool = ArrayPool<T>.Shared;
		private T?[] array;

		public MyDynamicArray () : this(DefaultCapacity) { }

		public MyDynamicArray (int capacity)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(capacity);

			if ( capacity == 0 )
			{
				array = Array.Empty<T>();
			}
			array = _pool.Rent(capacity);
			Count = capacity;
		}

		public MyDynamicArray (IEnumerable<T> source)
		{
			ArgumentNullException.ThrowIfNull(source);

			array = _pool.Rent(DefaultCapacity);
			foreach ( T item in source )
			{
				Add(item);
			}
		}

		public MyDynamicArray (int capacity, IEnumerable<T> source)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(capacity);
			ArgumentNullException.ThrowIfNull(source);

			array = _pool.Rent(capacity);
			foreach ( T item in source )
			{
				Add(item);
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

		public void Clear () => array.AsSpan().Clear();

		public bool Contains (T item)
		{
			ArgumentNullException.ThrowIfNull(item);

			return array.AsSpan().Contains(item);
		}
		// stopped here
		public void CopyTo (T[] dest, int start)
		{
			ArgumentNullException.ThrowIfNull(dest);
			ArgumentOutOfRangeException.ThrowIfNegative(start);
			ArgumentOutOfRangeException.ThrowIfGreaterThan(start + Count, dest.Length);

			array.AsSpan().CopyTo(dest);
			for ( int i = 0; i < Count; i++ )
			{
				if ( array[i] != null )
				{
					dest[i] = array[i + start]!;
				}
			}
		}

		public void Remove (T item)
		{
			ArgumentNullException.ThrowIfNull(item);

			for ( int i = 0; i < Count; i++ )
			{
				if ( array[i] is not null
					&& item.Equals(array[i]) )
				{
					array[i] = default;
				}
			}
		}
		private void SizeUp ()
		{
			T[] newArr = new T[Capacity * 2];
			array.CopyTo(newArr, 0);
			array = newArr;
		}

		bool ICollection<T>.Remove (T item) => throw new NotImplementedException();
		public IEnumerator<T> GetEnumerator () => throw new NotImplementedException();
		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
	}
}
