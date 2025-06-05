using System.Net.Http.Json;
using System.Text.Json;
using MA_GA.Models;
using Microsoft.Extensions.Logging;

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
                dataObjectCenter.ReadGraph();
            }
            else
            {
                Console.WriteLine("DataObjects is null");
            }

        }

    }
}