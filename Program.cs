using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Web;
using System.Diagnostics;
using System.IO;

namespace NameMC_Sniper
{
    class Program
    {
        static void Main(string[] args)
        {
            int availableNamesFound = 0;
            int requestsMade = 0;
            int secondsToSleep;
            Console.WriteLine("Started! Remember, everything is saved, to exit just press Ctrl + C! Also the log will be apended so if you wish to generate complete new names please delete the log file!!! ");
            Console.WriteLine("All the names will be saved in the log.txt file");
            StreamWriter log;
            HtmlAgilityPack.HtmlWeb web = new HtmlWeb();
            Random random = new Random();
            HtmlAgilityPack.HtmlDocument doc;
            List<string> namesToSearch = new List<string>();
            char[] charTable = new char[] {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'y', 'z', '_'
            , '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

            List<char> nameCreator;


            Console.WriteLine("Number of Letters in the usernames: ");
            int numberOfLetters = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Number of Names to search:");
            int numberOfNames = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Time to sleep between 10 requests (Seconds) (Min: 1, Default: 3): ");
            try
            {
               secondsToSleep = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                secondsToSleep = 3;
            }

            //create a list of strings based on the char table
            for (int x = 0; x < numberOfNames; x++)
            {
                nameCreator = new List<char>();

                for (int i = 0; i < numberOfLetters; i++)
                {
                    int toRemove = charTable.Length;
                    int randChar = random.Next(0, toRemove--);
                    nameCreator.Add(charTable[randChar]);
                }
                string name = new string(nameCreator.ToArray());

                if(namesToSearch.Contains(name) == false)
                {
                    namesToSearch.Add(name);
                    if(namesToSearch.IndexOf(name) != 0)
                    Console.WriteLine("Generating Names:   -   " + namesToSearch.IndexOf(name));
                }




            }
            namesToSearch.Add("b75");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Starting...");
            foreach (var item in namesToSearch)
            {
                //make the request
                try {
                    var isAvailable = "none";
                    string searchQuery = item;
                    doc = web.Load("https://namemc.com/search?q=" + searchQuery);


                    //make sure the requests are not denied by sleeping between 11 requests
                    if(requestsMade % 11 == 0 && requestsMade != 0)
                    {
                        System.Threading.Thread.Sleep(secondsToSleep * 1000);
                    }

                    //try and get the data
                    try
                    {
                        var getAvailability = doc.DocumentNode.SelectNodes("//*[@class = 'col-lg-7']");
                        if(getAvailability != null)
                        isAvailable = HttpUtility.HtmlDecode(getAvailability[0].NextSibling.NextSibling.InnerText);
                        requestsMade++;
                    }
                    catch
                    {
                        isAvailable = "none";
                    }




                    //check if its available and write to log
                    if (isAvailable.ToLower().Contains("unavailable"))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine(item + ": unavailable");
                    }else if(isAvailable.ToLower().Contains("available later"))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(item + " : available later");
                        if (File.Exists("log.txt"))
                        {
                            log = File.AppendText("log.txt");
                        }
                        else
                        {
                            log = File.CreateText("log.txt");
                        }
                        log.WriteLine(item + ": available later");
                        log.Close();
                    }
                    else if (isAvailable.ToLower().Contains("available"))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(item + ": available");
                        availableNamesFound++;
                        if (File.Exists("log.txt"))
                        {
                            log = File.AppendText("log.txt");
                        }
                        else
                        {
                            log = File.CreateText("log.txt");
                        }

                        log.WriteLine(item + ": available");
                        log.Close();
                        Console.WriteLine($"Available Names With {numberOfLetters} Letters Found: {availableNamesFound}");

                    }
                    
                }
                catch
                {
                    Console.WriteLine("unable to check");
                }


              }





            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Finished, press any key to exit");
            Console.ReadLine();
        }
    }
}
