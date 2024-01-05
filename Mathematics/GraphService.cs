using SharpAstrology.Enums;

namespace SharpAstrology.HumanDesign.Mathematics;

/// <summary>
/// Provides an operation to calculate the connected components of the Human Design graph.
/// </summary>
internal static class GraphService
{
    /// <summary>
    /// Calculates the connected centers based on the active channels.
    /// </summary>
    /// <param name="activeChannels">The collection of active channels.</param>
    /// <returns>A tuple containing a dictionary of centers and their corresponding component id, as well as the number of splits.</returns>
    public static (Dictionary<Centers, int>, int) ConnectedCenters(IEnumerable<Channels> activeChannels)
    {
        var vertices = new HashSet<Vertex>();
        var edges = new List<Edge>();

        foreach (var channel in activeChannels)
        {
            var (key1, key2) = channel.ToGates();
            var (center1, center2) = (key1.GetCenter(), key2.GetCenter());
            var v1 = new Vertex(center1);
            var v2 = new Vertex(center2);
            vertices.Add(v1);
            vertices.Add(v2);
            edges.Add(new Edge(v1, v2));
        }

        int componentId = 0;
        foreach (var vertex in vertices)
        {
            if (!vertex.Visited)
            {
                componentId++;
                DFS(vertex, componentId, edges);
            }
        }

        var dict = vertices.ToDictionary(v => v.Node, v => v.ComponentId);
        var splits = dict.Count == 0 ? 0 : dict.Max(x => x.Value);
        
        return (dict, splits);
    }

    /// <summary>
    /// Depth First Search method to traverse a graph starting from a given vertex.
    /// </summary>
    /// <param name="vertex">The starting vertex for the depth first search.</param>
    /// <param name="componentId">The component ID that will be assigned to the vertices connected to the given vertex.</param>
    /// <param name="edges">The list of edges in the graph.</param>
    private static void DFS(Vertex vertex, int componentId, List<Edge> edges)
    {
        vertex.Visited = true;
        vertex.ComponentId = componentId;
        foreach (var edge in edges.Where(e=>e.HasVertex(vertex)))
        {
            var v = edge.GetOther(vertex);
            if (!v.Visited)
            {
                DFS(v, componentId, edges);
            }
        }
    }
}

/// <summary>
/// Represents a center in the Human Design graph as vertex in a graph. </summary>
/// /
internal sealed class Vertex(Centers node)
{
    /// <summary>
    /// Flag variable indicating if this vertx has been visited or not.
    /// </summary>
    public bool Visited = false;
    
    /// <summary>
    /// The center that represents the vertex.
    /// </summary>
    public readonly Centers Node = node;
    
    /// <summary>
    /// The unique identifier for a component.
    /// </summary>
    public int ComponentId;
    
    public override int GetHashCode() => Node.GetHashCode();

    public override bool Equals(object? obj)
    {
        if (obj is Vertex vertex)
        {
            return Node == vertex.Node;
        }

        return false;
    }
}

/// <summary>
/// Represents an edge between two vertices.
/// </summary>
internal sealed class Edge(Vertex vertex1, Vertex vertex2)
{
    /// <summary>
    /// Determines whether the graph has the given vertex.
    /// </summary>
    /// <param name="vertex">The vertex to check.</param>
    /// <returns>True if the graph has the vertex; otherwise, false.</returns>
    public bool HasVertex(Vertex vertex) => vertex1.Equals(vertex) || vertex2.Equals(vertex);

    /// <summary>
    /// Returns the other vertex connected to a given vertex.
    /// </summary>
    /// <param name="self">The vertex for which the other vertex needs to be retrieved.</param>
    /// <returns>The other vertex connected to the given vertex.</returns>
    public Vertex GetOther(Vertex self) => vertex1.Equals(self) ? vertex2 : vertex1;
}
