using System;
using MA_GA.Models;
using QuikGraph;

namespace MA_GA.domain;

public static class GraphService
{

    public static AdjacencyGraph<IDataObject, IObjectRelation> CreateBidrectionalGraph()
    {
        var graph = new AdjacencyGraph<IDataObject, IObjectRelation>(
            allowParallelEdges: true
        );
        return graph;
    }

}
