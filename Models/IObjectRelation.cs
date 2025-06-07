using System;
using QuikGraph;

namespace MA_GA.Models;

/// <summary>
/// This interface defines the existing relations between objects in the graph.
/// It includes properties for the type of relation, the source object, and the target object.
/// </summary>

public interface IObjectRelation : IEdge<DataObject>
{
    // Type of the relation
    RelationType RelationType { get; set; }

    // Source object
    IDataObject SourceObject { get; set; }


    // target object

    IDataObject TargetObject { get; set; }


}
