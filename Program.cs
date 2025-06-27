using System.Net.Http.Json;
using System.Text.Json;
using MA_GA.domain;
using MA_GA.domain.GreedyAlgorithm;
using MA_GA.Models;
using Microsoft.Extensions.Logging;
using QuikGraph;
using QuikGraph.Algorithms;
using QuikGraph.Algorithms.ConnectedComponents;

class MainApp
{
    static void Main(string[] args)
    {
        // logger
        using ILoggerFactory factory = LoggerFactory.Create(builder => builder.AddConsole());
        ILogger logger = factory.CreateLogger("Program");
        // define the path to the JSON file
        string filePath = "/home/danno/Documents/MA_Project/MA_GA/data/SmallTestcase.json";
        // object to hold the data
        Graph dataObjectCenter = new Graph();
        GraphObject rawObject;

        using (StreamReader sr = new StreamReader(filePath))
        {

            Console.WriteLine("Reading JSON file...");
            string json = sr.ReadToEnd();
            rawObject = JsonSerializer.Deserialize<GraphObject>(json);

            if (rawObject != null)
            {

                ObjectHelper.MapDataObjects(rawObject, dataObjectCenter, logger);
            }

            if (!dataObjectCenter.IsEmpty())
            {
                //  dataObjectCenter.ReadList();
                //  dataObjectCenter.AddConnectionToNodes();
                //  Console.WriteLine("Graph is not empty, connections added.");
                //  dataObjectCenter.ReadNodeConnection();
                //  Console.WriteLine("Connections read successfully.");
                //  dataObjectCenter.RemoveNodeFromGraph("PKPla"); // Example of removing a node
                //  Console.WriteLine("Node PKPla removed from graph.");
                //  dataObjectCenter.ReadList();
                //  Console.WriteLine("Graph after removal:");
                //  dataObjectCenter.ReadNodeConnection();
                
                logger.LogInformation("DataObjects are not empty, proceeding with graph displaying.");
                // dataObjectCenter.ReadGraph();
                var graph = dataObjectCenter.GetGraph();
                if (graph != null)
                {
                    DisplayGraphInfo(graph, logger);

                    void DisplayGraphInfo(AdjacencyGraph<DataObject, IObjectRelation> graph, ILogger logger)
                    {
                        // output edge count
                        Console.WriteLine($"Graph contains {graph.VertexCount} vertices and {graph.EdgeCount} edges.");
                        var algorithm = new GraphPartitionGreedyAlgorithm(graph);
                        algorithm.CreatePriorityList();
                        Console.WriteLine("Priority List created successfully. now partitioning the graph.");
                        // partition the graph
                        logger.LogInformation("Starting graph partitioning.");
                        var partitionResult = algorithm.PartitionGraph();
                        logger.LogInformation("Graph partitioning completed successfully.");
                        // output partition result
                        Console.WriteLine(GraphService.DiplayGraphByComponents(partitionResult));
                        // generate DOT representation of the graph

                      //  CreateClusteredGraphAndDisplay(partitionResult);
                    }

                }
                else
                {
                    logger.LogError("Graph is null after creation.");
                }

            }
            else
            {
                Console.WriteLine("DataObjects is null");
            }

        }

    }

    private static void CreateClusteredGraphAndDisplay(AdjacencyGraph<DataObject, IObjectRelation> partitionResult)
    {
        var newClusterGraph = GraphService.CreateClusteredGraph(partitionResult);
        Console.WriteLine(newClusterGraph.ClustersCount);
        var dotRepresentation = GraphService.GenerateClusteredGraphToDOT(newClusterGraph);
        Console.WriteLine("Graphviz DOT representation:");
        Console.WriteLine(dotRepresentation);
    }
}