using GeneticSharp;
using MA_GA.domain.geneticalgorithm.encoding;
using Module = MA_GA.domain.module.Module;

namespace MA_GA.domain.geneticalgorithm.encoding;

public sealed class LinearLinkageEncodingOperator
{

    public static IChromosome DivideRandomModule(LinearLinkageEncoding encoding)
    {
        throw new NotImplementedException("DivideRandomModule method is not implemented yet.");

    }

    public static IChromosome CombineRandomGroup(LinearLinkageEncoding encoding)
    {
        throw new NotImplementedException("CombineRandomGroup method is not implemented yet.");
    }

    public static IChromosome MoveRandomGeneToIncidentModule(LinearLinkageEncoding encoding)
    {
        throw new NotImplementedException("CombineRandomModule method is not implemented yet.");
    }

    public static LinearLinkageEncoding UpdateIntegerGenes(List<Module> effectedModules, LinearLinkageEncoding encoding)
    {
        if (effectedModules.Count == 0)
        {
            return encoding;
        }

        var integerGenes = encoding.GetGenes().ToList();
        effectedModules.ForEach(module => UpdateModule(module, integerGenes));

        return new LinearLinkageEncoding(encoding.GetGraph(), integerGenes.AsReadOnly());

    }

    // TODO: Correct the method
    public static void UpdateModule(Module module, List<Gene> integerGenes)
    {
        var indices = module.GetIndices();

        var affectedIntegerGenes = indices.Select(index => integerGenes[index]).ToList();

        for (int i = 0; i < affectedIntegerGenes.Count - 1; i++)
        {
            var gene = affectedIntegerGenes[i];
            var indexOfModule = indices[i];
            var successor = indices[i + 1];
            var updatedGene = new Gene(successor);
            integerGenes[indexOfModule] = updatedGene;
        }

        var lastAffectedGene = affectedIntegerGenes.Last();
        var updatedLastGene = new Gene(lastAffectedGene.Value);
        integerGenes[indices.Last()] = updatedLastGene;
    }

}
