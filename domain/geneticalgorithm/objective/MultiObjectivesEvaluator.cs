using System;
using GeneticSharp;
using MA_GA.models.enums;
using MA_GA.Models;

namespace MA_GA.domain.geneticalgorithm.objective;

public class MultiObjectivesEvaluator
{
    private readonly List<Objective> _objectives;
    private readonly Graph _graph;


    public MultiObjectivesEvaluator(Graph graph, List<Objective> objectives) : this(objectives)
    {
        this._graph = graph ?? throw new ArgumentNullException(nameof(graph));
    }

    public MultiObjectivesEvaluator(List<Objective> objectives)
    {
        _objectives = objectives;
    }

    // TODO: CHECK IF OBJECTIVES WEIGHT should be considreed in the evaluation
    public double[] EvaluateAll(IChromosome chromosome)
    {
        var _sumObjectiveWeights = _objectives.Sum(o => o.GetWeight());
        return _objectives.Select(obj =>
        {
            var weight = obj.GetWeight() / _sumObjectiveWeights;
            var objectiveValue = obj.Evaluate(chromosome);

            var weightedValue = weight * objectiveValue;
            // TODO: how to deal with type of objectives, see paper of ali p49

            if (obj.GetOptimizationType() == OptimizationType.Minimum)
            {
                return weightedValue *= -1;
            }
     ;

            return weightedValue;
        }).ToArray();
    }


    public int ObjectiveCount => _objectives.Count;

}
