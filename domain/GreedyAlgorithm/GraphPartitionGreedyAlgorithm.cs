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

    }

    private void HandleExternalRelations(List<IObjectRelation> externalRelationList)
    {
        throw new NotImplementedException();
    }

    private List<IObjectRelation> CheckConnectionsToExternalObjects()
    {
        var externalRelations = new List<IObjectRelation>();
        var Vertex_List  = InitGraph.Vertices.ToList();

        foreach (var vertex in Vertex_List)
        {
            if ((bool)vertex.isExternalComponent)
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
