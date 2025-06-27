using System;
using System.Dynamic;

namespace MA_GA.domain.module;

/// <summary>
/// Module class represents a bundle of modularisable elements. 
/// Indices are used to keep track of the modularisable elements in the module and is sorted in ascending order.
/// The HashSet IndicesSet is used to quickly check if an index (modularisable element) exists in the module or if an element contains an index, this is only for utility performance.
/// The last indext in the LinkedList is considered the ending node of the module.
/// </summary>

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

            for (int i = this.Indices.Count - 1; i >= 1; i--)
            {
                if (!this.Indices.Contains(index))
                {
                    var currentIndex = (int)this.Indices.ElementAt(i);
                    var previousIndex = (int)this.Indices.ElementAt(i - 1);
                    if (currentIndex > index && previousIndex < index)
                    {
                        Indices.AddBefore(Indices.Find(currentIndex), index);
                        break;
                    }
                }
            }

        }
    }

    public void AddIndices(HashSet<object> indices)
    {
        foreach (var index in indices)
        {
            AddIndex((int)index);
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



    public bool CheckIndexInModule(int index)
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

    public List<object> GetIndices()
    {
        return Indices.ToList();
    }
}
