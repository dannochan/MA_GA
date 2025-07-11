using System;
using GeneticSharp;
using MA_GA.Models;

namespace MA_GA.domain.geneticalgorithm.objective;

public class MultiObjectivesEvaluator
{
    private readonly List<Objective> _objectives;
    private readonly Graph _graph;


    public MultiObjectivesEvaluator(Graph graph, List<Objective> objectives): this(objectives)
    {
        this._graph = graph ?? throw new ArgumentNullException(nameof(graph));
    }

    public MultiObjectivesEvaluator(List<Objective> objectives)
    {
        _objectives = objectives;
    }

    // TODO: CHECK IF OBJECTIVES WEIGHT should be considreed in the evaluation
    public double[] EvaluateAll(IChromosome chromosome)
        => _objectives.Select(obj => obj.Evaluate(chromosome)).ToArray();

    public int ObjectiveCount => _objectives.Count;

}
