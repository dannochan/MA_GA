using System;
using System.Runtime.CompilerServices;
using MA_GA.Models;
using QuikGraph;

namespace MA_GA.domain.GreedyAlgorithm;

public class GraphPartitionGreedyAlgorithm
{

    private Dictionary<int, string> PriorityList { get; set; }

    private Dictionary<string, int> VertexWeights { get; set; }

    private AdjacencyGraph<DataObject, IObjectRelation> InitGraph { get; set; }



    public GraphPartitionGreedyAlgorithm(AdjacencyGraph<DataObject, IObjectRelation> graph)
    {
        InitGraph = graph.Clone() ?? throw new ArgumentNullException(nameof(graph), "Graph cannot be null");
        PriorityList = new Dictionary<int, string>();
        VertexWeights = new Dictionary<string, int>();
    }

    // create pritority list based on the vertex weights
    public void CreatePriorityList()
    {
        // Check if the graph has external relations
        var externalRelationList = this.CheckConnectionsToExternalObjects();

        if (externalRelationList.Count != 0)
        {
            this.HandleExternalRelations(externalRelationList);
        }

        // sort edges by weight
        var sortedEdges = InitGraph.Edges.ToList()
            .OrderByDescending(edge => edge.Weight)
            .ToList();
        var CurrentEdgeWeight = sortedEdges.FirstOrDefault().Weight;
        sortedEdges.GroupBy(edge => edge.Weight)
            .ToList().ForEach(
                group =>
                {
                    // sort edges by the sum of source and target object weights
                    var sortedGroup = group.OrderByDescending(edge => edge.SourceObject.Weight + edge.TargetObject.Weight).ToList();
                    foreach (var edge in sortedGroup)
                    {
                        if (!PriorityList.ContainsKey(edge.Weight))
                        {
                            PriorityList.Add(edge.Weight, $"{edge.EdgeNumber}");
                        }
                        else
                        {
                            PriorityList[edge.Weight] += $", {edge.EdgeNumber}";
                        }
                    }
                }
            );
        // console output of the priority list
        Console.WriteLine("Priority List:");
        foreach (var item in PriorityList)
        {
            Console.WriteLine($"Weight: {item.Key}, Edges: {item.Value}");
        }

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
