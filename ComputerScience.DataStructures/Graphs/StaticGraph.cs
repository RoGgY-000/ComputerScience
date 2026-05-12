using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerScience.DataStructures.Graphs
{
    public class StaticGraph<TEdgeWeight, TVertexData>
    {
        private readonly int[] offsets;
        private readonly int[] targets;
        private readonly TEdgeWeight[]? weights;
        private readonly TVertexData[]? data;

        public bool HasWeight { get; }
        public bool HasVertexData { get; }

        internal StaticGraph (int v, int[] from, int[] to, TEdgeWeight[]? w = null, TVertexData[]? d = null)
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
