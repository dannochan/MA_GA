using System.Net.Http.Json;
using MA_GA.Models;

class MainApp
{
    static void Main(string[] args)
    {
        List<IDataObject> dataObjects = new List<IDataObject>();

        using (StreamReader sr = new StreamReader("/dat/SmallTestcase.json"))
        {
            string json = sr.ReadToEnd();

        }
        

        
    }
}