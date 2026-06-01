using ComputerScience.DataStructures;
using ComputerScience.DataStructures.Graphs;
using ComputerScience.DataStructures.Graphs.Algorithms;
using System.Security.Cryptography;
using System.Collections;

namespace ComputerScience.Core
{
    internal class Program
    {
        static void Main ()
        {
			GraphBuilder<Empty, Empty> g = new(100, 100);
            for ( int i = 0; i < 100; i++ )
            {
                g.AddEdge(RandomNumberGenerator.GetInt32(0, 100), RandomNumberGenerator.GetInt32(0, 100));
            }

            DateTime start = DateTime.Now;
            Graph<Empty, Empty> sg = g.Build(new GraphBuildingOptions().AlwaysReflexiveEdges().GetOptions());

            //Console.WriteLine(sg);
            Console.WriteLine(DateTime.Now - start);

            start = DateTime.Now;
            Graph<Empty, Empty> sg1 = sg.BFSGetReachableFrom(10);
            Console.WriteLine(DateTime.Now - start);

            Console.WriteLine(sg1);
            Console.WriteLine(GC.GetAllocatedBytesForCurrentThread());
        }
    }
}
