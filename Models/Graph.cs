using System;

namespace MA_GA.Models;

public class Graph
{

    List<IDataObject> nodeObjects;
    List<IObjectRelation> relations;

    public Graph()
    {
        nodeObjects = new List<IDataObject>();
        relations = new List<IObjectRelation>();
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

        relations.Add(relation);
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
        if (string.IsNullOrEmpty(name))
        {
            throw new ArgumentNullException(nameof(name), "Name cannot be null or empty");
        }
        if (nodeObjects.Count == 0)
        {
            throw new InvalidOperationException("No data objects available");
        }
        return nodeObjects.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }

    public void readList()
    {
        Console.WriteLine("Registered Data Objects are :");
        foreach (var dataObject in nodeObjects)
        {
            Console.WriteLine(dataObject.Name);
        }
        Console.WriteLine("Relations are :");
        foreach (var relation in relations)
        {
            Console.WriteLine($"{relation.SourceObject} - {relation.TargetObject} ({relation.RelationType})");
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
