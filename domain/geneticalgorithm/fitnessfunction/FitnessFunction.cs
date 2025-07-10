using System;
using GeneticSharp;
using MA_GA.domain.geneticalgorithm.objective;
using MA_GA.Models;

namespace MA_GA.domain.geneticalgorithm.fitnessfunction;

public class FitnessFunction : IFitness
{
    private readonly List<Objective> _objectives;
    private readonly Graph _graph;

    private readonly double _sumObjectiveWeights;

    public FitnessFunction(List<Objective> objectives, Graph graph)
    {
        _objectives = objectives ?? throw new ArgumentNullException(nameof(objectives));
        _graph = graph ?? throw new ArgumentNullException(nameof(graph));
        _sumObjectiveWeights = objectives.Sum(o => o.GetWeight());
    }

    public double Evaluate(IChromosome chromosome)
    {
        throw new NotImplementedException();
    }
}
