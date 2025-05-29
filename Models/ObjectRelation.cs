using System;

namespace MA_GA.Models;

public class ObjectRelation : IObjectRelation
{
    public RelationType RelationType { get; set; }
    public string SourceObject { get; set; }
    public string TargetObject { get; set; }

    public ObjectRelation(RelationType relationType, string sourceObject, string targetObject)
    {
        RelationType = relationType;
        SourceObject = sourceObject;
        TargetObject = targetObject;
    }

}
