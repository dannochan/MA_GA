using System;
using GeneticSharp;
using MA_GA.Models;
using MA_GA.domain.geneticalgorithm.parameter;
using MA_GA.domain.geneticalgorithm.encoding;


namespace MA_GA.domain.geneticalgorithm.engine;

public class GeneticAlgorithmEngineBuilder
{
    public class Builder
    {
        private Graph _graph;
        private GeneticAlgorithmParameter _geneticAlgorithmParameter;

        private IFitness _fitness;

        public Builder Graph(Graph graph)
        {
            _graph = graph;
            return this;
        }

        public Builder GeneticAlgorithmParameter(GeneticAlgorithmParameter geneticAlgorithmParameter)
        {
            _geneticAlgorithmParameter = geneticAlgorithmParameter;
            return this;
        }

        public Builder Fitness(IFitness fitness)
        {
            _fitness = fitness;
            return this;
        }

        public GeneticAlgorithm CreatingEngineForMultiObjectiveProblem()
        {
            var geneticAlgorithmParameter = _geneticAlgorithmParameter;
            var population = CreatePopulation(_graph, geneticAlgorithmParameter);
            var selector = MultiObjectiveSelector();
            var crossover = CreateCrossover();
            var mutation = CreateMutatorn();

            return new GeneticAlgorithm(
                population,
                _fitness,
                selector,
                crossover,
                mutation)
            {
                Termination = new GenerationNumberTermination(geneticAlgorithmParameter.MaxGenerations),
                CrossoverProbability = geneticAlgorithmParameter.CrossoverRate,
                MutationProbability = geneticAlgorithmParameter.MutationRate
            };

        }

        // TODO: Termination need to be corrected for correct building 

        public GeneticAlgorithm CreatingEngineForWeightedSumProblem()
        {
            var geneticAlgorithmParameter = _geneticAlgorithmParameter;
            var population = CreatePopulation(_graph, geneticAlgorithmParameter);
            var selector = SingleObjectiveSelector();
            var crossover = CreateCrossover();
            var mutation = CreateMutatorn();

            return new GeneticAlgorithm(
                population,
                _fitness,
                selector,
                crossover,
                mutation)
            {
                Termination = new GenerationNumberTermination(geneticAlgorithmParameter.MaxGenerations),
                CrossoverProbability = geneticAlgorithmParameter.CrossoverRate,
                MutationProbability = geneticAlgorithmParameter.MutationRate

            };

        }

        private IMutation CreateMutatorn()
        {
            var geneticAlgorithmParameter = _geneticAlgorithmParameter;
            switch (geneticAlgorithmParameter.MutationType)
            {
                default: return new UniformMutation();
            }
        }

        private ICrossover CreateCrossover()
        {
            var geneticAlgorithmParameter = _geneticAlgorithmParameter;
            switch (geneticAlgorithmParameter.CrossoverType)
            {
                default: return new UniformCrossover();
            }
        }

        private ISelection MultiObjectiveSelector()
        {
            var geneticAlgorithmParameter = _geneticAlgorithmParameter;
            switch (geneticAlgorithmParameter.OffspringSelection)
            {
                default:
                    return new TournamentSelection(geneticAlgorithmParameter.TournamentSize);
            }
        }

        private ISelection SingleObjectiveSelector()
        {

            var geneticAlgorithmParameter = _geneticAlgorithmParameter;
            switch (geneticAlgorithmParameter.OffspringSelection)
            {
                default:
                    return new RouletteWheelSelection();
            }

        }


        // Create a population with the given graph and genetic algorithm parameters
        private IPopulation CreatePopulation(Graph graph, GeneticAlgorithmParameter geneticAlgorithmParameter, bool isGreedyAlgoResult=false )
        {
            IChromosome chromosome = isGreedyAlgoResult ? LinearLinkageEncodingInitialiser.InitializeLinearLinkageEncodingWithGreedyAlgorithm(graph) : Genotypeinitializer.GenerateGenotypeWithModulesForEachConnectedComponet(graph);
            return new Population(geneticAlgorithmParameter.PopulationSize, geneticAlgorithmParameter.PopulationSize, chromosome);
        }


    }

}
