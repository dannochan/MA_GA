using System;
using MA_GA.domain.geneticalgorithm.encoding;
using MA_GA.domain.module;
using MA_GA.Models;
using QuikGraph.Algorithms.ConnectedComponents;

namespace MA_GA.domain.geneticalgorithm.encoding;

public sealed class LinearLinkageEncodingInitialiser
{

    /// <summary>
    /// Initializes a LinearLinkageEncoding with modules for each connected component in the graph.
    /// </summary>
    /// <param name="graph"></param>
    /// <returns></returns>
    public static LinearLinkageEncoding InitializeLinearLinkageEncodingWithModulesForEachConnectedCompponent(Graph graph)
    {
        var targetGraph = graph.GetGraph();

        var algorithm = GraphService.GetConnectedComponentsFromGraph(targetGraph);

        algorithm.Compute();
        var connectedComponents = algorithm.Components;
        var groupedComponents = connectedComponents.Keys
            .GroupBy(vertex => string.IsNullOrEmpty(vertex.Component) ? "default" : vertex.Component)
            .ToList();

        // module for each connected component
        var modules = new List<Module>();

        foreach (var component in groupedComponents)
        {
            var module = new Module();

            var verticesOfComponent = component.ToHashSet();

            var edgesOfComponent = targetGraph.Edges.Where(edge => verticesOfComponent.Contains(edge.Source) || verticesOfComponent.Contains(edge.Target)).Distinct().ToList();

            foreach (var vertex in verticesOfComponent)
            {
                module.AddIndex(vertex.GetIndex());
            }
            foreach (var edge in edgesOfComponent)
            {
                module.AddIndex(edge.GetIndex());

            }
            modules.Add(module);

        }

        // create chromosome with list of modules
        var modularisableElementSize = graph.GetModularisableElements().Count;
        var integerGenes = Enumerable.Range(0, modularisableElementSize)
            .Select(i => new GeneticSharp.Gene(i))
            .ToList();
        return LinearLinkageEncodingOperator.UpdateIntegerGenes(modules, new LinearLinkageEncoding(graph, integerGenes));
    }

    /// <summary>
    /// Initializes a LinearLinkageEncoding Chromosome using to convert the result of a greedy algorithm into a LinearLinkageEncoding.
    /// </summary>
    /// <param name="graph"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>

    public static LinearLinkageEncoding InitializeLinearLinkageEncodingWithGreedyAlgorithm(Graph graph)
    {
        if (graph == null)
        {
            throw new ArgumentNullException(nameof(graph), "Graph cannot be null");
        }
        var targetGraph = graph.GetGraph();

        var modularisableElements = graph.GetModularisableElements();

        var sortedElements = new Dictionary<string, List<int>>();

        var modules = new List<Module>();

        foreach (var element in modularisableElements)
        {
            if (element is DataObject dataObject)
            {
                if (!sortedElements.ContainsKey(dataObject.Component))
                {
                    sortedElements[dataObject.Component] = new List<int>();
                }
                sortedElements[dataObject.Component].Add(dataObject.GetIndex());
            }
            else if (element is ObjectRelation objectRelation)
            {
                if (!sortedElements.ContainsKey(objectRelation.Component))
                {
                    sortedElements[objectRelation.Component] = new List<int>();
                }
                sortedElements[objectRelation.Component].Add(objectRelation.GetIndex());
            }
        }

        foreach (var component in sortedElements)
        {
            var module = new Module();
            foreach (var index in component.Value)
            {
                module.AddIndex(index);
            }
            modules.Add(module);
        }

        // create chromosome with list of modules
        var modularisableElementSize = graph.GetModularisableElements().Count;
        var integerGenes = Enumerable.Range(0, modularisableElementSize)
    .Select(i => new GeneticSharp.Gene(i))
    .ToList()
    .AsReadOnly();


        return LinearLinkageEncodingOperator.UpdateIntegerGenes(modules, new LinearLinkageEncoding(graph, integerGenes));

    }

}
