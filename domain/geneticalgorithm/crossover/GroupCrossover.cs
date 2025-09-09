using System;
using GeneticSharp;
using MA_GA.domain.geneticalgorithm.encoding;
using MA_GA.domain.module;
using MA_GA.Models;

namespace MA_GA.domain.geneticalgorithm.crossover;

public class GroupCrossover : CrossoverBase
{
    private readonly Graph baseGraph;

    public GroupCrossover(Graph graph) : base(2, 2)
    {
        baseGraph = graph ?? throw new ArgumentNullException(nameof(graph), "Graph cannot be null");
    }
    protected override IList<IChromosome> PerformCross(IList<IChromosome> parents)
    {
        var parent1 = new LinearLinkageEncoding(parents[0], baseGraph);
        var parent2 = new LinearLinkageEncoding(parents[1], baseGraph);


        LinearLinkageEncoding offspring1;
        LinearLinkageEncoding offspring2;

        if (!parent1.IsValid() || !parent2.IsValid())
        {
            offspring1 = parent1.Clone();
            offspring2 = parent2.Clone();

            // If either parent is invalid, return  repaired clones of the parents
            offspring1 = (LinearLinkageEncoding)LinearLinkageEncodingOperator.FixLinearLinkageEncoding(offspring1);
            offspring2 = (LinearLinkageEncoding)LinearLinkageEncodingOperator.FixLinearLinkageEncoding(offspring2);

            return new List<IChromosome> { offspring1, offspring2 };
        }
        else
        {
            offspring1 = (LinearLinkageEncoding)parent1.CreateNew();
            offspring2 = (LinearLinkageEncoding)parent2.CreateNew();
        }


        var newModulesForOffspring1 = DetermineNewModulesForOffspring(parent1, parent2);
        var newModulesForOffspring2 = newModulesForOffspring1
                                  .ToDictionary(kvp => kvp.Key,
                                                kvp => (Module)kvp.Value.Clone());

        for (int i = 0; i < parent1.Length; i++)
        {

            var geneInParent1 = parent1.GetIntegerGene(i);
            var geneInParent2 = parent2.GetIntegerGene(i);

            if (IsEndingNode(geneInParent1, i) && IsEndingNode(geneInParent2, i)) continue;

            AssignGeneToOneOfNewModules(parent1, newModulesForOffspring1, i);
            AssignGeneToOneOfNewModules(parent2, newModulesForOffspring2, i);

        }


        UpdateparentToOffspring(offspring1, newModulesForOffspring1.Values.ToList());
        UpdateparentToOffspring(offspring2, newModulesForOffspring2.Values.ToList());

        if (!LinearLinkageEncodingInformationService.IsValidChromose(offspring1))
        {
            offspring1 = (LinearLinkageEncoding)LinearLinkageEncodingOperator.FixLinearLinkageEncoding(offspring1);
        }
        ;
        if (!LinearLinkageEncodingInformationService.IsValidChromose(offspring2))
        {
            offspring2 = (LinearLinkageEncoding)LinearLinkageEncodingOperator.FixLinearLinkageEncoding(offspring2);
        }


        return new List<IChromosome> { offspring1, offspring2 };
    }


    private void UpdateparentToOffspring(LinearLinkageEncoding offspring, List<Module> modules)
    {
        foreach (var module in modules)
        {
            var indicesOfModule = module.GetIndices();
            for (int i = 0; i < indicesOfModule.Count - 1; i++)
            {
                //   offspring.ReplaceGene(indicesOfModule[i], new Gene(indicesOfModule[i + 1]));
                offspring.ReplaceIntegerGene(indicesOfModule[i], new Gene(indicesOfModule[i + 1]));

            }
            var lastIndex = indicesOfModule[indicesOfModule.Count - 1];

            // offspring.ReplaceGene(lastIndex, new Gene(lastIndex));
            offspring.ReplaceIntegerGene(lastIndex, new Gene(lastIndex));
        }


    }

    private void AssignGeneToOneOfNewModules(LinearLinkageEncoding encodingParent, IDictionary<int, Module> newModuleForOffspring, int index)
    {
        var geneInParent = encodingParent.GetIntegerGene(index);
        var moduleOfGene = encodingParent.GetModuleOfAllele((int)geneInParent.Value);
        var alleleEndingNodeInParent = moduleOfGene.GetAlleleOfEndingNode();

        if (newModuleForOffspring.ContainsKey(alleleEndingNodeInParent))
        {
            newModuleForOffspring[alleleEndingNodeInParent].AddIndex(index);
            return;
        }

        var modularisableElement = baseGraph.GetModularisableElementByIndex(index);
        var incidentModularisableElements = baseGraph.GetIncidentModularisableElements(modularisableElement);

        var potentialModules = newModuleForOffspring.Values
            .Where(m => incidentModularisableElements.Any(e => m.CheckIndexInModule(e.GetIndex())))
            .ToList();

        if (potentialModules.Count > 0)
        {
            var random = RandomizationProvider.Current;
            var randomModule = potentialModules[random.GetInt(0, potentialModules.Count)];
            randomModule.AddIndex(index);
        }
        else
        {
            // If no potential module found, create a new one
            var newModule = new Module();
            newModule.AddIndex(index);
            newModuleForOffspring.Add(index, newModule);
        }

    }

    private bool IsEndingNode(Gene geneInParent, int i)
    {
        return geneInParent.Value is int value && value == i;
    }

    private bool IsEndingNode(LinearLinkageEncoding encoding, int index)
    {
        return IsEndingNode(encoding.GetIntegerGene(index), index);
    }

    /// <summary>
    /// Determines new modules for the offspring based on the parents' encodings.
    /// new modules should contains end nodes as follow:
    /// in the first parent and the second parent the integer is an ending node
    /// in the first parent integer gene is  an endig node with 50% probability
    /// in the second parent integer gene is an ending node with 50% probability
    /// isolated vertices are always ending nodes
    /// </summary>
    /// <param name="encodingParent1"></param>
    /// <param name="encodingParent2"></param>
    /// <returns></returns>

    private IDictionary<int, Module> DetermineNewModulesForOffspring(LinearLinkageEncoding encodingParent1, LinearLinkageEncoding encodingParent2)
    {
        var random = RandomizationProvider.Current;
        var newModules = new Dictionary<int, Module>();

        for (int i = 0; i < encodingParent1.Length; i++)
        {
            var moduleElement = baseGraph.GetModularisableElementByIndex(i);
            var isIsolatedVertex = moduleElement is DataObject dataObject &&
            baseGraph.IsIsolatedVertex(dataObject);

            if (isIsolatedVertex || (IsEndingNode(encodingParent1, i) && IsEndingNode(encodingParent2, i)) || (IsEndingNode(encodingParent1, i) && random.GetDouble() < 0.5d) || (IsEndingNode(encodingParent2, i) && random.GetDouble() < 0.5d))
            {

                var module = new Module();
                module.AddIndex(i);

                newModules.Add(i, module);

            }

        }

        return newModules;

    }
}
