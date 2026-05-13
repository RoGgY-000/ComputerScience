using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerScience.DataStructures.Graphs
{
    public class Graph<TEdgeWeight, TVertexData>
    {
        private readonly int[] offsets;
        private readonly int[] targets;
        private readonly TEdgeWeight[]? weights;
        private readonly TVertexData[]? data;

        public int VertexCount { get; }
        public int EdgeCount { get; }
        public bool HasWeight { get; }
        public bool HasVertexData { get; }

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
                ArgumentOutOfRangeException.ThrowIfNotEqual(from.Length, d.Length);
            }

            int u = from.Length;

            VertexCount = v;
            EdgeCount = u;

            offsets = new int[v + 1];
            offsets[v] = u;

            int[] neighborCount = new int[v];

            for ( int i = 0; i < u; i++ )
            {
                int originVertex = from[i];
                neighborCount[originVertex]++;
            }

            for ( int i = 1; i < v; i++ )
            {
                offsets[i] = neighborCount[i - 1] + offsets[i - 1];
            }

            targets = new int[u];

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
        }

        internal Graph (int[] offsets, int[] targets, TEdgeWeight[]? weights = null, TVertexData[]? data = null)
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
            EdgeCount = targets.Length;
        }

        public override string ToString ()
        {
            StringBuilder sb = new();
            sb.AppendLine($"Граф имеет {VertexCount} вершин, {EdgeCount} ребёр.");
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

        public ReadOnlySpan<int> GetNeighbors (int v)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(v);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(v, offsets.Length - 1);

            int start = offsets[v];
            int end = offsets[v+1];
            ReadOnlySpan<int> span = targets.AsSpan(start,end - start);
            return span;
        }

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
    }
}
