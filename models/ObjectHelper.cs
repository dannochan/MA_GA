using System;
using Microsoft.Extensions.Logging;

namespace MA_GA.Models;


// <summary>
// Helper class for object operations
// </summary>
public static class ObjectHelper
{

    public static void MapDataObjects(GraphObject rawObject, Graph dataObjectCenter, ILogger logger)
    {
        int elementIndex = 0;
        // map information objects
        if (rawObject.informationObjects != null)
        {
            foreach (var item in rawObject.informationObjects)
            {
                dataObjectCenter.AddNodeToGraph(new DataObject(
                    item.name,
                    ObjectType.InformationObject,
                    item.nameShort,
                    item.externalComponent
                ));
            }

        }
        else
        {
            logger.LogError("InformationObjects is null");
        }

        // map function objects

        if (rawObject.functions != null)
        {
            var index = 0;
            foreach (var item in rawObject.functions)
            {
                dataObjectCenter.AddNodeToGraph(new DataObject(
                    item.name,
                    ObjectType.FunctionObject,
                    item.nameShort,
                    item.externalComponent,
                    index++ // Assigning an index to each function object
                ));
            }

            elementIndex += index; // Update the element index after adding function objects
        }
        else
        {
            logger.LogError("FunctionObjects is null");
        }

        // map relation objects
        if (rawObject.relations != null)
        {
            foreach (var item in rawObject.relations)
            {
                dataObjectCenter.AddRelationToGraph(new ObjectRelation(
                    elementIndex++,
                   convertIntToRelationTyp(item.type),
                    dataObjectCenter.GetNodeObjectByName(item.from),
                    dataObjectCenter.GetNodeObjectByName(item.to)

                ));


            }
        }
        else
        {
            logger.LogError("RelationObjects is null");
        }


    }

    /// <summary>
    /// Converts a RelationType to a weight value.
    /// </summary>
    /// <param name="relationType">The type of relation to convert.</param>
    /// <returns>An integer representing the weight of the relation type. -1 indicates relation between information object</returns>
    public static int ConvertRelationTypeToWeight(RelationType relationType)
    {
        return relationType switch
        {
            RelationType.Konjunktion => 2,
            RelationType.Disjunktion => 1,
            RelationType.ExclusiveDisjunktion => 3,
            RelationType.Create => 3,
            RelationType.Read => 1,
            RelationType.Update => 3,
            RelationType.RelatedTo => 0,
            RelationType.PartOf => 0,
            RelationType.IsA => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(relationType), "Invalid relation type")
        };
    }

    private static RelationType convertIntToRelationTyp(int type)
    {
        return type switch
        {
            0 => RelationType.Konjunktion,
            1 => RelationType.Disjunktion,
            2 => RelationType.ExclusiveDisjunktion,
            3 => RelationType.Create,
            4 => RelationType.Read,
            5 => RelationType.Update,
            7 => RelationType.RelatedTo,
            8 => RelationType.PartOf,
            9 => RelationType.IsA,
            _ => throw new ArgumentOutOfRangeException(nameof(type), "Invalid relation type")
        };
    }


}
