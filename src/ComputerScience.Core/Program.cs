using ComputerScience.DataStructures.Graphs;
using System.Security.Cryptography;

namespace ComputerScience.Core
{
    public class Program
    {
        public static void Main ()
        {
            for ( int k = 0; k < 10; k++ )
            {
                GraphBuilder<Empty, Empty> gb = new(1000, 499500);
                for ( int i = 0; i < 499500; i++ )
                {
                    gb.AddArc(RandomNumberGenerator.GetInt32(1000), RandomNumberGenerator.GetInt32(1000));
                }
                DateTime start = DateTime.Now;
                Graph<Empty, Empty> g = gb.Build(GraphBuildingOptions.Default);
                Console.WriteLine(DateTime.Now - start);

                int dfs = 0, bfs = 0;
                start = DateTime.Now;
                Parallel.For(0, g.VertexCount, (int i) =>
                {
                    for ( int j = 0; j < g.VertexCount; j++ )
                    {
                        if ( g.DFSIsReachable(i, j) )
                        {
                            dfs++;
                        }
                    }
                });
                Console.WriteLine(DateTime.Now - start);
                start = DateTime.Now;
                Parallel.For(0, g.VertexCount, (int i) =>
                {
                    for ( int j = 0; j < g.VertexCount; j++ )
                    {
                        if ( g.BFSIsReachable(i, j) )
                        {
                            bfs++;
                        }
                    }
                });
                Console.WriteLine(DateTime.Now - start);
                Console.WriteLine(dfs);
                Console.WriteLine(bfs);

                dfs = 0;
                bfs = 0;
                start = DateTime.Now;
                for ( int i = 0; i < g.VertexCount; i++ )
                {
                    for ( int j = 0; j < g.VertexCount; j++ )
                    {
                        if ( g.DFSIsReachable(i, j) )
                        {
                            dfs++;
                        }
                    }
                }
                Console.WriteLine(DateTime.Now - start);
                start = DateTime.Now;
                for ( int i = 0; i < g.VertexCount; i++ )
                {
                    for ( int j = 0; j < g.VertexCount; j++ )
                    {
                        if ( g.BFSIsReachable(i, j) )
                        {
                            bfs++;
                        }
                    }
                }
                Console.WriteLine(DateTime.Now - start);
                Console.WriteLine(dfs);
                Console.WriteLine(bfs);
            }
        }
	}
}
