using GeneticSharp;
using MA_GA.domain.geneticalgorithm.encoding;
using MA_GA.domain.module;
using MA_GA.Models;
using Module = MA_GA.domain.module.Module;

namespace MA_GA.domain.geneticalgorithm.encoding;

public sealed class LinearLinkageEncodingOperator
{
    public static int COUNT_LOOP_TERMINATION = 100;

    public static IChromosome DivideRandomModule(LinearLinkageEncoding encoding)
    {
        var moduleWithMultipleGenes = encoding.GetModules()
            .Where(module => module.GetIndices().Count > 1 && ModuleInformationService.IsModuleConnected(module, encoding.GetGraph()))
            .ToList();

        if (moduleWithMultipleGenes.Count == 0)
        {
            return encoding;
        }

        var random = RandomizationProvider.Current;
        var randomModule = random.GetInt(0, moduleWithMultipleGenes.Count - 1);

        var selectedModule = moduleWithMultipleGenes[randomModule];

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

        var random = RandomizationProvider.Current;
        var sourceModuleIndex = random.GetInt(0, moduleWithIncidenModules.Count - 1);// - 1);
        var sourceModule = moduleWithIncidenModules[sourceModuleIndex];

        var graph = encoding.GetGraph();

        var movableElementsWithNeighbourInOtherModules = sourceModule.GetIndices().Select(index => graph.GetModularisableElementByIndex(index)).ToDictionary(
            element => element,
            element => ModuleInformationService.GetIncidentModules(element, encoding));

        var candidateElements = movableElementsWithNeighbourInOtherModules.Where(kv => kv.Value.Any()).ToDictionary(kv => kv.Key, kv => kv.Value);

        if (!candidateElements.Any())
        {
            return encoding;
        }

        var modularisableElemToMove = candidateElements.Keys.ToList()[random.GetInt(0, candidateElements.Count - 1)]; // -1

        // target module 
        var targetModules = candidateElements[modularisableElemToMove];
        var targetModule = targetModules[random.GetInt(0, targetModules.Count)];

        // move the element 
        var elementIndex = modularisableElemToMove.GetIndex();
        sourceModule.RemoveIndex(elementIndex);
        targetModule.AddIndex(elementIndex);

        // affected modules
        var effectedModules = new List<Module> { targetModule };
        // add source module if it still has indices

        if (sourceModule.GetIndices().Count > 0)
        {

            // check if source module still connected when elements are moved
            var isSourceModuleConnected = ModuleInformationService.IsModuleConnected(sourceModule, graph);

            if (!isSourceModuleConnected)
            {
                var splitUpdateModules = ModuleService.SplitNonIncidentModule(sourceModule, encoding).ToList();
                effectedModules.AddRange(splitUpdateModules);
            }
            else
            {
                effectedModules.Add(sourceModule);

            }


        }


        return UpdateIntegerGenes(effectedModules, encoding);


    }

    public static LinearLinkageEncoding UpdateIntegerGenes(List<Module> effectedModules, LinearLinkageEncoding encoding)
    {

        var integerGenes = encoding.GetIntegerGenes();
        // effectedModules.ForEach(module => UpdateModule(module, integerGenes));
        //   var newIntegergenes = new List<Gene>(integerGenes.Count);
        //   foreach (var module in effectedModules)
        //   {
        //       UpdateModule(module, integerGenes);
        //   }
        for (int i = 0; i < effectedModules.Count; i++)
        {
            var module = effectedModules[i];
            UpdateModule(module, integerGenes);
        }

        return new LinearLinkageEncoding(encoding.GetGraph(), integerGenes);

    }

    public static void UpdateModule(Module module, List<Gene> integerGenes)
    {
        // get a list of index of element in module
        var indices = module.GetIndices();

        // retrieve the genes based on the indices
        var affectedGenes = indices.Select(index => integerGenes[index]).ToList();


        for (int i = 0; i < affectedGenes.Count - 1; i++)
        {

            var indexOfModule = indices[i];
            var successor = indices[i + 1];
            var updatedGene = new Gene(successor);
            integerGenes[indexOfModule] = updatedGene;
        }

        var lastAffectedGene = indices.Last();
        var updatedLastGene = new Gene(lastAffectedGene);
        integerGenes[indices.Last()] = updatedLastGene;
    }

    public static LinearLinkageEncoding FixLinearLinkageEncoding(LinearLinkageEncoding lle)
    {
        var repairedLinearLinkageEncoding = new LinearLinkageEncoding(lle.GetGraph(), lle.GetIntegerGenes());

        if (!LinearLinkageEncodingInformationService.IsAllElementsInModuleConnected(repairedLinearLinkageEncoding))
        {
            repairedLinearLinkageEncoding = RepairNonConnectedModules2(repairedLinearLinkageEncoding);

        }

        if (LinearLinkageEncodingInformationService.IsOneModuleConsistOfOneEdge(repairedLinearLinkageEncoding))
        {

            repairedLinearLinkageEncoding =
                    RepairModulesWithOnlyOneVertexOrEdge(repairedLinearLinkageEncoding);

        }

        if (!LinearLinkageEncodingInformationService.IsValidAlleleValues(repairedLinearLinkageEncoding))
        {
            repairedLinearLinkageEncoding =
                    RepairInvalidGeneAssignment(repairedLinearLinkageEncoding);

        }

        if (LinearLinkageEncodingInformationService.IsMonolith(lle))
        {
            repairedLinearLinkageEncoding = RandomlySplitUpModulesV2(lle);

        }

        /*
                if (!repairedLinearLinkageEncoding.IsValid())
                {
                    repairedLinearLinkageEncoding = FixLinearLinkageEncoding(repairedLinearLinkageEncoding);
                }


        */
        // Add logic to check for connectivity and repair, similar to Jav
        return repairedLinearLinkageEncoding;
    }

    private static LinearLinkageEncoding RandomlySplitUpModulesV2(LinearLinkageEncoding lle)
    {
        // Get modules with more than one element
        var modulesToSplit = lle.GetModules().Where(m => m.GetIndices().Count > 1).ToList();

        if (modulesToSplit.Count == 0)
        {
            // Nothing to split
            return lle;
        }

        // Select a random module to split
        var random = RandomizationProvider.Current;
        var selectedModule = modulesToSplit[random.GetInt(0, modulesToSplit.Count - 1)];

        var splitupModule = ModuleService.DivideModuleRandomWalk2(selectedModule, lle.GetGraph());
        return UpdateIntegerGenes(splitupModule, lle);
    }



    public static LinearLinkageEncoding RepairInvalidGeneAssignment(LinearLinkageEncoding linearLinkageEncoding)
    {
        var interGenes = linearLinkageEncoding.GetIntegerGenes().Select(g => (int)g.Value).ToList();

        // Create a list of remaining allele values, which should be assigned afterwards
        var remainingUnassignedAlleles = Enumerable.Range(0, linearLinkageEncoding.GetIntegerGenes().Count).ToList();

        // Create a map to track the number of already existing allele values.
        var alleleCountMap = new Dictionary<int, int>();

        // Keep track of number of alleles and determine the remaining unused alleles
        foreach (var integerGene in interGenes)
        {
            var allele = integerGene;
            if (alleleCountMap.ContainsKey(allele))
                alleleCountMap[allele]++;
            else
                alleleCountMap[allele] = 1;

            // Remove alleles to keep unassigned alleles for later assignment
            remainingUnassignedAlleles.Remove(allele);
        }

        // Create a map for the remaining unused alleles
        var remainingUnassignedAllelesMap = remainingUnassignedAlleles.ToDictionary(a => a, a => 2);

        // Start assigning remaining alleles to one of invalid assigned genes
        var updatedInterGenes = new List<int>(interGenes);

        foreach (var overusedAllele in alleleCountMap.Where(kv => kv.Value > LinearLinkageEncodingConstant.ALLOWED_AMOUNT_OF_SAME_ALLELES).ToList())
        {
            var allele = overusedAllele.Key;

            // Determine the indices of allele values
            var indicesOfOverusedAllele = new List<int>(Enumerable.Range(0, updatedInterGenes.Count)
                .Where(i => updatedInterGenes[i] == allele).ToList());

            // Repeat process until only MAX_NUMBER_OF_SAME_ALLELE remain
            int countReassignments = indicesOfOverusedAllele.Count - LinearLinkageEncodingConstant.ALLOWED_AMOUNT_OF_SAME_ALLELES;
            var random = RandomizationProvider.Current;

            for (int i = 0; i < countReassignments; i++)
            {
                var remainingPossibleUnassignedAlleles = remainingUnassignedAllelesMap.Keys.ToList();

                // Randomly assign one of the remaining unused alleles
                int randomRemainingUnusedAlleleIndex = random.GetInt(0, remainingPossibleUnassignedAlleles.Count);
                int randomRemainingUnusedAllele = remainingPossibleUnassignedAlleles[randomRemainingUnusedAlleleIndex];

                int randomIndexOfOverusedAllelesIndex = random.GetInt(0, indicesOfOverusedAllele.Count);
                int randomIndexOfOverusedAlleles = indicesOfOverusedAllele[randomIndexOfOverusedAllelesIndex];
                // Replace the overused allele with the random remaining unused allele

                updatedInterGenes[randomIndexOfOverusedAlleles] = randomRemainingUnusedAllele;

                // Decrease usage of this element
                remainingUnassignedAllelesMap[randomRemainingUnusedAllele]--;
                if (remainingUnassignedAllelesMap[randomRemainingUnusedAllele] == 0)
                    remainingUnassignedAllelesMap.Remove(randomRemainingUnusedAllele);

                indicesOfOverusedAllele.RemoveAt(randomIndexOfOverusedAllelesIndex);
            }
        }

        return new LinearLinkageEncoding(linearLinkageEncoding.GetGraph(),
            updatedInterGenes.Select(g => new Gene(g)).ToList());
    }

    private static LinearLinkageEncoding RepairModulesWithOnlyOneVertexOrEdge(LinearLinkageEncoding repairedLinearLinkageEncoding)
    {
        var knowledgeGraph = repairedLinearLinkageEncoding.GetGraph();
        var modules = new HashSet<Module>(repairedLinearLinkageEncoding.GetModules());

        // Identify modules with 1 or 2 elements that are not isolated
        var invalidModules = repairedLinearLinkageEncoding.GetModules().Where(m => m.GetIndices().Count <= 1 && !ModuleInformationService.IsIsolated(m, knowledgeGraph))
            .ToList();

        var rnd = RandomizationProvider.Current;
        for (int i = 0; i < invalidModules.Count; i++)
        {
            if (invalidModules[i].GetIndices().Count <= 1)
            {
                // If the module has only one element, randomly assign it to a neighboring module
                var neighbors = ModuleInformationService.GetModuleNeighbors(invalidModules[i], repairedLinearLinkageEncoding);
                if (neighbors.Count == 0) continue;

                var selectedNeighbor = neighbors[rnd.GetInt(0, neighbors.Count - 1)];

                // Merge modules
                var mergeModule = ModuleService.MergeModules(invalidModules[i], selectedNeighbor);
                /*
                                if (invalidModules.Contains(invalidModules[i]))
                                {
                                    // replace the invalid module with the merged one

                                    invalidModules.Remove(invalidModules[i]);
                                    invalidModules.Add(mergeModule);
                                }
                */
                modules.Remove(invalidModules[i]);
                modules.Remove(selectedNeighbor);
                modules.Add(mergeModule);

            }

        }

        // Update the LinearLinkageEncoding with the repaired modules
        return UpdateIntegerGenes(modules.ToList(), repairedLinearLinkageEncoding);
    }


    public static LinearLinkageEncoding RepairNonConnectedModules2(LinearLinkageEncoding linearLinkageEncoding)
    {
        var knowledgeGraph = linearLinkageEncoding.GetGraph();
        var modules = new List<Module>(linearLinkageEncoding.GetModules());
        var invalidModules = modules
            .Where(module => module.GetIndices().Count > 1 &&
                             !ModuleInformationService.IsModuleConnected(module, knowledgeGraph))
            .ToList();

        int iterations = 0;

        while (invalidModules.Any())
        {
            foreach (var invalidModule in invalidModules.ToList())
            {
                var nonConnectedModularisableElements =
                    ModuleInformationService.GetNonConnectedModularisableElements(invalidModule, knowledgeGraph);

                foreach (var nonConnectedModularisableElement in nonConnectedModularisableElements)
                {
                    var incidentModules = ModuleInformationService.GetIncidentModules(
                        nonConnectedModularisableElement, linearLinkageEncoding);

                    if (!incidentModules.Any())
                        continue;

                    var random = RandomizationProvider.Current;
                    var randomIncidentModule = incidentModules[random.GetInt(0, incidentModules.Count)];

                    // Move index to target module and remove from current module
                    invalidModule.RemoveIndex(nonConnectedModularisableElement.GetIndex());
                    randomIncidentModule.AddIndex(nonConnectedModularisableElement.GetIndex());

                    if (!invalidModule.GetIndices().Any())
                        modules.Remove(invalidModule);
                }
            }

            invalidModules = modules
                .Where(module => module.GetIndices().Count > 1 &&
                                 !ModuleInformationService.IsModuleConsistOfSoloVertex(module, knowledgeGraph) &&
                                 !ModuleInformationService.IsModuleConnected(module, knowledgeGraph))
                .ToList();

            iterations++;

            if (iterations >= COUNT_LOOP_TERMINATION)
            {
                // fallback logic, e.g. split each connected component into its own module
                var initialLinearLinkageEncoding =
                    LinearLinkageEncodingInitialiser.InitializeLinearLinkageEncodingWithModulesForEachConnectedCompponent(knowledgeGraph);

                var randomlySplitUpLinearLinkageEncoding =
                    RandomlySplitUpModules(initialLinearLinkageEncoding);

                return randomlySplitUpLinearLinkageEncoding;
            }
        }

        return UpdateIntegerGenes(modules, linearLinkageEncoding);
    }

    private static LinearLinkageEncoding RandomlySplitUpModules(LinearLinkageEncoding initialLinearLinkageEncoding)
    {
        var possibleModules = initialLinearLinkageEncoding.GetModules().Where(m => m.GetIndices().Count > 1).ToList();

        if (possibleModules.Count == 0)
        {
            return initialLinearLinkageEncoding;
        }

        var random = RandomizationProvider.Current;
        var selectedModule = possibleModules[random.GetInt(0, possibleModules.Count)];

        var splitupModule = ModuleService.DivideModuleRandomWalk2(selectedModule, initialLinearLinkageEncoding.GetGraph());
        return UpdateIntegerGenes(splitupModule, initialLinearLinkageEncoding);


    }
}
