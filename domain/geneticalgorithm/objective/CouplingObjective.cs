using System;
using MA_GA.domain.module;
using MA_GA.models.enums;
using QuikGraph;

namespace MA_GA.domain.geneticalgorithm.objective;

public class CouplingObjective : Objective
{
    public override double CalculateValue(List<Module> modules)
    {
        return modules.Where(module => !ModuleInformationService.IsIsolated(module, graph)).Sum(
         module =>
         {
             return ModuleInformationService.GetBoundaryEdgesOfModule(module, graph).Sum(edge => edge.Weight);
         }
       );
    }
    public override string GetObjectiveName()
    {
        return GetObjectiveType().ToString();
    }

    public override ObjectiveType GetObjectiveType()
    {
        return ObjectiveType.MINIMISE_COUPLING;
    }

    public override OptimizationType GetOptimizationType()
    {
        return OptimizationType.Minimum;
    }


}
