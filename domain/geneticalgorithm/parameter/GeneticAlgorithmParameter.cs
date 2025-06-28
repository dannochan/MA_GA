using System;

namespace MA_GA.domain.geneticalgorithm.parameter;

public class GeneticAlgorithmParameter
{
    private string ChromosomeEncoding { get; set; }
    private string OffspringSelection { get; set; }
    private string SurvivalSelection { get; set; }
    private string CrossoverType { get; set; }
    private string MutationType { get; set; }
    private int PopulationSize { get; set; }
    private double CrossoverRate { get; set; }
    private double MutationRate { get; set; }
    private int MaxGenerations { get; set; }
    private int TournamentSize { get; set; }
    private int ElitismCount { get; set; }

    private double ConvergedGeneRate { get; set; }

    private double ConvergenceRate { get; set; }
    private int CountGeneration { get; set; }
    private int MinimumParetoSetSize { get; set; }
    private int MaximumParetoSetSize { get; set; }

    public GeneticAlgorithmParameter(
        string chromosomeEncoding,
        string offspringSelection,
        string survivalSelection,
        string crossoverType,
        string mutationType,
        int populationSize,
        double crossoverRate,
        double mutationRate,
        int maxGenerations,
        int tournamentSize,
        int elitismCount,
        double convergedGeneRate,
        double convergenceRate,
        int countGeneration,
        int minimumParetoSetSize,
        int maximumParetoSetSize)
    {
        ChromosomeEncoding = chromosomeEncoding;
        OffspringSelection = offspringSelection;
        SurvivalSelection = survivalSelection;
        CrossoverType = crossoverType;
        MutationType = mutationType;
        PopulationSize = populationSize;
        CrossoverRate = crossoverRate;
        MutationRate = mutationRate;
        MaxGenerations = maxGenerations;
        TournamentSize = tournamentSize;
        ElitismCount = elitismCount;
        ConvergedGeneRate = convergedGeneRate;
        ConvergenceRate = convergenceRate;
        CountGeneration = countGeneration;
        MinimumParetoSetSize = minimumParetoSetSize;
        MaximumParetoSetSize = maximumParetoSetSize;
    }

    public override string ToString()
    {
        return $"GeneticAlgorithmParameter: " +
               $"ChromosomeEncoding={ChromosomeEncoding}, " +
               $"OffspringSelection={OffspringSelection}, " +
               $"SurvivalSelection={SurvivalSelection}, " +
               $"CrossoverType={CrossoverType}, " +
               $"MutationType={MutationType}, " +
               $"PopulationSize={PopulationSize}, " +
               $"CrossoverRate={CrossoverRate}, " +
               $"MutationRate={MutationRate}, " +
               $"MaxGenerations={MaxGenerations}, " +
               $"TournamentSize={TournamentSize}, " +
               $"ElitismCount={ElitismCount}, " +
               $"ConvergedGeneRate={ConvergedGeneRate}, " +
               $"ConvergenceRate={ConvergenceRate}, " +
               $"CountGeneration={CountGeneration}, " +
               $"MinimumParetoSetSize={MinimumParetoSetSize}, " +
               $"MaximumParetoSetSize={MaximumParetoSetSize}";
    }

    
    

}
