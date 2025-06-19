using System;
using System.Dynamic;

namespace MA_GA.domain.module;

public class Module
{

    private readonly LinkedList<object> Indices;

    private readonly HashSet<object> IndexSet;


    public Module()
    {
        Indices = new LinkedList<object>();
        IndexSet = new HashSet<object>();
    }

    public void AddIndex(int index)
    {
        this.IndexSet.Add(index);

        if (Indices.Count == 0)
        {
            Indices.AddFirst(index);
        }
        else if (Indices.Last() is int lastIndex && lastIndex < index)
        {
            Indices.AddLast(index);
        }
        else if (Indices.First() is int firstIndex && firstIndex > index)
        {
            Indices.AddFirst(index);
        }
        else
        {
            var indexList = Indices.ToList();
            for (int i = 1; i < indexList.Count; i++)
            {
                if (!indexList.Contains(index))
                {
                    var currentIndex = (int)indexList[i];
                    var previousIndex = (int)indexList[i - 1];
                    if (currentIndex > index && previousIndex < index)
                    {
                        Indices.AddBefore(Indices.Find(currentIndex), index);
                        break;
                    }
                }
            }

        }
    }

    public void AddIndices(HashSet<int> indices)
    { 
        foreach (var index in indices)
        {
            AddIndex(index);
        }
    }

    public void RemoveIndex(int index)
    {
        if (!this.IndexSet.Contains(index))
        {
            throw new ArgumentException($"Index {index} does not exist in the module.");
        }

        var node = Indices.Find(index);
        if (node != null)
        {
            Indices.Remove(node);
        }
        
         // Remove from the HashSet
        this.IndexSet.Remove(index);
     }



    public Boolean ContainsIndex(int index)
    {
        return IndexSet.Contains(index);
    }

    public int GetAlleleOfEndingNode()
    {
        return Indices.Last?.Value is int lastIndex ? lastIndex : throw new InvalidOperationException("Module is empty, no ending node found.");
    }


    public override bool Equals(object? obj)
    {
        return obj is Module module &&
               EqualityComparer<LinkedList<object>>.Default.Equals(Indices, module.Indices) &&
               EqualityComparer<HashSet<object>>.Default.Equals(IndexSet, module.IndexSet);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Indices, IndexSet);
    }
}
