using System;
using GeneticSharp;
using MA_GA.domain.module;
using MA_GA.Models;

namespace MA_GA.domain.GeneticAlgorithm.encoding;

public class LinearLinkageEncoding : ChromosomeBase
{

    private readonly Module _module;
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
}
