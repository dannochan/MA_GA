using System;
using MA_GA.Models;
using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Graphviz;
using QuikGraph.Graphviz.Dot;

namespace MA_GA.domain;

public static class GraphService
{

    /// <summary>
    ///  Creates a adjacency graph for the given data objects and their relations.
    /// </summary>
    /// <returns></returns>
    public static AdjacencyGraph<DataObject, IObjectRelation> CreateAdjacencyGraph()
    {
        var graph = new AdjacencyGraph<DataObject, IObjectRelation>(
            allowParallelEdges: true
        );
        return graph;
    }

    /// <summary>
    /// Generates a Graphviz DOT representation of the given graph.
    /// This method formats the vertices and edges according to their types and relations.
    /// The vertex shape is set to Ellipse for Information Objects and Rectangle for other types.
    /// The edge label is set to the relation type.
    /// The method throws an ArgumentNullException if the graph is null.
    /// </summary>
    /// <param name="graph"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>

    public static string GenerateGraphToDOT(AdjacencyGraph<DataObject, IObjectRelation> graph)
    {
        if (graph == null)
        {
            throw new ArgumentNullException("Graph cannot be null");
        }

        string graphviz = graph.ToGraphviz(algo =>
        {
            algo.CommonVertexFormat.Shape = GraphvizVertexShape.Rectangle;
            algo.FormatVertex += (sender, args) =>
            {
                var vertex = args.Vertex;
                args.VertexFormat.Label = vertex.Name;

                args.VertexFormat.Shape = vertex.ObjectType == ObjectType.InformationObject
                    ? GraphvizVertexShape.Ellipse
                    : GraphvizVertexShape.Rectangle;

            };

            algo.FormatEdge += (sender, args) =>
            {
                var edge = args.Edge;
                args.EdgeFormat.Label = new GraphvizEdgeLabel
                {
                    Value = edge.RelationType.ToString()
                };
            };
        }

        );

        return graphviz;


    }

    public static string DiplayGraphByComponents(AdjacencyGraph<DataObject, IObjectRelation> graph)
    {
        if (graph == null || graph.VertexCount == 0)
        {
            return "Graph is empty.";
        }

        var components = graph.Vertices
            .GroupBy(v => v.Component)
            .Select(g => new { Component = g.Key, Vertices = g.ToList() })
            .ToList();

        var result = new System.Text.StringBuilder();
        foreach (var component in components)
        {
            result.AppendLine($"Component: {component.Component}");
            foreach (var vertex in component.Vertices)
            {
                result.AppendLine($" - Vertex: {vertex.Name}, Weight: {vertex.Weight}");
            }
        }

        return result.ToString();
    }

    public static List<DataObject> GetVertexNeighbors(DataObject vertex, AdjacencyGraph<DataObject, IObjectRelation> graph)
    {

        return graph.Edges
            .Where(e => e.SourceObject.Equals(vertex) || e.TargetObject.Equals(vertex))
            .Select(e => e.SourceObject.Equals(vertex) ? (DataObject)e.TargetObject : (DataObject)e.SourceObject)
            .ToList();

    }

    /// <summary>
    /// Retrieves all incident elements (both vertices and edges) for a given modularisable element.
    /// If the element is an ObjectRelation, it returns the source and target objects.
    /// If the element is a DataObject, it retrieves all edges connected to that vertex.
    /// </summary>
    /// <param name="element">The modularisable element for which to find incident elements.</param>
    /// <param name="graph">The graph containing the modularisable elements.</param>
    /// <returns>A list of incident ModularisableElement objects.</returns>
    public static List<ModularisableElement> GetIncidentElements(ModularisableElement element, Graph graph)
    {
        var incidentElements = new List<ModularisableElement>();
        if (element is ObjectRelation)
        {
            var relation = (ObjectRelation)element;
            incidentElements.Add(relation.SourceObject);
            incidentElements.Add(relation.TargetObject);
        }
        else
        {
            var vertex = (DataObject)element;
            var edges = graph.GetGraph().Edges
                .Where(e => e.SourceObject.Equals(vertex) || e.TargetObject.Equals(vertex))
                .ToList();
            foreach (var edge in edges)
            {
                incidentElements.Add((ObjectRelation)edge);
            }
        }
        return incidentElements;
    }
    
    /// <summary>
    /// Creates a subgraph from the original graph based on the provided indices.
    /// The subgraph will include vertices and edges corresponding to the indices of modularisable elements.
    /// </summary>
    /// <param name="indices">List of indices representing the modularisable elements to include in the subgraph.</param>
    /// <param name="originalGraph">The original graph from which to create the subgraph.</param>
    /// <returns>A new AdjacencyGraph containing the subgraph.</returns>

    public static AdjacencyGraph<DataObject, IObjectRelation> CreateSubgraphGraphFromIndices(
    List<int> indices,
    Graph originalGraph

)
    {

        var modularisableElements = indices.Select(index => originalGraph.GetModularisableElementByIndex(index))
            .ToList();
        var vertices = modularisableElements.Select(me => me as DataObject)
            .Where(v => v != null)
            .Cast<DataObject>()
            .ToList();
        var edges = modularisableElements
            .Select(me => me as ObjectRelation)
            .Where(e => e != null)
            .Cast<ObjectRelation>()
            .ToList();

        var subgraph = CreateAdjacencyGraph();

        foreach (var vertex in vertices)
        {
            subgraph.AddVertex(vertex);
        }

        foreach (var edge in edges)
        {
            if (subgraph.ContainsVertex(edge.SourceObject) && subgraph.ContainsVertex(edge.TargetObject))
            {
                subgraph.AddEdge(edge);
            }
        }

        return subgraph;
    }


}
