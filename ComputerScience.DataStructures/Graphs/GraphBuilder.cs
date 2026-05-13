using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerScience.DataStructures.Graphs
{
    public class GraphBuilder<TEdgeWeight, TVertexData>
    {
        private const int MinEdgeCapacity = 4;

        private int[] heads;
        private int[] counts;
        private int[] targets;
        private int[] nexts;
        private TEdgeWeight[]? weights;
        private TVertexData[]? data;

        public bool HasWeight { get; }
        public bool HasVertexData { get; }

        public int VertexCount { get; private set; }
        public int EdgeCount { get; private set; }
        public int EdgeCapacity { get; private set; }

        public GraphBuilder (int v = 1, int u = 0)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(v);
            ArgumentOutOfRangeException.ThrowIfNegative(u);

            HasVertexData = typeof(TVertexData) != typeof(Empty);
            HasWeight = typeof(TEdgeWeight) != typeof(Empty);

            EnsureVertexCount(v);
            EnsureEdgeCapacity(Math.Max(u, MinEdgeCapacity));
        }

        public void AddEdge (int from, int to)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(from);
            ArgumentOutOfRangeException.ThrowIfNegative(to);

            if ( HasWeight
                || weights != null )
            {
                throw new InvalidOperationException();
            }

            EnsureVertexCount(Math.Max(from, to));
            if ( EdgeCount == EdgeCapacity )
            {
                SizeUpEdges();
            }

            targets[EdgeCount] = to;
            nexts[EdgeCount] = heads[from];
            heads[from] = EdgeCount;
            counts[from]++;

            EdgeCount++;
        }

        public void AddEdge (int from, int to, TEdgeWeight w)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(from);
            ArgumentOutOfRangeException.ThrowIfNegative(to);
            ArgumentNullException.ThrowIfNull(w);

            if ( !HasWeight
                || weights == null )
            {
                throw new InvalidOperationException();
            }

            EnsureVertexCount(Math.Max(from, to)+1);
            if ( EdgeCount == EdgeCapacity )
            {
                SizeUpEdges();
            }

            targets[EdgeCount] = to;
            nexts[EdgeCount] = heads[from];
            heads[from] = EdgeCount;
            counts[from]++;
            weights[EdgeCount] = w;

            EdgeCount++;
        }

        public void SetVertexData (int v, TVertexData d)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(v);
            ArgumentNullException.ThrowIfNull(d);
            if ( !HasVertexData
                || data == null )
            {
                throw new InvalidOperationException();
            }
            EnsureVertexCount(v+1);
            data[v] = d;
        }

        public Graph<TEdgeWeight, TVertexData> Build ()
        {
            int[] offsets = new int[VertexCount + 1];
            int[] targets = new int[EdgeCount];
            offsets[VertexCount] = EdgeCount;

            TEdgeWeight[]? newWeights = null;
            if ( HasWeight
                && weights != null )
            {
                newWeights = new TEdgeWeight[EdgeCount];
            }

            offsets[0] = 0;
            int pointer = heads[0];

            for ( int i = 0; i < counts[0]; i++ )
            {
                targets[i] = this.targets[pointer];
                pointer = nexts[pointer];
            }

            for ( int i = 1; i < VertexCount; i++ )
            {
                offsets[i] = offsets[i - 1] + counts[i - 1];
                pointer = heads[i];
                for ( int j = 1; j < counts[i]; j++ )
                {
                    targets[offsets[i] + j] = this.targets[pointer];
                    pointer = nexts[pointer];
                    if ( HasWeight )
                    {
                        newWeights![offsets[i] + j] = weights![pointer];
                    }
                }
            }
            Graph<TEdgeWeight, TVertexData> g = new(offsets, targets, newWeights, data);
            return g;
        }

        private void EnsureEdgeCapacity (int capacity)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(capacity);

            if ( EdgeCapacity < capacity )
            {
                EdgeCapacity = capacity;
                Array.Resize(ref targets, EdgeCapacity);
                Array.Resize(ref nexts, EdgeCapacity);

                if ( HasWeight )
                {
                    Array.Resize(ref weights, EdgeCapacity);
                }
            }
            else
            {
                targets ??= new int[EdgeCapacity];
                nexts ??= new int[EdgeCapacity];
                if ( HasWeight )
                {
                    weights ??= new TEdgeWeight[EdgeCapacity];
                }
            }
        }

        private void EnsureVertexCount (int capacity)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(capacity);

            if ( VertexCount < capacity )
            {
                VertexCount = capacity;
                Array.Resize(ref heads, VertexCount);
                Array.Resize(ref counts, VertexCount);

                if ( HasVertexData )
                {
                    Array.Resize(ref data, VertexCount);
                }
            }
            else
            {
                heads ??= new int[VertexCount];
                counts ??= new int[VertexCount];
                if ( HasVertexData )
                {
                    data ??= new TVertexData[VertexCount];
                }
            }
        }

        private void FullArray (int[] arr, int value)
        {
            ArgumentNullException.ThrowIfNull(arr);

            for ( int i = 0; i < arr.Length; i++ )
            {
                if ( counts[i] == 0 )
                {
                    arr[i] = value;
                }
            }
        }

        private void SizeUpVertexes ()
        {
            VertexCount *= 2;

            Array.Resize(ref heads, VertexCount);
            Array.Resize(ref counts, VertexCount);
            FullArray(heads, -1);

            if ( HasVertexData
                && data != null )
            {
                Array.Resize(ref data, VertexCount);
            }
        }

        private void SizeUpEdges ()
        {
            EdgeCapacity *= 2;

            Array.Resize(ref targets, EdgeCapacity);
            Array.Resize(ref nexts, EdgeCapacity);

            if ( HasWeight
                && weights != null )
            {
                Array.Resize(ref weights, EdgeCapacity);
            }
        }
    }
}