using System;

namespace MA_GA.Models;

public interface IObjectRelation
{
    // Type of the relation
    RelationType RelationType { get; set; }

    // Source object
    string SourceObject { get; set; }


    // target object
    string TargetObject { get; set; }


}
