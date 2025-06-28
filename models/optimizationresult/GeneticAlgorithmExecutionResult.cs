using System;

namespace MA_GA.models.optimizationresult;

public class GeneticAlgorithmExecutionResult
{

    private HashSet<ParetoOptimalSolution> ParetoOptimalSolutions { get; set; }
    private GeneticAlgorithmResults GeneticAlgorithmResults { get; set; }

}
