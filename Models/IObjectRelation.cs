using System;

namespace MA_GA.Models;

public interface IObjectRelation
{
    // Type of the relation
    RelationType RelationType { get; set; }


    // target object
    IDataObject TargetObject { get; set; }


}
