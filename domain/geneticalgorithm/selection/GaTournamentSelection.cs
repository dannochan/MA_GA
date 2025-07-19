using System;
using GeneticSharp;

namespace MA_GA.domain.geneticalgorithm.selection;

public class GaTournamentSelection : SelectionBase
{

    public int Size { get; set; }
    public bool AllowWinnerCompeteNextTournament { get; set; }

    public GaTournamentSelection() : this(2)
    {

    }
    public GaTournamentSelection(int size, bool allowWinnerCompeteNextTournament = true) : base(2)
    {
        Size = size;
        AllowWinnerCompeteNextTournament = allowWinnerCompeteNextTournament;
    }
    public GaTournamentSelection(int size) : this(size, allowWinnerCompeteNextTournament: true)
    {

    }

    protected override IList<IChromosome> PerformSelectChromosomes(int number, Generation generation)
    {
        if (Size > generation.Chromosomes.Count)
        {
            throw new SelectionException(this, "The tournament size is greater than available chromosomes. Tournament size is {0} and generation {1} available chromosomes are {2}.".With(Size, generation.Number, generation.Chromosomes.Count));
        }

        List<IChromosome> list = generation.Chromosomes.ToList();
        List<IChromosome> list2 = new List<IChromosome>();
        while (list2.Count < number)
        {
            int[] randomIndexes = RandomizationProvider.Current.GetUniqueInts(Size, 0, list.Count);
            IChromosome chromosome = (from c in list.Where((IChromosome c, int i) => randomIndexes.Contains((int)i)).OrderByDescending(c => c.Fitness)
                                      select c).FirstOrDefault();
            try
            {
                chromosome = (from c in list.Where((IChromosome c, int i) => randomIndexes.Contains(i))
                              orderby c.Fitness descending
                              select c).First();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during random index generation: {ex.Message}");
            }
            //    IChromosome chromosome = (from c in list.Where((IChromosome c, int i) => randomIndexes.Contains(i))
            //                              orderby c.Fitness descending
            //                              select c).First();
            list2.Add(chromosome.Clone());
            if (!AllowWinnerCompeteNextTournament)
            {
                list.Remove(chromosome);
            }
        }

        return list2;
    }
}
