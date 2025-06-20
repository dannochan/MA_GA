using System;
using System.Collections;
using MA_GA.domain.GeneticAlgorithm.encoding;
using MA_GA.Models;

namespace MA_GA.domain.module;
/// <summary>
/// ModuleService provides methods for manipulating modules, including merging, splitting, and dividing modules.
/// </summary>
public class ModuleService
{
    public static Module MergeModules(Module module1, Module module2)
    {
        // ger indices of both modules
        var indices1 = module1.GetIndices();
        var indices2 = module2.GetIndices();

        // create a new module to hold the merged indices
        Module mergedModule = new Module();

        int index1 = 0, index2 = 0;

        bool isNewModuleBuilt = false;

        while (!isNewModuleBuilt)
        {
            if (index1 < indices1.Count && index2 < indices2.Count)
            {
                // compare the current indices of both modules
                if ((int)indices1[index1] < (int)indices2[index2])
                {
                    // add index from module1 to merged module
                    mergedModule.AddIndex((int)indices1[index1]);
                    index1++;
                }
                else
                {
                    mergedModule.AddIndex((int)indices2[index2]);
                    index2++;
                }
            }
            else
            {
                // if one of the indices is exhausted, add the remaining indices from the other module
                while (index1 < indices1.Count)
                {
                    mergedModule.AddIndex((int)indices1[index1]);
                    index1++;
                }
                while (index2 < indices2.Count)
                {
                    mergedModule.AddIndex((int)indices2[index2]);
                    index2++;
                }
                isNewModuleBuilt = true;
            }
        }
        return mergedModule;

    }

    public List<Module> SplitNonIncidentModule(Module module, LinearLinkageEncoding encoding)
    {
        return new List<Module>();
    }

    public static HashSet<Module> DivideModuleRandomly(Module module, Graph graph)
    {
        if (module == null || graph == null)
        {
            throw new ArgumentNullException("Module or graph cannot be null.");
        }

        if (module.GetIndices().Count == 0)
        {
            throw new ArgumentException("Module must contain at least one index.");
        }

        var splitModules = new HashSet<Module>();

        var indices = new ArrayList(module.GetIndices());



        switch (indices.Count)
        {
            case 1:
                splitModules.Add(module);
                return splitModules;
            case 2:
                var newModule1 = new Module();
                newModule1.AddIndex((int)indices[0]);
                var newModule2 = new Module();
                newModule2.AddIndex((int)indices[1]);
                splitModules.Add(newModule1);
                splitModules.Add(newModule2);
                return splitModules;

            default:
                var halfSizeOfModule = indices.Count / 2;
                var remainingIndices = new List<object>(module.GetIndices());
                while (remainingIndices.Count > 0)
                {
                    var randomStartElementIndex = Random.Shared.Next(remainingIndices.Count);
                    var startElement = graph.GetModularisableElementByIndex((int)remainingIndices[randomStartElementIndex]);
                    var indicesOfSplitteModule = CreateIndicesOfSubGraphRandomly(startElement, graph, halfSizeOfModule, remainingIndices);

                    Module newModule = new Module();
                    newModule.AddIndices(indicesOfSplitteModule);
                    splitModules.Add(module);
                    remainingIndices.Clear();

                }
                return splitModules;
        }

    }

    public static int DetermineSplittedModuleSize(List<object> indices)
    {
        return indices.Count / 2;
    }

    public static HashSet<object> CreateIndicesOfSubGraphRandomly(ModularisableElement modularisableElement, Graph graph, int subgraphSize, List<object> indicesOfModule)
    {
        var selectedIndices = new HashSet<object>();
        var visitedModularisableElement = new HashSet<ModularisableElement>();

        var queue = new Stack<ModularisableElement>();
        queue.Push(modularisableElement);

        while (queue.Count > 0 && selectedIndices.Count < subgraphSize)
        {
            var currentElement = queue.Pop();
            selectedIndices.Add(currentElement.GetIndex());

            if (currentElement is DataObject vertex)
            {
                var currentVertex = (DataObject)currentElement;
                var edgesOfCurrentVertex = graph.GetGraph().Edges
                    .Where(e => (e.SourceObject.Equals(currentVertex) || e.TargetObject.Equals(currentVertex)) &&
                                selectedIndices.Contains(e.GetIndex()) &&
                                !visitedModularisableElement.Contains((ObjectRelation)e)).OrderBy(e => e.GetIndex());

                edgesOfCurrentVertex.ToList().ForEach(edge =>
                {
                    queue.Push((ObjectRelation)edge);
                });


            }
            else if (currentElement is ObjectRelation relation)
            {
                var currentEdge = (ObjectRelation)currentElement;
                var sourceVertex = currentEdge.SourceObject;
                var targetVertex = currentEdge.TargetObject;

                if (!visitedModularisableElement.Contains(sourceVertex) && indicesOfModule.Contains(sourceVertex.GetIndex())
                && indicesOfModule.Contains(currentEdge.GetIndex()))
                {
                    queue.Push(sourceVertex);
                }

                if (!visitedModularisableElement.Contains(targetVertex) && indicesOfModule.Contains(targetVertex.GetIndex())
                && indicesOfModule.Contains(currentEdge.GetIndex()))
                {
                    queue.Push(targetVertex);
                }

            }
            visitedModularisableElement.Add(currentElement);

        }


        return selectedIndices;
    }
}
