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
            foreach (var item in rawObject.functions)
            {
                dataObjectCenter.AddNodeToGraph(new DataObject(
                    item.name,
                    ObjectType.FunctionObject,
                    item.nameShort,
                    item.externalComponent
                ));
            }
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
