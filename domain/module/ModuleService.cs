using System;
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
        return new HashSet<Module>();
    }

    public static int DetermineSplittedModuleSize(List<object> indices)
    {
        return indices.Count / 2;
    }

    public static HashSet<object> CreateIndicesOfSubGraphRandomly(ModularisableElement modularisableElement, Graph graph, int subgraphSize, List<object> indicesOfModule)
    {
        return new HashSet<object>();
    }
}
