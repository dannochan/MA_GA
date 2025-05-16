using System;

namespace MA_GA.Models;

public class DataObject : IDataObject
{
    public Guid Id { get; set; }
    public ObjectType? ObjectType { get; set; }

    public string Name { get; set; }

    public string ShortName { get; set; }

    public bool? isExternalComponent { get; set; }

    public List<IObjectRelation> Relations { get; set; } = [];



    public DataObject(string name, ObjectType objectType, string shortName, bool? isExternalComponent)
    {
        Id = Guid.NewGuid();
        this.ObjectType = objectType;
        this.Name = name;
        this.ShortName = shortName;
        this.isExternalComponent = isExternalComponent;
    }


    public string getName()
    {
        return Name;
    }
    public string getShortName()
    {
        return ShortName;
    }

    public void addRelation(IObjectRelation relation)
    {
        Relations.Add(relation);
    }
    public void removeRelation(IObjectRelation relation)
    {
        Relations.Remove(relation);
    }

    public void getAllRelations()
    {
        Console.WriteLine("Relations of " + Name + ":");
        foreach (var relation in Relations)
        {
            Console.WriteLine(relation.TargetObject.Name + " - " + relation.RelationType);
        }
    }


}
