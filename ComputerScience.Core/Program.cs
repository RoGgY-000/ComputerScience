using ComputerScience.DataStructures;
using ComputerScience.DataStructures.Graphs;
using System.Security.Cryptography;

namespace ComputerScience.Core
{
    internal class Program
    {
        static void Main ()
        {
            DateTime start = DateTime.Now;
            GraphBuilder<int, int> g = new(1, 1);
            for ( int i = 0; i < 100; i++ )
            {
                for ( int j = 0; j < 100; j++ )
                {
                    for ( int k = 1; k < 100; k++ )
                    {
                        g = new(i, j);
                        for ( int i2 = 0; i2 < 100; i2++ )
                        {
                            g.SetVertexData(i2, i2 + 1);
                        }
                        for ( int i2 = 0; i2 < 10000; i2++ )
                        {
                            g.AddEdge(RandomNumberGenerator.GetInt32(0, k), RandomNumberGenerator.GetInt32(0, k), 1000 - k);
                        }
                    }
                }
            }
            Graph<int, int> sg = g.Build();
            Console.WriteLine(DateTime.Now-start);
        }
    }
}
