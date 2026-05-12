using ComputerScience.DataStructures;
using ComputerScience.DataStructures.Graphs;
using System.Security.Cryptography;

namespace ComputerScience.Core
{
    internal class Program
    {
        static void Main ()
        {
            int[] from = { 1, 5, 2, 3, 5, };
            int[] to = { 3, 4, 4, 5, 1 };
            int[] w = { 5, 4, 3, 2, 1 };
            DynamicGraph<Empty, Empty> g = new();
            for ( int i = 0; i < 10; i++ )
            {
                g.AddEdge(5, RandomNumberGenerator.GetInt32(0, 100));
            }
            Console.WriteLine(g);
        }
    }
}
