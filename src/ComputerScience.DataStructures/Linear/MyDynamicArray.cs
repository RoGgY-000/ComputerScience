using System.Buffers;
using System.Collections;

namespace ComputerScience.DataStructures.Linear
{
	public class MyDynamicArray<T> : ICollection<T>
	{
		public int Count { get; private set; }
		public int Capacity => _array.Length;

		public bool IsReadOnly => throw new NotImplementedException();

		private const int DefaultCapacity = 8;
		private readonly ArrayPool<T> _pool = ArrayPool<T>.Shared;
		private T[] _array;

		public MyDynamicArray () : this(DefaultCapacity) { }

		public MyDynamicArray (int capacity)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(capacity);

			if ( capacity == 0 )
			{
				_array = Array.Empty<T>();
			}
			_array = _pool.Rent(capacity);
			Count = capacity;
		}

		public MyDynamicArray (IEnumerable<T> source)
		{
			ArgumentNullException.ThrowIfNull(source);

			_array = _pool.Rent(DefaultCapacity);
			foreach ( T item in source )
			{
				Add(item);
			}
		}

		public MyDynamicArray (int capacity, IEnumerable<T> source)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(capacity);
			ArgumentNullException.ThrowIfNull(source);

			_array = _pool.Rent(capacity);
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
			_array[Count++] = item;
		}

		public void Clear () => _array.AsSpan().Clear();

		public bool Contains (T item)
		{
			ArgumentNullException.ThrowIfNull(item);

			return _array.AsSpan().Contains(item);
		}
		// stopped here
		public void CopyTo (T[] dest, int start)
		{
			ArgumentNullException.ThrowIfNull(dest);
			ArgumentOutOfRangeException.ThrowIfNegative(start);
			ArgumentOutOfRangeException.ThrowIfGreaterThan(start + Count, dest.Length);

			_array.AsSpan().CopyTo(dest);
		}

		public bool Remove (T item)
		{
			ArgumentNullException.ThrowIfNull(item);

			int index = -1;
			EqualityComparer<T> comparer = EqualityComparer<T>.Default;
			for ( int i = 0; i < Count; i++ )
			{
				if ( comparer.Equals(item, _array[i]) )
				{
					index = i;
					break;
				}
			}

			if ( index < 0 )
			{
				return false;
			}

			for ( int i = index; i + 1 < Count; i++ )
			{
				_array[i] = _array[i + 1];
			}

			Count--;
			return true;
		}
		private void SizeUp ()
		{
			T[] newArr = _pool.Rent(Capacity * 2);
			_array.AsSpan().CopyTo(newArr);
			_array = newArr;
			_pool.Return(_array);
		}

		public IEnumerator<T> GetEnumerator () => throw new NotImplementedException();
		IEnumerator IEnumerable.GetEnumerator () => GetEnumerator();
	}
}
