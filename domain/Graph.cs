using System;
using MA_GA.domain;
using QuikGraph;
using QuikGraph.Graphviz;

namespace MA_GA.Models;

/// <summary>
/// Graph class represents a directed graph structure containing nodes and edges.
/// It allows adding nodes, relations, and connections between nodes.
/// It also provides methods to retrieve nodes by ID or name, remove nodes and edges,
/// and read the current state of the graph.
/// </summary>

public class Graph
{

    List<IDataObject> nodeObjects;
    List<IObjectRelation> edgeObjects;

    // properties for creating graph using quikgraph
    private readonly Dictionary<int, IDataObject> _nodeDictionary = [];
    private readonly Dictionary<int, IObjectRelation> _edgeDictionary = [];

    private readonly AdjacencyGraph<DataObject, IObjectRelation> _Graph;


    public Graph()
    {
        nodeObjects = new List<IDataObject>();
        edgeObjects = new List<IObjectRelation>();
        _Graph = GraphService.CreateAdjacencyGraph();

    }

    public bool IsEmpty()
    {
        return nodeObjects.Count == 0;
    }

    public void AddNodeToGraph(DataObject dataObject)
    {
        nodeObjects.Add(dataObject);
        _Graph.AddVertex(dataObject);
        _nodeDictionary.Add(dataObject.GetIndex() + _Graph.VertexCount, dataObject);
    }

    public void AddRelationToGraph(ObjectRelation relation)
    {


        if (relation == null)
        {
            throw new ArgumentNullException(nameof(relation), "Relation cannot be null");
        }
        // check if source and target objects are both information objects
        if (relation.SourceObject.ObjectType == ObjectType.InformationObject &&
            relation.TargetObject.ObjectType == ObjectType.InformationObject)
        {
            return; // do not add relation between two information objects
        }

        edgeObjects.Add(relation);
        _Graph.AddEdge(relation);
        _edgeDictionary.Add(relation.GetIndex() + _Graph.EdgeCount, relation);
        // update vertex weights based on relation type
        relation.SourceObject.UpdateWeight(ObjectHelper.ConvertRelationTypeToWeight(relation.RelationType));
        relation.TargetObject.UpdateWeight(ObjectHelper.ConvertRelationTypeToWeight(relation.RelationType));

    }

    public void AddConnectionToNodes()
    {

        if (nodeObjects.Count == 0)
        {
            throw new InvalidOperationException("No data objects available to connect");
        }
        if (edgeObjects.Count == 0)
        {
            throw new InvalidOperationException("No relations available to connect nodes");
        }
        foreach (var relation in edgeObjects)
        {
            var sourceObject = GetNodeObjectByName(relation.SourceObject.Name);
            var targetObject = GetNodeObjectByName(relation.TargetObject.Name);

            if (sourceObject == null || targetObject == null)
            {
                throw new InvalidOperationException("Source or target object not found for relation");
            }

            sourceObject.TargetObjects.Add(targetObject);
            targetObject.OriginObjects.Add(sourceObject);

        }

    }


    public DataObject GetNodeObjectByName(string name)
    {
        var nodeWanted = default(DataObject);
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name), "Name cannot be null or empty");
        }
        if (nodeObjects.Count == 0)
        {
            throw new InvalidOperationException("No data objects available");
        }


        foreach (var dataObject in nodeObjects)
        {
            if (dataObject.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            {
                nodeWanted = (DataObject?)dataObject;
            }
        }

        return nodeWanted;

    }

    public void RemoveEdgeFromGraph(string sourceNode, string targetNode)
    {
        if (string.IsNullOrEmpty(sourceNode) || string.IsNullOrEmpty(targetNode))
        {
            throw new ArgumentNullException("Source or target node name cannot be null or empty");
        }

        var relationToRemove = edgeObjects.FirstOrDefault(r => r.SourceObject.Name.Equals(sourceNode, StringComparison.OrdinalIgnoreCase) &&
                                                                r.TargetObject.Name.Equals(targetNode, StringComparison.OrdinalIgnoreCase));


        if (relationToRemove == null)
        {
            throw new InvalidOperationException("Relation not found in the graph");
        }

        edgeObjects.Remove(relationToRemove);

        var sourceObject = GetNodeObjectByName(sourceNode);
        var targetObject = GetNodeObjectByName(targetNode);

        sourceObject?.TargetObjects.Remove(targetObject);
        targetObject?.OriginObjects.Remove(sourceObject);
    }

    public void RemoveNodeFromGraph(string dataObjectName)
    {
        var dataObject = GetNodeObjectByName(dataObjectName);
        if (dataObject == null)
        {
            throw new InvalidOperationException("Data object not found in the graph");
        }

        if (dataObject.TargetObjects.Count == 0 && dataObject.OriginObjects.Count == 0)
        {
            nodeObjects.Remove(dataObject);
            return;
        }

        foreach (var originNode in dataObject.OriginObjects.ToList())
        {
            this.RemoveEdgeFromGraph(originNode.Name, dataObject.Name);
        }

        foreach (var targetNode in dataObject.TargetObjects.ToList())
        {
            this.RemoveEdgeFromGraph(dataObject.Name, targetNode.Name);
        }

        this.nodeObjects.Remove(dataObject);

    }

    public void ReadList()
    {
        Console.WriteLine("Registered Data Objects are :");
        foreach (var dataObject in nodeObjects)
        {
            Console.WriteLine(dataObject.Name);
        }
        Console.WriteLine("Relations are :");
        foreach (var relation in edgeObjects)
        {
            Console.WriteLine($"{relation.SourceObject} - {relation.TargetObject} ({relation.RelationType})");
        }
    }

    public void ReadNodeConnection()
    {
        if (nodeObjects.Count == 0)
        {
            throw new InvalidOperationException("No data objects available to read connections");
        }
        foreach (var dataObject in nodeObjects)
        {
            Console.WriteLine($"Data Object: {dataObject.Name}");
            Console.WriteLine(dataObject.ReadOriginObjects());
            Console.WriteLine(dataObject.ReadTargetObjects());
        }
    }

    public void ReadGraph()
    {
        Console.WriteLine("Graph contains the following nodes and edges:");
        foreach (var vertex in _Graph.Vertices)
        {
            Console.WriteLine($"Node: {vertex.Name}");
        }
        foreach (var edge in _Graph.Edges)
        {
            Console.WriteLine($"Edge from {edge.SourceObject.Name} to {edge.TargetObject.Name} with relation type {edge.RelationType}");
        }

        Console.WriteLine("Graphviz representation:");

        Console.WriteLine(GraphService.GenerateGraphToDOT(_Graph));

    }

    public AdjacencyGraph<DataObject, IObjectRelation> GetGraph()
    {
        return _Graph;
    }


}
