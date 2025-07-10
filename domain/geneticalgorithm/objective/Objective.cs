using System;
using System.Reflection;
using GeneticSharp;
using MA_GA.models.enums;
using MA_GA.Models;

namespace MA_GA.domain.geneticalgorithm.objective;

public abstract class Objective : IFitness
{

    protected double weight;
    protected Graph graph;
    protected int numberOFElementsPerModule;

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

    public bool isNumberOfElementsNeeded()
    {
        return false;
    }

    public void SetNumberOfElementsPerModule(int numberOFElementsPerModule)
    {
        this.numberOFElementsPerModule = numberOFElementsPerModule;
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
        throw new NotImplementedException();
    }
}
