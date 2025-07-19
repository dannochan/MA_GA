using System;
using GeneticSharp;
using MA_GA.domain.geneticalgorithm.encoding;
using MA_GA.domain.module;
using MA_GA.Models;

namespace MA_GA.domain.geneticalgorithm.encoding;

public class LinearLinkageEncoding : ChromosomeBase
{

    private readonly List<Module> Modules;
    private IReadOnlyList<Gene> IntegerGenes { get; }
    private readonly Graph BaseGraph;


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
        BaseGraph = graph ?? throw new ArgumentNullException(nameof(graph), "Graph cannot be null");
        IntegerGenes = new List<Gene>(length).AsReadOnly();
        Modules = new List<Module>();
    }


    /// <summary>
    ///  Initializes a new instance of the <see cref="LinearLinkageEncoding"/> class.
    ///  Since there is no Genotype in GeneticSharp, this constructor initializes using chromosome genes.
    ///  This constructor initializes the encoding with a specified graph and a chromosome.
    ///  The genes of the chromosome are used to initialize the encoding.
    ///  It throws an ArgumentNullException if the graph is null.
    /// </summary>
    /// <param name="chromosome"></param>
    /// <param name="graph"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public LinearLinkageEncoding(IChromosome chromosome, Graph graph) : this(graph, chromosome.GetGenes().ToList())
    {

    }

    public LinearLinkageEncoding(Graph graph, IReadOnlyList<Gene> genes) : base(genes.Count)
    {
        BaseGraph = graph ?? throw new ArgumentNullException(nameof(graph), "Graph cannot be null");

        IntegerGenes = genes.ToList().AsReadOnly();
        for (int i = 0; i < genes.Count; i++)
        {
            ReplaceGene(i, genes[i]);
        }
        Modules = LinearLinkageEncodingInformationService.DetermineModules(this);
    }

    public override IChromosome CreateNew()
    {
        var CloneGenes = IntegerGenes.Select(g => new Gene(g.Value)).ToList();
        var encoding = new LinearLinkageEncoding(BaseGraph, CloneGenes);
        return MutateLinearLinkageEncoding(encoding);
    }

    private IChromosome MutateLinearLinkageEncoding(LinearLinkageEncoding encoding)
    {
        var random = RandomizationProvider.Current;
        var value = random.GetDouble();
        if (value < 1.0d / 3.0d)
        {
            return LinearLinkageEncodingOperator.DivideRandomModule(encoding);
        }
        else if (value < 2.0d / 3.0d)
        {
            if (LinearLinkageEncodingInformationService.GetNumberOfNonIsolatedModules(encoding) > 2)
            {
                return LinearLinkageEncodingOperator.CombineRandomGroup(encoding);
            }
        }

        return LinearLinkageEncodingOperator.MoveRandomGeneToIncidentModule(encoding);

    }

    public override Gene GenerateGene(int geneIndex)
    {
        return new Gene(IntegerGenes[geneIndex]);
    }

    public int GetChromosomeLength()
    {
        return Length;
    }

    public bool IsValid()
    {
        // Implement validation logic if needed
        return LinearLinkageEncodingInformationService.IsValidLinearLinkageEncoding(this);
    }

    public Graph GetGraph()
    {
        return BaseGraph;
    }

    public List<Module> GetModules()
    {
        return Modules;
    }

    public Module GetModuleOfAllele(int allele)
    {
        Module module = Modules.First(m => m.CheckIndexInModule(allele));
        if (module == null)
        {
            throw new NullReferenceException($"Module not found for allele {allele}. Ensure the allele is part of the encoding.");
        }
        return module;
    }

    public override string ToString()
    {

        return string.Join(", ", IntegerGenes.Select(g => g.Value));
    }
    public void DisplayChromosome()
    {
        var indices = new List<int>();
        // Display the encoding
        Console.WriteLine("Linear Linkage Encoding:");
        Console.WriteLine("Module Count: " + this.GetModules()?.Count);
        Console.WriteLine("indices of modules:");
        foreach (var module in this.GetModules())
        {
            foreach (var index in module.GetIndices())
            {
                indices.Add(index);
                Console.Write(index + " -> ");
            }

        }
        Console.WriteLine("End of Module indices"); ;

        // Display the genes
        Console.WriteLine("Genes:");
        var geneIndices = this.GetGenes().Select(g => g.Value).ToList();
        for (int i = 0; i < indices.Count; i++)
        {
            var geneIndex = indices[i];
            Console.Write(geneIndices[geneIndex] + " -> ");
        }

        Console.WriteLine("End of Gene Indices ");


    }
}
