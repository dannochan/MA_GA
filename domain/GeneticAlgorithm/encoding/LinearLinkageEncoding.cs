using System;
using GeneticSharp;
using MA_GA.domain.module;
using MA_GA.Models;

namespace MA_GA.domain.GeneticAlgorithm.encoding;

public class LinearLinkageEncoding : ChromosomeBase
{

    private readonly List<Module> _modules;
    private IReadOnlyList<Gene> IntegerGenes { get; }
    private readonly Graph _graph;


/// <summary>
///  Initializes a new instance of the <see cref="LinearLinkageEncoding"/> class.
///  This constructor initializes the encoding with a specified graph and length.
///  The length is the number of genes in the chromosome, which refers to number of modularisable elements.
///  It throws an ArgumentNullException if the graph is null.
/// </summary>
/// <param name="graph"></param>
/// <param name="length"></param>
/// <exception cref="ArgumentNullException"></exception>
    public LinearLinkageEncoding(Graph graph, int length) : base(length)
    {
        _graph = graph ?? throw new ArgumentNullException(nameof(graph), "Graph cannot be null");
        
        _modules = new List<Module>();
    }

    public LinearLinkageEncoding(Graph graph, IReadOnlyList<Gene> genes) : base(genes.Count)
    {
        _graph = graph ?? throw new ArgumentNullException(nameof(graph), "Graph cannot be null");
        _modules = LinearLinkageEncodingInformationService.DetermineModules(this);
        IntegerGenes = genes;
    }

    public override IChromosome CreateNew()
    {
        var genes = IntegerGenes.ToList();
        var encoding = new LinearLinkageEncoding(_graph, genes.Count);
        return MutateLinearLinkageEncoding(encoding, genes);
    }

    private IChromosome MutateLinearLinkageEncoding(LinearLinkageEncoding encoding, List<Gene> genes)
    {
        throw new NotImplementedException();
    }

    public override Gene GenerateGene(int geneIndex)
    {
        return new Gene(IntegerGenes[geneIndex]);
    }

    public int GetChromosomeLength()
    {
        return Length;
    }

    public Graph GetGraph()
    {
        return _graph;
    }

    public List<Module> GetModules()
    {
        return _modules;
    }
}
