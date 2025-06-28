using System;
using GeneticSharp;
using MA_GA.domain.geneticalgorithm.parameter;
using MA_GA.models.optimizationresult;
using MA_GA.Models;

namespace MA_GA.domain.geneticalgorithm.engine;

public class MainGeneticAlgorithmEngine : GeneticAlgorithmEngine
{

    private static readonly float RANDOM_GENERTATED_SEED = 12345f;


    public GeneticAlgorithmExecutionResult run(Graph graph, GeneticAlgorithmParameter geneticAlgorithmParameter)
    {

        BasicRandomization.ResetSeed((int)RANDOM_GENERTATED_SEED);
        RandomizationProvider.Current = new BasicRandomization();

        return ModularisewithMultiObjectiveFitnessFunction(geneticAlgorithmParameter, graph);


    }

    private GeneticAlgorithmExecutionResult ModularisewithMultiObjectiveFitnessFunction(GeneticAlgorithmParameter geneticAlgorithmParameter, Graph graph)
    {

        // TODO: ADD objective when available

        var fitnessFunction = new MultiObjectiveFitnessFunction();

        // build genetic algorithm engine
        var geneticAlgorithmEngine = new GeneticAlgorithmEngineBuilder.Builder()
            .Graph(graph)
            .GeneticAlgorithmParameter(geneticAlgorithmParameter)
            .Fitness(new MultiObjectiveFitnessFunction())
            .CreatingEngineForMultiObjectiveProblem();


        // run the genetic algorithm
        geneticAlgorithmEngine.Start();
        Console.WriteLine($"Population Size: {geneticAlgorithmEngine.Population.GenerationsNumber}");
        var chromosome = geneticAlgorithmEngine.BestChromosome.ToString();
        Console.WriteLine($"Best Chromosome: {chromosome}");

        // run the genetic algorithm
        geneticAlgorithmEngine.Start();

        return new GeneticAlgorithmExecutionResult();
    }
}

internal class MultiObjectiveFitnessFunction : IFitness
{
    public MultiObjectiveFitnessFunction()
    {
    }

    public double Evaluate(IChromosome chromosome)
    {
        chromosome.Fitness = 0.0; // Placeholder for actual fitness evaluation logic
        return chromosome.Fitness.Value;
    }
}