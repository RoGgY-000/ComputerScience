using System;
using System.Collections.Generic;
using System.Text;
using BenchmarkDotNet.Attributes;

namespace ComputerScience.DataStructures.Graphs
{
	public class GraphBuilder<TEdgeWeight, TVertexData>
    {
        private const int MinCapacity = 4;

        private int vertexCapacity;

        private int[] heads;
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
            EnsureEdgeCapacity(Math.Max(u, MinCapacity));
        }

        public void AddArc (int from, int to)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(from);
            ArgumentOutOfRangeException.ThrowIfNegative(to);

            if ( HasWeight
                || weights != null )
            {
                throw new InvalidOperationException();
            }

            EnsureVertexCount(Math.Max(from, to)+1);
            EnsureEdgeCapacity(EdgeCount + 1);

            targets[EdgeCount] = to;
            nexts[EdgeCount] = heads[from];
            heads[from] = EdgeCount;

            EdgeCount++;
        }

        public void AddArc (int from, int to, TEdgeWeight w)
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
			EnsureEdgeCapacity(EdgeCount+1);

			targets[EdgeCount] = to;
            nexts[EdgeCount] = heads[from];
            heads[from] = EdgeCount;
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

        public Graph<TEdgeWeight, TVertexData> Build (GraphBuildingOptionsFixed options)
        {
            int[] offsets = new int[VertexCount+1];
            int[] targets = new int[EdgeCount];
            offsets[VertexCount] = EdgeCount;
            TEdgeWeight[]? newWeights = null;
            if ( HasWeight
                && weights != null )
            {
                newWeights = new TEdgeWeight[EdgeCount];
            }

            bool addReflexive = !options.alwaysReflexiveEdges && options.enableReflexiveEdges;

			offsets[0] = 0;
            int pointer = heads[0];
            int current = 0;
            while ( pointer != -1 )
            {
                if ( addReflexive
					|| (options.alwaysReflexiveEdges
                    && this.targets[pointer] != 0))
                {
                    targets[current] = this.targets[pointer];
                    current++;
                    if ( HasWeight )
                    {
                        newWeights![current] = weights![pointer];
                    }
                }
                pointer = nexts[pointer];
            }

            for ( int i = 1; i < VertexCount; i++ )
            {
                offsets[i] = offsets[i - 1] + current;
                pointer = heads[i];
                current = 0;
                while ( pointer != -1 )
                {
                    if ( addReflexive
					    || (options.alwaysReflexiveEdges
					    && this.targets[pointer] != i) )
                    {
                        targets[offsets[i] + current] = this.targets[pointer];
                        current++;
                        if ( HasWeight )
                        {
                            newWeights![offsets[i] + current] = weights![pointer];
                        }
                    }
                    pointer = nexts[pointer];
                }
            }
            Graph<TEdgeWeight, TVertexData> g = new(offsets, targets, options, newWeights, data);
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
				Array.Resize(ref targets, EdgeCapacity);
				Array.Resize(ref nexts, EdgeCapacity);
                nexts.AsSpan(EdgeCount, EdgeCapacity - EdgeCount).Fill(-1);

				if ( HasWeight )
				{
					Array.Resize(ref weights, EdgeCapacity);
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
            else if (count <= vertexCapacity)
            {
                VertexCount = count;
            }
            else
            {
                vertexCapacity = MinCapacity;
				while ( vertexCapacity < count )
				{
					vertexCapacity *= 2;
				}
				Array.Resize(ref heads, vertexCapacity);
                heads.AsSpan(VertexCount, vertexCapacity - VertexCount).Fill(-1);

				if ( HasVertexData )
				{
					Array.Resize(ref data, VertexCount);
				}
                VertexCount = count;
			}
        }
    }
}