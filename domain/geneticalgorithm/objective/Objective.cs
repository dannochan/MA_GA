using System;
using System.Reflection;
using GeneticSharp;
using MA_GA.domain.geneticalgorithm.encoding;
using MA_GA.domain.module;
using MA_GA.models.enums;
using MA_GA.Models;
using Microsoft.Extensions.Logging;
using Module = MA_GA.domain.module.Module;

namespace MA_GA.domain.geneticalgorithm.objective;

public abstract class Objective
{

    protected double weight;
    protected Graph graph;
    protected int numberOfElementsPerModule;

    public abstract double CalculateValue(List<Module> modules);

    public abstract string GetObjectiveName();
    public abstract OptimizationType GetOptimizationType();
    public abstract ObjectiveType GetObjectiveType();

    public double GetWeight() { return weight; }

    public void SetWeight(double weight)
    {
        if (weight < 0)
        {
            throw new ArgumentException("Weight cannot be negative.");
        }
        this.weight = weight;
    }

    public bool IsNumberOfElementsNeeded()
    {
        return false;
    }

    public void SetGraph(Graph graph)
    {
        this.graph = graph;
    }

    public void SetNumberOfElementsPerModule(int numberOFElementsPerModule)
    {
        this.numberOfElementsPerModule = numberOFElementsPerModule;
    }

    public void GetPrepare()
    {

    }

    public override string ToString()
    {
        return $"{GetObjectiveName()} (Weight: {weight})";
    }

    public double Evaluate(IChromosome chromosome)
    {
        if (chromosome is not LinearLinkageEncoding encoding)
        {
            throw new ArgumentException($"Chromosome must be of type {nameof(LinearLinkageEncoding)}", nameof(chromosome));
        }
        var lle = encoding as LinearLinkageEncoding;
        return this.CalculateValue(lle.GetModules());
    }

}
