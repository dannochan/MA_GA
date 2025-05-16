using System;
using System.Collections.Generic;

namespace MA_GA.Models;

public interface IDataObject
{
    /// <summary>
    /// Gets the unique identifier of the information object.
    /// </summary>
    Guid Id { get; }

    /// <summary>
    /// get and set the type of the information object.
    /// </summary>
    ObjectType? ObjectType { get; set; }

    /// <summary>
    /// Gets the name of the information object.
    /// </summary>
    string Name { get; set; }

    /// <summary>
    /// Gets the short name of the information object.
    /// </summary>
    string ShortName { get; set; }

    /// <summary>
    /// if the information object is an external component.
    /// </summary>
    bool? isExternalComponent { get; set; }


    List<IObjectRelation> Relations { get; set; }

}
