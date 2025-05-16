using System.Net.Http.Json;
using System.Text.Json;
using MA_GA.Models;

class MainApp
{
    static void Main(string[] args)
    {
        string filePath = "/home/danno/Documents/MA_Project/MA_GA/data/SmallTestcase.json";
        DataObjects dataObjectCenter = new DataObjects();
        GraphObject rawObject;
        Console.WriteLine(File.Exists(filePath));

        using (StreamReader sr = new StreamReader(filePath))
        {

            Console.WriteLine("Reading JSON file...");
            string json = sr.ReadToEnd();
            rawObject = JsonSerializer.Deserialize<GraphObject>(json);

            if (rawObject != null)
            {

                Console.WriteLine("Raw object array is not null");


                if (rawObject.informationObjects != null)
                {
                    foreach (var item in rawObject.informationObjects)
                    {
                        dataObjectCenter.AddDataObject(new DataObject(
                            item.name,
                            ObjectType.InformationObject,
                            item.nameShort,
                            item.externalComponent
                        ));
                    }
                }
                else
                {
                    Console.WriteLine("InformationObjects is null");
                }


            }

            if (!dataObjectCenter.IsEmpty())
            {
                dataObjectCenter.readList();
            }
            else
            {
                Console.WriteLine("DataObjects is null");
            }





        }



    }
}