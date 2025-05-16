using System;
using MA_GA.Models.MappingModels;

namespace MA_GA.Models;

public class GraphObject
{
    public List<NodeObject>? externalComponents { get; set; }
    public List<NodeObject> informationObjects { get; set; }
    public List<NodeObject> functions { get; set; }
    public List<NodeObject>? actors { get; set; }
    public List<EdgeObject> relations { get; set; }




    public List<NodeObject> getInfoObjectList()
    {
        return informationObjects;
    }


}
