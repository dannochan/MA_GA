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

        return new LinearLinkageEncoding(encoding.GetGraph(), integerGenes);

    }

    public static void UpdateModule(Module module, List<Gene> integerGenes)
    {
        // get a list of index of element in module
        var indices = module.GetIndices();

        // retrieve the genes based on the indices
        var affectedGenes = new List<Gene>();
        foreach (var index in indices)
        {
            affectedGenes.Add(integerGenes[index]);
        }

        for (int i = 0; i < affectedGenes.Count - 1; i++)
        {

            var indexOfModule = indices[i];
            var successor = indices[i + 1];
            var updatedGene = new Gene(successor);
            integerGenes[indexOfModule] = updatedGene;
        }

        var lastAffectedGene = affectedGenes.Last();
        var updatedLastGene = new Gene(lastAffectedGene.Value);
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

        if (LinearLinkageEncodingInformationService.IsMonolith(repairedLinearLinkageEncoding))
        {
            repairedLinearLinkageEncoding = RandomlySplitUpModulesV2(lle);

        }



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
        var selectedModule = modulesToSplit[random.GetInt(0, modulesToSplit.Count)];

        var splitupModule = ModuleService.DivideModuleRandomWalk2(selectedModule, lle.GetGraph());
        return UpdateIntegerGenes(splitupModule, lle);
    }



    private static LinearLinkageEncoding RepairInvalidGeneAssignment(LinearLinkageEncoding repairedLinearLinkageEncoding)
    {
        var genes = repairedLinearLinkageEncoding.GetIntegerGenes().Select(g => g.Value).ToList();
        var alleleCounts = new Dictionary<int, int>();
        var unusedAlleles = new List<int>();
        int numGenes = genes.Count;

        // Count occurrencesgit c
        foreach (int allele in genes)
        {
            if (!alleleCounts.ContainsKey(allele))
                alleleCounts[allele] = 0;
            alleleCounts[allele]++;
        }

        // Build list of all possible alleles (assuming alleles are 0..numGenes-1)
        for (int i = 0; i < numGenes; i++)
        {
            if (!alleleCounts.ContainsKey(i) || alleleCounts[i] < LinearLinkageEncodingConstant.ALLOWED_AMOUNT_OF_SAME_ALLELES)
            {
                for (int cnt = alleleCounts.ContainsKey(i) ? alleleCounts[i] : 0;
                     cnt < LinearLinkageEncodingConstant.ALLOWED_AMOUNT_OF_SAME_ALLELES; cnt++)
                {
                    unusedAlleles.Add(i);
                }
            }
        }

        // Shuffle unusedAlleles for random assignment
        var random = RandomizationProvider.Current;
        unusedAlleles = unusedAlleles.OrderBy(x => random.GetInt(0, int.MaxValue)).ToList();

        // Replace overused alleles
        for (int i = 0; i < genes.Count; i++)
        {
            int allele = (int)genes[i];
            if (alleleCounts[allele] > LinearLinkageEncodingConstant.ALLOWED_AMOUNT_OF_SAME_ALLELES)
            {
                // Replace with an unused allele
                if (unusedAlleles.Count > 0)
                {
                    genes[i] = unusedAlleles[0];
                    alleleCounts[allele]--;
                    alleleCounts[unusedAlleles[0]] = alleleCounts.GetValueOrDefault(unusedAlleles[0], 0) + 1;
                    unusedAlleles.RemoveAt(0);
                }
            }
        }

        // Assume you have a constructor like: new LinearLinkageEncoding(List<int> genes, KnowledgeGraph knowledgeGraph)
        return new LinearLinkageEncoding(repairedLinearLinkageEncoding.GetGraph(), genes.Select(g => new Gene(g)).ToList());
    }

    private static LinearLinkageEncoding RepairModulesWithOnlyOneVertexOrEdge(LinearLinkageEncoding repairedLinearLinkageEncoding)
    {
        var knowledgeGraph = repairedLinearLinkageEncoding.GetGraph();
        var modules = new HashSet<Module>(repairedLinearLinkageEncoding.GetModules());

        // Identify modules with 1 or 2 elements that are not isolated
        var invalidModules = repairedLinearLinkageEncoding.GetModules().Where(m => m.GetIndices().Count <= 2 && !ModuleInformationService.IsIsolated(m, knowledgeGraph))
            .ToList();

        var rnd = RandomizationProvider.Current;
        foreach (var module in invalidModules)
        {
            // Find neighbors (modules sharing an edge in the knowledge graph)
            var neighbors = ModuleInformationService.GetModuleNeighbors(module, repairedLinearLinkageEncoding)
                .Where(neighbor => neighbor != module)
                .ToList();

            if (!neighbors.Any())
                continue;

            // Randomly select a neighbor module to merge with
            var selectedNeighbor = neighbors[rnd.GetInt(0, neighbors.Count)];

            // Merge modules
            ModuleService.MergeModules(module, selectedNeighbor);
            modules.Remove(module);
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

                    var random = new Random();
                    var randomIncidentModule = incidentModules[random.Next(incidentModules.Count)];

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
