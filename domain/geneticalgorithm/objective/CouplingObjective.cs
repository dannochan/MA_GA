using System;
using MA_GA.domain.module;
using MA_GA.models.enums;
using MA_GA.Models;
using QuikGraph;

namespace MA_GA.domain.geneticalgorithm.objective;

public class CouplingObjective : Objective
{
    public CouplingObjective(Graph graph, double weight)
    {
        SetGraph(graph ?? throw new ArgumentNullException(nameof(graph)));
        SetWeight(weight);
    }
    public override double CalculateValue(List<Module> modules)
    {
        var visitedEdges = new HashSet<IObjectRelation>();
        return modules.Where(module => !ModuleInformationService.IsIsolated(module, graph)).Sum(
         module =>
         {
             return ModuleInformationService.GetBoundaryEdgesOfModule(module, graph).Sum(edge =>
             {
                 if (visitedEdges.Contains(edge))
                 {
                     return 0.0; // Skip already visited edges
                 }
                 visitedEdges.Add(edge);
                 return edge.Weight;
             });
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
