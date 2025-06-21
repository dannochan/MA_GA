using System;
using GeneticSharp;
using MA_GA.domain.module;
using MA_GA.Models;

namespace MA_GA.domain.GeneticAlgorithm.encoding;

public class LinearLinkageEncoding : ChromosomeBase
{

    private readonly List<Module> _modules;
    private readonly Graph _graph;


    public LinearLinkageEncoding(int length) : base(length)
    {
    }

    public override IChromosome CreateNew()
    {
        throw new NotImplementedException();
    }

    public override Gene GenerateGene(int geneIndex)
    {
        throw new NotImplementedException();
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
