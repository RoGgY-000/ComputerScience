namespace ComputerScience.DataStructures.Graphs
{
	public class GraphBuildingOptions
	{
		private bool _alwaysReflexiveEdges = false;
		private bool _enableReflexiveEdges = true;
		private bool _enableDuplicateEdges = true;

		public static GraphBuildingOptionsFixed Default { get; } = new GraphBuildingOptions().Fix();

		public GraphBuildingOptions AlwaysReflexiveEdges ()
		{
			_alwaysReflexiveEdges = true;
			return this;
		}
		public GraphBuildingOptions NeverReflexiveEdges ()
		{
			_alwaysReflexiveEdges = false;
			return this;
		}
		public GraphBuildingOptions EnableReflexiveEdges ()
		{
			_enableReflexiveEdges = true;
			return this;
		}
		public GraphBuildingOptions DisableReflexiveEdges ()
		{
			_enableReflexiveEdges = false;
			return this;
		}
		public GraphBuildingOptions EnableDuplicateEdges ()
		{
			_enableDuplicateEdges = true;
			return this;
		}
		public GraphBuildingOptions DisableDuplicateEdges ()
		{
			_enableDuplicateEdges = false;
			return this;
		}

		public GraphBuildingOptionsFixed Fix ()
		{
			GraphBuildingOptionsFixed options = new(_alwaysReflexiveEdges, _enableReflexiveEdges, _enableDuplicateEdges);
			return options;
		}
	}
}
