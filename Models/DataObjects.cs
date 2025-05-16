using System;

namespace MA_GA.Models;

public class DataObjects
{

    List<IDataObject> dataObjects = new List<IDataObject>();

    public DataObjects()
    {
        dataObjects = new List<IDataObject>();
    }

    public bool IsEmpty()
    {
        return dataObjects.Count == 0;
    }

    public void AddDataObject(IDataObject dataObject)
    {
        dataObjects.Add(dataObject);
    }

    public IDataObject GetDataObject(Guid id)
    {
        if (id == null)
        {
            throw new ArgumentNullException(nameof(id), "ID cannot be null");
        }
        if (dataObjects.Count == 0)
        {
            throw new InvalidOperationException("No data objects available");
        }
        return dataObjects.FirstOrDefault(d => d.Id == id);
    }

    public void readList()
    {
        Console.WriteLine("Registered Data Objects are :");
        foreach (var dataObject in dataObjects)
        {
            Console.WriteLine(dataObject.Name);
        }
    }

    public void addRelation(Guid id, IObjectRelation relation)
    {
        var sourceObject = GetDataObject(id);
        var targetObject = GetDataObject(relation.TargetObject.Id);
        if (sourceObject != null && targetObject != null)
        {
            // check if the relation already exists

            if (sourceObject.Relations.Any(r => r.TargetObject.Id == targetObject.Id))
            {
                throw new InvalidOperationException("Relation already exists in source object");

            }

            if (targetObject.Relations.Any(r => r.TargetObject.Id == targetObject.Id))
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
