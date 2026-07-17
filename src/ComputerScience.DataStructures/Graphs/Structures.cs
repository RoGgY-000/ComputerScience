namespace ComputerScience.DataStructures.Graphs
{
    public struct Empty;

    public readonly struct GraphBuildingOptionsFixed
    {
		internal readonly bool alwaysReflexiveArcs;
		internal readonly bool enableReflexiveArcs;
		internal readonly bool enableDuplicateArcs;

		internal GraphBuildingOptionsFixed (bool alwaysReflexive, bool enableReflexive, bool enableDuplicate)
		{
			alwaysReflexiveArcs = alwaysReflexive;
			enableReflexiveArcs = enableReflexive;
			enableDuplicateArcs = enableDuplicate;
		}
	}
}
