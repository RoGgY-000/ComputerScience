namespace ComputerScience.DataStructures.Graphs
{
	public class GraphBuilder<TEdgeWeight, TVertexData>
	{
		private const int MinCapacity = 4;

		public bool HasWeight { get; }
		public bool HasVertexData { get; }

		public int VertexCount { get; private set; }
		public int EdgeCount { get; private set; }
		public int EdgeCapacity { get; private set; }

		private int _vertexCapacity;
		private int[] _heads;
		private int[] _targets;
		private int[] _nexts;
		private TEdgeWeight[]? _weights;
		private TVertexData[]? _data;

		public GraphBuilder (int v = 1, int u = 0)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(v);
			ArgumentOutOfRangeException.ThrowIfNegative(u);

			HasVertexData = typeof(TVertexData) != typeof(Empty);
			HasWeight = typeof(TEdgeWeight) != typeof(Empty);

			EnsureVertexCount(v);
			EnsureEdgeCapacity(Math.Max(u, MinCapacity));
		}

		public void AddArc (int from, int to)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(from);
			ArgumentOutOfRangeException.ThrowIfNegative(to);

			if ( HasWeight
				|| _weights != null )
			{
				throw new InvalidOperationException();
			}

			EnsureVertexCount(Math.Max(from, to) + 1);
			EnsureEdgeCapacity(EdgeCount + 1);

            InsertArc(from, to);
        }

		public void AddArc (int from, int to, TEdgeWeight w)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(from);
			ArgumentOutOfRangeException.ThrowIfNegative(to);
			ArgumentNullException.ThrowIfNull(w);

			if ( !HasWeight
				|| _weights == null )
			{
				throw new InvalidOperationException();
			}

			EnsureVertexCount(Math.Max(from, to) + 1);
			EnsureEdgeCapacity(EdgeCount + 1);

            InsertArc(from, to, w);
        }

        public void AddEdge (int v1, int v2)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(v1);
            ArgumentOutOfRangeException.ThrowIfNegative(v2);

            if ( HasWeight
                || _weights != null )
            {
                throw new InvalidOperationException();
            }

            EnsureVertexCount(Math.Max(v1, v2)+1);
            EnsureEdgeCapacity(EdgeCount + 2);

            InsertArc(v1, v2);
            InsertArc(v2, v1);
		}

        public void AddEdge (int v1, int v2, TEdgeWeight w)
        {
			ArgumentOutOfRangeException.ThrowIfNegative(v1);
			ArgumentOutOfRangeException.ThrowIfNegative(v2);
            ArgumentNullException.ThrowIfNull(w);

            if ( !HasWeight
                || _weights == null )
            {
                throw new InvalidOperationException();
            }

            EnsureVertexCount(Math.Max(v1, v2) + 1);
            EnsureEdgeCapacity(EdgeCount + 2);

			InsertArc(v1, v2, w);
			InsertArc(v2, v1, w);
		}

		public void SetVertexData (int v, TVertexData d)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(v);
			ArgumentNullException.ThrowIfNull(d);
			if ( !HasVertexData
				|| _data == null )
			{
				throw new InvalidOperationException();
			}
			EnsureVertexCount(v + 1);
			_data[v] = d;
		}

		public Graph<TEdgeWeight, TVertexData> Build (GraphBuildingOptionsFixed options)
		{
			int[] offsets = new int[VertexCount + 1];
			int[] _targets = new int[EdgeCount];
			offsets[VertexCount] = EdgeCount;
			TEdgeWeight[]? newWeights = null;
			if ( HasWeight
				&& _weights != null )
			{
				newWeights = new TEdgeWeight[EdgeCount];
			}

			bool addReflexive = !options.alwaysReflexiveArcs && options.enableReflexiveArcs;

			offsets[0] = 0;
			int pointer = _heads[0];
			int current = 0;
			while ( pointer != -1 )
			{
				if ( addReflexive
					|| (options.alwaysReflexiveArcs
					&& this._targets[pointer] != 0) )
				{
					_targets[current] = this._targets[pointer];
					current++;
					if ( HasWeight )
					{
						newWeights![current] = _weights![pointer];
					}
				}
				pointer = _nexts[pointer];
			}

			for ( int i = 1; i < VertexCount; i++ )
			{
				offsets[i] = offsets[i - 1] + current;
				pointer = _heads[i];
				current = 0;
				while ( pointer != -1 )
				{
					if ( addReflexive
						|| (options.alwaysReflexiveArcs
						&& this._targets[pointer] != i) )
					{
						_targets[offsets[i] + current] = this._targets[pointer];
						current++;
						if ( HasWeight )
						{
							newWeights![offsets[i] + current] = _weights![pointer];
						}
					}
					pointer = _nexts[pointer];
				}
			}
			Graph<TEdgeWeight, TVertexData> g = new(offsets, _targets, options, newWeights, _data);
			return g;
		}

		private void EnsureEdgeCapacity (int capacity)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(capacity);

			if ( capacity <= EdgeCapacity )
			{
				return;
			}
			else
			{
				EdgeCapacity = MinCapacity;
				while ( EdgeCapacity < capacity )
				{
					EdgeCapacity *= 2;
				}
				Array.Resize(ref _targets, EdgeCapacity);
				Array.Resize(ref _nexts, EdgeCapacity);
				_nexts.AsSpan(EdgeCount, EdgeCapacity - EdgeCount).Fill(-1);

				if ( HasWeight )
				{
					Array.Resize(ref _weights, EdgeCapacity);
				}
			}
		}

		private void EnsureVertexCount (int count)
		{
			ArgumentOutOfRangeException.ThrowIfNegative(count);

			if ( count <= VertexCount )
			{
				return;
			}
			else if ( count <= _vertexCapacity )
			{
				VertexCount = count;
			}
			else
			{
				_vertexCapacity = MinCapacity;
				while ( _vertexCapacity < count )
				{
					_vertexCapacity *= 2;
				}
				Array.Resize(ref _heads, _vertexCapacity);
				_heads.AsSpan(VertexCount, _vertexCapacity - VertexCount).Fill(-1);

				if ( HasVertexData )
				{
					Array.Resize(ref _data, VertexCount);
				}
				VertexCount = count;
			}
        }

        private void InsertArc (int from, int to)
        {
			_targets[EdgeCount] = to;
			_nexts[EdgeCount] = _heads[from];
			_heads[from] = EdgeCount;
			EdgeCount++;
		}

        private void InsertArc (int from, int to, TEdgeWeight w)
        {
			_targets[EdgeCount] = to;
			_nexts[EdgeCount] = _heads[from];
			_heads[from] = EdgeCount;
            _weights![EdgeCount] = w;
			EdgeCount++;
		}
    }
}