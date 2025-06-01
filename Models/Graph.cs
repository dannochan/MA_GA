using System;

namespace MA_GA.Models;

public class Graph
{

    List<IDataObject> nodeObjects;
    List<IObjectRelation> edgeObjects;

    public Graph()
    {
        nodeObjects = new List<IDataObject>();
        edgeObjects = new List<IObjectRelation>();
    }

    public bool IsEmpty()
    {
        return nodeObjects.Count == 0;
    }

    public void AddNodeToGraph(IDataObject dataObject)
    {
        nodeObjects.Add(dataObject);
    }

    public void AddRelationToGraph(IObjectRelation relation)
    {


        if (relation == null)
        {
            throw new ArgumentNullException(nameof(relation), "Relation cannot be null");
        }

        edgeObjects.Add(relation);

    }

    public void addConnectionToNodes()
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
            var sourceObject = GetNodeObjectByName(relation.SourceObject);
            var targetObject = GetNodeObjectByName(relation.TargetObject);

            if (sourceObject == null || targetObject == null)
            {
                throw new InvalidOperationException("Source or target object not found for relation");
            }

            sourceObject.TargetObjects.Add(targetObject);
            targetObject.OriginObjects.Add(sourceObject);

        }

    }


    public IDataObject GetNodeObjectById(Guid id)
    {
        if (id == null)
        {
            throw new ArgumentNullException(nameof(id), "ID cannot be null");
        }
        if (nodeObjects.Count == 0)
        {
            throw new InvalidOperationException("No data objects available");
        }
        return nodeObjects.FirstOrDefault(d => d.Id == id);
    }

    public IDataObject GetNodeObjectByName(string name)
    {
        var nodeWanted = default(IDataObject);
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
                nodeWanted = dataObject;
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

        var relationToRemove = edgeObjects.FirstOrDefault(r => r.SourceObject.Equals(sourceNode, StringComparison.OrdinalIgnoreCase) &&
                                                                r.TargetObject.Equals(targetNode, StringComparison.OrdinalIgnoreCase));


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

    public void readList()
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

    public void addRelation(Guid id, IObjectRelation relation)
    {
        var sourceObject = GetNodeObjectById(id);
        var targetObject = GetNodeObjectByName(relation.TargetObject);
        if (sourceObject != null && targetObject != null)
        {
            // check if the relation already exists

            if (sourceObject.Relations.Any(r => r == targetObject))
            {
                throw new InvalidOperationException("Relation already exists in source object");

            }

            if (targetObject.Relations.Any(r => r == sourceObject))
            {
                throw new InvalidOperationException("Relation already exists in target object");

            }

            sourceObject.Relations.Add(relation);
            targetObject.Relations.Add(relation);
            Console.WriteLine($"Relation added between {sourceObject.Name} and {targetObject.Name}");
        }
        else
        {
            throw new InvalidOperationException("Data object not found");
        }
    }

}
