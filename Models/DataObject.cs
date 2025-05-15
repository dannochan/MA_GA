using System;

namespace MA_GA.Models;

public class InformationObject : IDataObject
{
    public Guid Id { get; set; }
    public ObjectType ObjectType { get; set; } = ObjectType.InformationObject;

    public required string Name { get; set; }

    public string ShortName { get; set; }

    public bool isExternalComponent { get; set; }

    public List<IObjectRelation> Relations { get; set; } = [];



    public InformationObject(string name, ObjectType objectType, string shortName, bool isExternalComponent)
    {
        Id = Guid.NewGuid();
        ObjectType = objectType;
        Name = name;
        ShortName = shortName;
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
