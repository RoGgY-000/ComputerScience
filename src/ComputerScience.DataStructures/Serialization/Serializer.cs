using System.Text;
using ComputerScience.DataStructures.Graphs;

namespace ComputerScience.DataStructures.Serialization
{
	public static class Serializer
	{
		public static void Serialize<TEdgeWeight, TVertexData> (Graph<TEdgeWeight, TVertexData> graph, string filePath, GraphSerializationFormat format)
		{
			ArgumentNullException.ThrowIfNull(graph);
			ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

			FileStream? fs = null;
			try
			{
				fs = File.Open(filePath, FileMode.OpenOrCreate);
			}
			catch
			{
				throw new IOException("File path is incorrect");
			}
			finally
			{
				fs?.Close();
			}

			switch ( format )
			{
				case GraphSerializationFormat.DOT:
					SerializeGraphAsDOT(graph, Path.ChangeExtension(filePath, "dot"));
					break;
				case GraphSerializationFormat.GraphML:
					break;
				default:
					throw new NotImplementedException("This format is not supported yet");
			}
		}

		private static void SerializeGraphAsDOT<TEdgeWeight, TVertexData> (Graph<TEdgeWeight, TVertexData> graph, string filePath)
		{
			StringBuilder sb = new("digraph {");

			for ( int i = 0; i < graph.VertexCount; i++ )
			{
				ReadOnlySpan<int> neighbors = graph.GetNeighbors(i);
				if ( neighbors.Length > 0 )
				{
					sb.AppendFormat("\r\n    {0} -> ", i);
					sb.Append("{ ");
					for ( int j = 0; j < neighbors.Length; j++ )
					{
						sb.AppendFormat("{0} ", neighbors[j]);
					}
					sb.Append("};");
				}
			}

			sb.AppendLine("\r\n}");

			File.WriteAllText(filePath, sb.ToString());
		}
	}
}
