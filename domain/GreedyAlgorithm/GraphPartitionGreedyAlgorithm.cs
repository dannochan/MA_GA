using System;
using System.Runtime.CompilerServices;
using MA_GA.Models;
using QuikGraph;
using QuikGraph.Algorithms;

namespace MA_GA.domain.GreedyAlgorithm;

public class GraphPartitionGreedyAlgorithm
{

    private Dictionary<int, List<IObjectRelation>> PriorityList { get; set; }

    private Dictionary<string, int> VertexWeights { get; set; }

    private AdjacencyGraph<DataObject, IObjectRelation> InitGraph { get; set; }



    public GraphPartitionGreedyAlgorithm(AdjacencyGraph<DataObject, IObjectRelation> graph)
    {
        InitGraph = graph.Clone() ?? throw new ArgumentNullException(nameof(graph), "Graph cannot be null");
        PriorityList = new Dictionary<int, List<IObjectRelation>>();
        VertexWeights = new Dictionary<string, int>();
    }

    /// <summary>
    /// Creates a priority list based on the edges of the graph.
    /// The priority list is sorted by edge weight, and edges with the same weight are sorted by the sum of the weights of their source and target objects.
    /// </summary>/
    public void CreatePriorityList()
    {
        // Check if the graph has external relations
        var externalRelationList = this.CheckConnectionsToExternalObjects();

        if (externalRelationList.Count != 0)
        {
            this.HandleExternalRelations(externalRelationList);
        }

        // sort edges by weight of edge
        var sortedEdges = InitGraph.Edges.ToList()
            .OrderByDescending(edge => edge.Weight)
            .ToList();


        // group edges by weight and sort them by the sum of source and target object weights
        sortedEdges.GroupBy(edge => edge.Weight)
            .ToList().ForEach(
                group =>
                {
                    var sortedGroup = group.OrderByDescending(edge => edge.SourceObject.Weight + edge.TargetObject.Weight).ToList();
                    foreach (var edge in sortedGroup)
                    {
                        if (!PriorityList.ContainsKey(edge.Weight))
                        {
                            PriorityList.Add(edge.Weight, new List<IObjectRelation> { edge });
                        }
                        else
                        {
                            PriorityList[edge.Weight].Add(edge);
                        }
                    }
                }
            );
        // console output of the priority list
        Console.WriteLine("Priority List:");
        foreach (var item in PriorityList)
        {
            Console.WriteLine($"Weight: {item.Key}, Edges: {string.Join(", ", item.Value.Select(e => $"{e.EdgeNumber} : {e.SourceObject.Name} -> {e.TargetObject.Name}"))}");
        }

    }

    // graph partitioning algorithm
    public AdjacencyGraph<DataObject, IObjectRelation> PartitionGraph()
    {
        var newGraph = InitGraph.Clone() ?? throw new ArgumentNullException(nameof(InitGraph), "Graph cannot be null");


        // Implement the partitioning logic here

        if (PriorityList.Count == 0)
        {
            throw new InvalidOperationException("Priority list is empty. Please create the priority list before partitioning the graph.");
        }
        if (newGraph.VertexCount == 0 || newGraph.EdgeCount == 0)
        {
            throw new InvalidOperationException("Graph is empty. Cannot partition an empty graph.");
        }

        // Example partitioning logic: assign each vertex to a component value based on the priority list
        int componentValue = 1;
        foreach (var edgeGroup in PriorityList.OrderByDescending(kvp => kvp.Key))
        {
            foreach (var edge in edgeGroup.Value)
            {
                var sourceVertex = newGraph.Vertices.FirstOrDefault(v => v.Name == edge.SourceObject.Name);
                var targetVertex = newGraph.Vertices.FirstOrDefault(v => v.Name == edge.TargetObject.Name);
                if (sourceVertex == null || targetVertex == null)
                {
                    throw new InvalidOperationException($"Edge {edge.EdgeNumber} references non-existent vertices: {edge.SourceObject.Name} or {edge.TargetObject.Name}");
                }
                // Check if the source and target objects are already assigned to a component
                if (sourceVertex.Component == null && targetVertex.Component == null)
                {
                    // check if their neighbors are already assigned to a component
                   

                    // Assign both objects to the same component
                    sourceVertex.Component = $"Component_{componentValue}";
                    targetVertex.Component = $"Component_{componentValue}";
                    componentValue++;
                }
                else if (sourceVertex.Component != null && targetVertex.Component == null)
                {
                    // Assign target object to the same component as source object

                    targetVertex.Component = sourceVertex.Component;

                }
                else if (sourceVertex.Component == null && targetVertex.Component != null)
                {
                    // Assign source object to the same component as target object

                    sourceVertex.Component = targetVertex.Component;

                }
                // If both objects already have a component, they are already connected
            }

        }

        return newGraph;

    }



    private void HandleExternalRelations(List<IObjectRelation> externalRelationList)
    {
        throw new NotImplementedException();
    }

    private List<IObjectRelation> CheckConnectionsToExternalObjects()
    {
        var externalRelations = new List<IObjectRelation>();
        var Vertex_List = InitGraph.Vertices.ToList();

        foreach (var vertex in Vertex_List)
        {
            if (vertex.isExternalComponent != null && (bool)vertex.isExternalComponent)
            {

                foreach (var relation in vertex.Relations)
                {
                    if (relation.TargetObject.isExternalComponent == false)
                    {
                        externalRelations.Add(relation);
                    }
                }

            }
        }
        return externalRelations;

    }
}
