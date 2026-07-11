using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;

namespace ComputerScience.DataStructures.Graphs
{
	public class Graph<TEdgeWeight, TVertexData>
    {
        private readonly int[] offsets;
        private readonly int[] targets;
        private readonly TEdgeWeight[]? weights;
        private readonly TVertexData[]? data;

        private readonly ArrayPool<int> intPool = ArrayPool<int>.Shared;

		private readonly GraphBuildingOptionsFixed options;

        public int VertexCount { get; }
        public int ArcCount { get; }
        public bool HasWeight { get; }
        public bool HasVertexData { get; }

        public bool IsAlwaysReflexive => options.alwaysReflexiveArcs;
        public bool AllowReflexiveEdges => options.enableReflexiveArcs;
        public bool AllowDuplicateEdges => options.enableDuplicateArcs;

        public int ReflexiveVertexCount
        {
            get
            {
                if ( field > -1 )
                {
                    return field;
                }
                int res = 0;
                for ( int vertex = 0; vertex < VertexCount; vertex++ )
                {
                    ReadOnlySpan<int> neighbors = GetNeighbors(vertex);
                    int index = neighbors.BinarySearch(vertex);

					if ( index >= 0)
                    {
                        res++;
                        int pointerLow = index-1, pointerHigh = index+1;
                        bool flag = true;
                        while ( flag )
                        {
                            if ((pointerLow < 0 || neighbors[pointerLow] != vertex)
                                && (pointerHigh >= neighbors.Length || neighbors[pointerHigh] != vertex))
                            {
                                flag = false;
                            }
                            if ( pointerLow >= 0 && neighbors[pointerLow] == vertex )
                            {
                                res++;
                                pointerLow--;
                            }
                            if ( pointerHigh < neighbors.Length && neighbors[pointerHigh] == vertex )
                            {
                                res++;
                                pointerHigh++;
                            }
                        }
                    }
                }
                field = res;
                return field;
            }
        } = -1;
        public int DuplicateArcCount
        {
            get
            {
                if ( field > -1 )
                {
                    return field;
                }
                int res = 0;
                int currentNeighbor = -1;
                for ( int vertex = 0; vertex < VertexCount; vertex++ )
                {
                    ReadOnlySpan<int> neighbors = GetNeighbors(vertex);
                    foreach ( int neighbor in neighbors )
                    {
                        if ( currentNeighbor != neighbor )
                        {
                            currentNeighbor = neighbor;
						}
                        else
                        {
                            res++;
                        }
                    }
                }
                field = res;
                return field;
            }
        } = -1;
        public float AverageVertexDegree
        {
            get
            {
                if (field != -1f)
                {
                    return field;
                }
                field = (float) ArcCount / VertexCount;
				return field;
            }
        } = -1f;
        public int MaxVertexDegree
        {
            get
            {
                if ( field != -1f )
                {
                    return field;
                }
                int max = 0;
                for ( int vertex = 0; vertex < VertexCount; vertex++ )
                {
                    int neighborCount = offsets[vertex + 1] - offsets[vertex];

                    max = Math.Max(max, neighborCount);
                }
                field = max;
                return field;
            }
        } = -1;
        public float Density => (float) (ArcCount - ReflexiveVertexCount - DuplicateArcCount) / (VertexCount * (VertexCount - 1));

		internal Graph (int v, int[] from, int[] to, TEdgeWeight[]? w = null, TVertexData[]? d = null)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(v);
            ArgumentOutOfRangeException.ThrowIfNotEqual(from.Length, to.Length);
            ArgumentOutOfRangeException.ThrowIfNegative(from.Length);

            if ( typeof(TEdgeWeight) != typeof(Empty) )
            {
                ArgumentNullException.ThrowIfNull(w);
                ArgumentOutOfRangeException.ThrowIfNotEqual(from.Length, w.Length);
            }
            if ( typeof(TVertexData) != typeof(Empty) )
            {
                ArgumentNullException.ThrowIfNull(d);
                ArgumentOutOfRangeException.ThrowIfNotEqual(v, d.Length);
            }

            int u = from.Length;

            VertexCount = v;
            ArcCount = u;

            offsets = intPool.Rent(VertexCount + 1);
            offsets[VertexCount] = ArcCount;
            offsets.AsSpan(0, VertexCount).Clear();

			int[] neighborCount = intPool.Rent(VertexCount);
            neighborCount.AsSpan(0, VertexCount).Clear();

			for ( int i = 0; i < u; i++ )
            {
                int originVertex = from[i];
                neighborCount[originVertex]++;
            }

            for ( int i = 1; i < VertexCount; i++ )
            {
                offsets[i] = neighborCount[i - 1] + offsets[i - 1];
            }

            targets = intPool.Rent(u);
            targets.AsSpan(0, ArcCount).Clear();

			HasWeight = typeof(TEdgeWeight) != typeof(Empty);
            HasVertexData = typeof(TVertexData) != typeof(Empty);

            if ( HasWeight )
            {
                weights = new TEdgeWeight[u];
            }
            if ( HasVertexData )
            {
                data = d;
            }

            for ( int i = 0; i < u; i++ )
            {
                int originVertex = from[i];
                int originOffset = offsets[originVertex];
                int originNeighbors = --neighborCount[originVertex];
                targets[originOffset + originNeighbors] = to[i];
                if ( HasWeight )
                {
                    weights![originOffset + originNeighbors] = w![i];
                }
            }
            intPool.Return(neighborCount);
			for ( int vertex = 0; vertex < VertexCount; vertex++ )
			{
				Span<int> neighbors = targets.AsSpan(offsets[vertex], offsets[vertex + 1] - offsets[vertex]);
				if ( HasWeight )
				{
					Span<TEdgeWeight> neighborWeights = weights.AsSpan(offsets[vertex], offsets[vertex + 1] - offsets[vertex]);
					neighbors.Sort(neighborWeights);
				}
				else
				{
					neighbors.Sort();
				}
			}
		}

        internal Graph (int[] offsets, int[] targets, GraphBuildingOptionsFixed options, 
            TEdgeWeight[]? weights = null, TVertexData[]? data = null)
        {
            ArgumentNullException.ThrowIfNull(offsets);
            ArgumentNullException.ThrowIfNull(targets);

            if ( typeof(TEdgeWeight) != typeof(Empty) )
            {
                ArgumentNullException.ThrowIfNull(weights);
                ArgumentOutOfRangeException.ThrowIfNotEqual(targets.Length, weights.Length);
                HasWeight = true;
                this.weights = weights;
            }
            if ( typeof(TVertexData) != typeof(Empty) )
            {
                ArgumentNullException.ThrowIfNull(data);
                ArgumentOutOfRangeException.ThrowIfNotEqual(offsets.Length, data.Length + 1);
                HasVertexData = true;
                this.data = data;
            }

            this.offsets = offsets;
            this.targets = targets;

            VertexCount = offsets.Length - 1;
            ArcCount = targets.Length;

            this.options = options;

			for ( int vertex = 0; vertex < VertexCount; vertex++ )
			{
				Span<int> neighbors = targets.AsSpan(offsets[vertex], offsets[vertex + 1] - offsets[vertex]);
                if ( HasWeight )
                {
                    Span<TEdgeWeight> neighborWeights = weights.AsSpan(offsets[vertex], offsets[vertex + 1] - offsets[vertex]);
                    neighbors.Sort(neighborWeights);
                }
                else
                {
                    neighbors.Sort();
                }
			}
		}

        public override string ToString ()
        {
            StringBuilder sb = new();
            sb.AppendLine($"Граф имеет {VertexCount} вершин, {ArcCount} ребёр.");
            sb.Append(HasWeight ? "Граф является взвешенным и " : "Граф не является взвешенным и ");
            sb.AppendLine(HasVertexData ? "вершины содержат данные." : "вершины не содержат данные.");
            sb.Append(HasVertexData ? "Вершины графа в формате 'Вершина : данные вершины - " : "Вершины графа в формате 'Вершина - ");
            sb.AppendLine(HasWeight ? "(сосед1, вес1), (сосед2, вес2)...':" : "сосед1, сосед2,...':");
            for ( int i = 0; i < VertexCount; i++ )
            {
                int currentOffset = offsets[i], nextOffset = i + 1 == VertexCount ? targets.Length : offsets[i + 1];
                ReadOnlySpan<int> neighbors = targets.AsSpan(currentOffset, nextOffset - currentOffset);
                if ( !neighbors.IsEmpty )
                {
                    sb.Append($"{i} ");
                    if ( HasVertexData )
                    {
                        sb.Append($": {data[i]} ");
                    }
                    sb.Append("- ");
                    for ( int j = 0; j < neighbors.Length; j++ )
                    {
                        if ( HasWeight )
                        {
                            sb.Append($"({neighbors[j]}, {weights[currentOffset + j]}), ");
                        }
                        else
                        {
                            sb.Append($"{neighbors[j]}, ");
                        }
                    }
                    sb.Remove(sb.Length - 2, 2);
                    sb.AppendLine();
                }
            }
            return sb.ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<int> GetNeighbors (int v)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(v);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(v, offsets.Length - 1);

            return targets.AsSpan(offsets[v], offsets[v + 1] - offsets[v]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ReadOnlySpan<TEdgeWeight> GetWeights (int v)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(v);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(v, offsets.Length - 1);

            if ( !HasWeight )
            {
                throw new InvalidOperationException();
            }

            int start = offsets[v];
            int end = offsets[v + 1];
            ReadOnlySpan<TEdgeWeight> span = weights.AsSpan(start, end - start);
            return span;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TVertexData GetVertexData (int v)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(v);
			ArgumentOutOfRangeException.ThrowIfGreaterThan(v, offsets.Length - 1);

            if ( !HasVertexData )
            {
				throw new InvalidOperationException();
			}
			return data![v];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool BFSIsReachable (int start, int end)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(start);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(start, VertexCount);
            ArgumentOutOfRangeException.ThrowIfNegative(end);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(end, VertexCount);

            if ( IsAlwaysReflexive
                && start == end )
            {
                return true;
            }

            int qStart = 0, qEnd = 0;
			int[] visited = intPool.Rent(VertexCount);
            int[] queue = intPool.Rent(VertexCount);
            visited.AsSpan(0, VertexCount).Clear();
			queue.AsSpan(0, VertexCount).Clear();
			bool found = false;
            try
            {
                Enqueue(start);
                visited[start] = 1;
                while ( qEnd - qStart > 0 && !found)
                {
                    int curr = Dequeue();
                    ReadOnlySpan<int> neighbors = GetNeighbors(curr);
                    for ( int i = 0; i < neighbors.Length; i++ )
                    {
                        if ( neighbors[i] == end )
                        {
                            found = true;
                        }
                        if ( visited[neighbors[i]] == 0 )
                        {
                            Enqueue(neighbors[i]);
                            visited[neighbors[i]] = 1;
                        }
                    }
                }
            }
            finally
            {
                intPool.Return(visited);
                intPool.Return(queue);
            }
            return found;

            void Enqueue (int el)
            {
                queue[qEnd++] = el;
            }
            int Dequeue ()
            {
                return queue[qStart++];
            }
        }

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public Graph<TEdgeWeight, TVertexData> BFSGetReachableFrom (int v)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(v);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(v, VertexCount);

            int visitedCount = 0, qStart = 0, qEnd = 0, newVertexCounter = 0;
            int[] queue = intPool.Rent(VertexCount);
            int[] visited = intPool.Rent(VertexCount);
            int[] newVertexes = intPool.Rent(VertexCount);
            queue.AsSpan(0, VertexCount).Clear();
			visited.AsSpan(0, VertexCount).Clear();
			newVertexes.AsSpan(0, VertexCount).Clear();

			try
            {
                GraphBuilder<TEdgeWeight, TVertexData> builder = new(1, ArcCount);
				Enqueue(v);
                while ( qEnd > qStart )
                {
                    int curr = Dequeue();
                    visited[curr] = 1;
					ReadOnlySpan<int> neighbors = GetNeighbors(curr);
                    for ( int i = 0; i < neighbors.Length; i++ )
                    {
                        if ( visited[neighbors[i]] == 0 )
                        {
							newVertexes[neighbors[i]] = ++newVertexCounter;
							Enqueue(neighbors[i]);
							visited[neighbors[i]] = 1;
						}

                        builder.AddArc(newVertexes[curr], newVertexes[neighbors[i]]);
                    }
                }
                return builder.Build(options);
			}
            finally
            {
                intPool.Return(queue);
                intPool.Return(visited);
                intPool.Return(newVertexes);
            }

            void Enqueue (int el)
			{
				queue[qEnd++] = el;
			}
			int Dequeue ()
			{
				return queue[qStart++];
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void BFSAll (Action<int> action)
		{
			int visitedCount = 0, qStart = 0, qEnd = 0;
			int[] queue = intPool.Rent(VertexCount);
			int[] visited = intPool.Rent(VertexCount);
            try
            {
                for ( int i = 0; i < VertexCount; i++ )
                {
                    if ( visited[i] == 0 )
                    {
                        Enqueue(i);
                        while ( qEnd > qStart )
                        {
                            int curr = Dequeue();
                            if ( visited[curr] == 0 )
                            {
                                action(curr);
                                visited[curr] = 1;
                                visitedCount++;
                                ReadOnlySpan<int> neighbors = GetNeighbors(curr);
                                for ( int j = 0; j < neighbors.Length; j++ )
                                {
                                    if ( visited[neighbors[j]] == 0 )
                                    {
                                        Enqueue(neighbors[j]);
                                    }
                                }
                            }
                        }
                    }
                }
            }
            finally
            {
				intPool.Return(queue);
				intPool.Return(visited);
			}
            void Enqueue (int el)
			{
				queue[qEnd++] = el;
			}
			int Dequeue ()
			{
				return queue[qStart++];
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool DFSIsReachable (int start, int end)
        {
			ArgumentOutOfRangeException.ThrowIfNegative(start);
			ArgumentOutOfRangeException.ThrowIfGreaterThan(start, VertexCount);
			ArgumentOutOfRangeException.ThrowIfNegative(end);
			ArgumentOutOfRangeException.ThrowIfGreaterThan(end, VertexCount);

			if ( IsAlwaysReflexive
				&& start == end )
			{
				return true;
			}

            int stackEnd = 0;
			bool found = false;
			int[] visited = intPool.Rent(VertexCount);
			int[] stack = intPool.Rent(VertexCount);
			visited.AsSpan(0, VertexCount).Clear();
			stack.AsSpan(0, VertexCount).Clear();

            try
            {
                Push(start);
                while ( stackEnd > 0 && !found )
                {
                    int curr = Pop();
                    if ( visited[curr] <= 1 )
                    {
                        visited[curr] = 2;
                        ReadOnlySpan<int> neighbors = GetNeighbors(curr);
                        for ( int i = 0; i < neighbors.Length && !found; i++ )
                        {
                            if ( neighbors[i] == end )
                            {
                                found = true;
                            }
                            else if ( visited[neighbors[i]] == 0 )
                            {
                                visited[neighbors[i]] = 1;
                                Push(neighbors[i]);
                            }
                        }
                    }
                }
            }
            finally
            {
                intPool.Return(visited);
                intPool.Return(stack);
            }
            return found;
            void Push (int el)
            {
                stack[stackEnd++] = el;
            }
            int Pop ()
            {
                return stack[--stackEnd];
            }
        }
    }
}
