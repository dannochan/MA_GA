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
        var encoding = new LinearLinkageEncoding(chromosome, baseGraph);
        if (encoding == null)
            throw new InvalidOperationException("Chromosome must be of type YourEncodingChromosome.");

        // Check if the encoding is valid
        if (!LinearLinkageEncodingInformationService.IsValidChromose(encoding))
        {
            var newLLE = LinearLinkageEncodingOperator.FixLinearLinkageEncoding(encoding);
            if (newLLE != null)
            {
                encoding = newLLE;
            }
            ;
        }

        var rnd = RandomizationProvider.Current;

        if (rnd.GetFloat() < probability)
        {

            var opRoll = rnd.GetFloat();

            if (opRoll < divideModuleprobability)
            {
                // Divide a random module
                var newLLE = LinearLinkageEncodingOperator.DivideRandomModule(encoding);
                if (newLLE != null)
                {
                    encoding = (LinearLinkageEncoding)newLLE;
                }
            }
            else if (opRoll < combinedModuleprobability + divideModuleprobability)
            {
                // Combine modules
                if (LinearLinkageEncodingInformationService.GetNumberOfNonIsolatedModules(encoding) > 2)
                {
                    var newLLE = LinearLinkageEncodingOperator.CombineRandomGroup(encoding);
                    if (newLLE != null)
                    {
                        encoding = (LinearLinkageEncoding)newLLE;
                    }
                }

            }
            else
            {
                // Move a gene to a different module
                var newLLE = LinearLinkageEncodingOperator.MoveRandomGeneToIncidentModule(encoding);
                if (newLLE != null)
                {
                    encoding = (LinearLinkageEncoding)newLLE;
                }
            }
            // else: no mutation this time
            // Ensure the encoding is still valid after mutation


        }

        if (!LinearLinkageEncodingInformationService.IsValidChromose(encoding))
        {
            var newLLE = LinearLinkageEncodingOperator.FixLinearLinkageEncoding(encoding);
            if (newLLE != null)
            {
                encoding = newLLE;
            }
        }

        if (chromosome is LinearLinkageEncoding lle)
        {
            for (int i = 0; i < lle.GetChromosomeLength(); i++)
            {
                lle.ReplaceIntegerGene(i, encoding.GetIntegerGene(i));
            }
            lle.Fitness = encoding.Fitness;
            lle.Modules = encoding.Modules.Select(m => m.Clone()).ToList();

        }
        else
        {
            throw new InvalidOperationException("Chromosome must be of type LinearLinkageEncoding.");
        }

    }
}
