using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerScience.DataStructures.Graphs
{
    public struct Empty;

    public readonly struct GraphBuildingOptionsFixed
    {
		internal readonly bool alwaysReflexiveEdges;
		internal readonly bool enableReflexiveEdges;
		internal readonly bool enableDuplicateEdges;

		internal GraphBuildingOptionsFixed (bool alwaysReflexive, bool enableReflexive, bool enableDuplicate)
		{
			alwaysReflexiveEdges = alwaysReflexive;
			enableReflexiveEdges = enableReflexive;
			enableDuplicateEdges = enableDuplicate;
		}
	}
}
