using System;
using System.Collections.Generic;
using System.Text;

namespace ComputerScience.DataStructures.Graphs
{
	public class GraphBuildingOptions
	{
		private bool alwaysReflexiveEdges = false;
		private bool enableReflexiveEdges = true;
		private bool enableDuplicateEdges = true;

		public static GraphBuildingOptionsFixed Default { get; } = new GraphBuildingOptions().Fix();

		public GraphBuildingOptions AlwaysReflexiveEdges ()
		{
			alwaysReflexiveEdges = true;
			return this;
		}
		public GraphBuildingOptions NeverReflexiveEdges ()
		{
			alwaysReflexiveEdges = false;
			return this;
		}
		public GraphBuildingOptions EnableReflexiveEdges ()
		{
			enableReflexiveEdges = true;
			return this;
		}
		public GraphBuildingOptions DisableReflexiveEdges ()
		{
			enableReflexiveEdges = false;
			return this;
		}
		public GraphBuildingOptions EnableDuplicateEdges ()
		{
			enableDuplicateEdges = true;
			return this;
		}
		public GraphBuildingOptions DisableDuplicateEdges ()
		{
			enableDuplicateEdges = false;
			return this;
		}

		public GraphBuildingOptionsFixed Fix ()
		{
			GraphBuildingOptionsFixed options = new(alwaysReflexiveEdges, enableReflexiveEdges, enableDuplicateEdges);
			return options;
		}
	}
}
