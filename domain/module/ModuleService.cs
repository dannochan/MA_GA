using System;
using MA_GA.domain.GeneticAlgorithm.encoding;
using MA_GA.Models;

namespace MA_GA.domain.module;

public class ModuleService
{
    public static Module MergeModules(Module module1, Module module2)
    {
        return new Module() { };
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
