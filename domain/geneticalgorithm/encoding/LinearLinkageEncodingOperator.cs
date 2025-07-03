using GeneticSharp;
using MA_GA.domain.geneticalgorithm.encoding;
using MA_GA.domain.module;
using Module = MA_GA.domain.module.Module;

namespace MA_GA.domain.geneticalgorithm.encoding;

public sealed class LinearLinkageEncodingOperator
{

    public static IChromosome DivideRandomModule(LinearLinkageEncoding encoding)
    {
        var moduleWithMultipleGenes = encoding.GetModules()
            .Where(module => module.GetIndices().Count > 1 && ModuleInformationService.IsModuleConnected(module, encoding.GetGraph()))
            .ToList();

        if (moduleWithMultipleGenes.Count == 0)
        {
            return encoding;
        }

        var random = new Random();
        moduleWithMultipleGenes = moduleWithMultipleGenes.OrderBy(_ => random.Next()).ToList();

        var selectedModule = moduleWithMultipleGenes.First();

        var splittedModules = ModuleService.DivideModuleRandomly(selectedModule, encoding.GetGraph()).ToList();

        return UpdateIntegerGenes(splittedModules, encoding);


    }

    public static IChromosome CombineRandomGroup(LinearLinkageEncoding encoding)
    {
        var modules = encoding.GetModules();
        var random = new Random();
        var randomModules = modules.OrderBy(_ => random.Next()).ToList();
        var selectedModule = randomModules.First();

        if (selectedModule == null)
        {
            return encoding;
        }

        // find neighbor modules
        var neighborModules = ModuleInformationService.GetModuleNeighbors(selectedModule, encoding);

        if (neighborModules.Count == 0)
        {
            return encoding;
        }

        neighborModules = neighborModules.OrderBy(_ => random.Next()).ToList();

        var secondSelected = neighborModules.First();

        var combinedModule = ModuleService.MergeModules(selectedModule, secondSelected);
        var combinedModuleList = new List<Module> { combinedModule };

        return UpdateIntegerGenes(combinedModuleList, encoding);
    }

    public static IChromosome MoveRandomGeneToIncidentModule(LinearLinkageEncoding encoding)
    {
        //select only modules which have neighbors and it element is not isolated
        var moduleWithIncidenModules = encoding.GetModules().Where(module =>
             !ModuleInformationService.IsIsolated(module, encoding.GetGraph()) &&
             ModuleInformationService.GetModuleNeighbors(module, encoding).Count > 0).ToList();

        if (!moduleWithIncidenModules.Any())
        {
            return encoding;
        }

        var random = new Random();
        var sourceModuleIndex = random.Next(moduleWithIncidenModules.Count);
        var sourceModule = moduleWithIncidenModules[sourceModuleIndex];

        var graph = encoding.GetGraph();

        var movableElements = sourceModule.GetIndices().Select(index => graph.GetModularisableElementByIndex(index)).ToDictionary(
            element => element,
            element => ModuleInformationService.GetIncidentModules(element, encoding));

        var candidateElements = movableElements.Where(kv => kv.Value.Any()).ToDictionary(kv => kv.Key, kv => kv.Value);

        if (!candidateElements.Any())
        {
            return encoding;
        }

        var modularisableElemToMove = candidateElements.Keys.ToList()[random.Next(candidateElements.Count)];

        // target module 
        var targetModules = candidateElements[modularisableElemToMove];
        var targetModule = targetModules[random.Next(targetModules.Count)];

        // move the element 
        var elementIndex = modularisableElemToMove.GetIndex();
        sourceModule.RemoveIndex(elementIndex);
        targetModule.AddIndex(elementIndex);

        // affected modules
        var effectedModules = new List<Module> { targetModule };
        // add source module if it still has indices
        if (sourceModule.GetIndices().Count > 0)
        {
            effectedModules.Add(sourceModule);
        }

        // check if source module connected 
        var isSourceModuleConnected = ModuleInformationService.IsModuleConnected(sourceModule, graph);

        if (!isSourceModuleConnected)
        {
            var splitUpdateModules = ModuleService.SplitNonIncidentModule(sourceModule, encoding).ToList();
            effectedModules.AddRange(splitUpdateModules);
        }

        return UpdateIntegerGenes(effectedModules, encoding);


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
