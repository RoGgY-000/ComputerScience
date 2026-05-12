using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerScience.DataStructures.Graphs
{
    public class DynamicGraph<TEdgeWeight, TVertexData>
    {
        private const int MinCapacity = 4;

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
        public int VertexCapacity { get; private set; }
        public int EdgeCapacity { get; private set; }

        public DynamicGraph (int v = 0, int u = 0)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(v);
            ArgumentOutOfRangeException.ThrowIfNegative(u);

            VertexCount = v;
            VertexCapacity = MinCapacity;
            while ( VertexCapacity < VertexCount )
            {
                VertexCapacity *= 2;
            }

            EdgeCount = u;
            EdgeCapacity = MinCapacity;
            while ( EdgeCapacity < EdgeCount )
            {
                EdgeCapacity *= 2;
            }

            HasVertexData = typeof(TVertexData) != typeof(Empty);
            HasWeight = typeof(TEdgeWeight) != typeof(Empty);

            heads = Array.Empty<int>();
            counts = Array.Empty<int>();
            targets = Array.Empty<int>();
            nexts = Array.Empty<int>();

            SizeUpVertexes();
            SizeUpEdges();
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

            EnsureVertexCapacity(Math.Max(from, to)+1);

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

            EnsureVertexCapacity(Math.Max(from, to)+1);

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

        public void AddVertex ()
        {
            if ( HasVertexData
                || data != null )
            {
                throw new InvalidOperationException();
            }

            if ( VertexCount == VertexCapacity )
            {
                SizeUpVertexes();
            }

            VertexCount++;
        }

        public void AddVertex (TVertexData d)
        {
            ArgumentNullException.ThrowIfNull(d);

            if ( !HasVertexData
                || data == null )
            {
                throw new InvalidOperationException();
            }

            ArgumentNullException.ThrowIfNull(d);

            if ( VertexCount == VertexCapacity )
            {
                SizeUpVertexes();
            }

            data[VertexCount] = d;
            VertexCount++;
        }

        public StaticGraph<TEdgeWeight, TVertexData> ToStaticGraph ()
        {
            int[] from = new int[EdgeCount];
            int[] to = new int[EdgeCount];
            TEdgeWeight[]? newWeights = null;
            if ( HasWeight )
            {
                newWeights = new TEdgeWeight[EdgeCount];
            }
            int nextFree = 0, currNode = 0,  prevNode = 0, lastTarget = 0;
            for ( int i = 0; i < VertexCount; i++ )
            {
                currNode = heads[i];
                for ( int j = 0; j < counts[i]; j++ )
                {
                    lastTarget = targets[currNode];
                    prevNode = nexts[currNode];
                    from[nextFree] = i;
                    to[nextFree] = lastTarget;

                    if ( HasWeight )
                    {
                        newWeights![nextFree] = weights![currNode];
                    }

                    currNode = prevNode;
                    nextFree++;
                }
            }
            StaticGraph<TEdgeWeight, TVertexData> g = new(VertexCount, from, to, newWeights, data);
            return g;
        }

        private void EnsureVertexCapacity (int capacity)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(capacity);

            if ( VertexCapacity < capacity )
            {
                VertexCapacity = capacity;

                Array.Resize(ref heads, VertexCapacity);
                Array.Resize(ref counts, VertexCapacity);
                FullArray(heads, -1);

                if ( HasVertexData
                    && data != null )
                {
                    Array.Resize(ref data, VertexCapacity);
                }
            }
        }

        private void FullArray (int[] arr, int value)
        {
            ArgumentNullException.ThrowIfNull(arr);

            for ( int i = 0; i < arr.Length; i++ )
            {
                if ( counts[i] == 0)
                { 
                    arr[i] = value;
                }
            }
        }

        private void SizeUpVertexes ()
        {
            VertexCapacity *= 2;

            Array.Resize(ref heads, VertexCapacity);
            Array.Resize(ref counts, VertexCapacity);
            FullArray(heads, -1);

            if ( HasVertexData
                && data != null )
            {
                Array.Resize(ref data, VertexCapacity);
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
