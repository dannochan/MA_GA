using System;

namespace MA_GA.Models;

public class ObjectRelation : ModularisableElement, IObjectRelation
{
    private int EdgeNumber { get; set; }
    public RelationType RelationType { get; set; }

    public IDataObject SourceObject { get; set; }

    public IDataObject TargetObject { get; set; }

    public IDataObject Source { get; set; }

    public IDataObject Target { get; set; }

    public ObjectRelation(RelationType relationType,
    IDataObject sourceObject,
    IDataObject targetObject)
    {
        RelationType = relationType;
        SourceObject = sourceObject;
        TargetObject = targetObject;
        Source = sourceObject;
        Target = targetObject;
    }

    public override int GetIndex()
    {
        return EdgeNumber;
    }

    public override bool Equals(object? obj)
    {
        return obj is ObjectRelation relation &&
               RelationType == relation.RelationType &&
               EqualityComparer<IDataObject>.Default.Equals(SourceObject, relation.SourceObject) &&
               EqualityComparer<IDataObject>.Default.Equals(TargetObject, relation.TargetObject);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(RelationType, SourceObject, TargetObject);
    }
}
