using System;
using GeneticSharp;
using MA_GA.domain.geneticalgorithm.encoding;
using MA_GA.domain.geneticalgorithm.parameter;
using MA_GA.Models;

namespace MA_GA.domain.geneticalgorithm.mutation;

public class GraftMutator : MutationBase
{

    private readonly Graph baseGraph;
    private float divideModuleprobability;
    private float combinedModuleprobability;
    private float movegeneToDifferentModuleProbability;



    public GraftMutator(MutationWeight mutationWeight, Graph graph)
    {

        baseGraph = graph;
        DetermineMutationWeights(mutationWeight);

    }

    private void DetermineMutationWeights(MutationWeight mutationWeight)
    {
        float sumPossibilities = mutationWeight.SplitModulesWeight + mutationWeight.CombineModulesWeight + mutationWeight.MoveGeneToDifferentModuleWeight;
        divideModuleprobability = mutationWeight.SplitModulesWeight / sumPossibilities;
        combinedModuleprobability = mutationWeight.CombineModulesWeight / sumPossibilities;
        movegeneToDifferentModuleProbability = mutationWeight.MoveGeneToDifferentModuleWeight / sumPossibilities;
    }

    protected override void PerformMutate(IChromosome chromosome, float probability)
    {

        // You may need to cast to your concrete chromosome type
        var encoding = chromosome as LinearLinkageEncoding;
        if (encoding == null)
            throw new InvalidOperationException("Chromosome must be of type YourEncodingChromosome.");

        // Check if the encoding is valid

        if (!encoding.IsValid())
        {
            return;
        }

        var rnd = RandomizationProvider.Current;

        double opRoll = rnd.GetFloat();

        if (opRoll < divideModuleprobability)
        {
            // Divide a random module
            LinearLinkageEncodingOperator.DivideRandomModule(encoding);
        }
        else if (opRoll < combinedModuleprobability)
        {
            // Combine modules
            if (LinearLinkageEncodingInformationService.GetNumberOfNonIsolatedModules(encoding) > 2)
            {
                LinearLinkageEncodingOperator.CombineRandomGroup(encoding);
            }

        }
        else
        {
            // Move a gene to a different module
            LinearLinkageEncodingOperator.MoveRandomGeneToIncidentModule(encoding);
        }
        // else: no mutation this time
    }
}
