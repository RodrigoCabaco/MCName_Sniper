using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;
using System.Web;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;

namespace NameMC_Sniper
{

    /***************************MADE BY JOIP****************************/
    class Program
    {
        static void Main(string[] args)
        {
            int availableNamesFound = 0;
            int requestsMade = 0;
            bool isCheckingAvailability = true;
            int secondsToSleep;
            Console.WriteLine("Started! Remember, everything is saved, to exit just press Ctrl + C! \nAlso the log will be apended so if you wish to generate complete new names please delete the log file!!! ");
            Console.WriteLine("All the names will be saved in the log.txt file");
            StreamWriter log;
            StreamWriter mostRecentAvailableNameLog;
            List<int> LowestAvailableDays = new List<int>();
            List<int> LowestAvailableHours = new List<int>();
            HtmlAgilityPack.HtmlWeb web = new HtmlWeb();
            Random random = new Random();
            HtmlAgilityPack.HtmlDocument doc;
            List<string> namesToSearch = new List<string>();
            char[] charTable = new char[] {'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'y', 'z', '_'
            , '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'};

            List<char> nameCreator;


            Console.WriteLine("Number of Letters in the usernames: ");
            Console.Write(">>");
            int numberOfLetters = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("Number of Names to search:");
            Console.Write(">>");
            int numberOfNames = Convert.ToInt32(Console.ReadLine());
            Console.Write(">>");
            Console.WriteLine("Time to sleep between 10 requests (Seconds) (Min: 1, Default: 3, Recommended: 5 or above): ");
            Console.Write(">>");
            try
            {
                secondsToSleep = Convert.ToInt32(Console.ReadLine());
            }
            catch
            {
                secondsToSleep = 3;
            }
            Console.Write("Do you want NameMc_Sniper to check the available time for \"Available Later\" Names? (Y/n): ");
            if (Console.ReadLine().ToLower() == "y")
            {
                isCheckingAvailability = true;
            }
            else if (Console.ReadLine() == "n")
            {
                isCheckingAvailability = false;
            }
            else
            {
                isCheckingAvailability = true;
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

                if (namesToSearch.Contains(name) == false)
                {
                    namesToSearch.Add(name);
                    if (namesToSearch.IndexOf(name) != 0)
                        Console.WriteLine("Generating Names:   -   " + namesToSearch.IndexOf(name) + " / " + numberOfNames);
                }
                else if (namesToSearch.Contains(name) == true)
                {
                    for (int i = 0; i < numberOfLetters; i++)
                    {
                        int toRemove = charTable.Length;
                        int randChar = random.Next(0, toRemove--);
                        nameCreator.Add(charTable[randChar]);
                    }
                    string name2 = new string(nameCreator.ToArray());
                    namesToSearch.Add(name);
                    if (namesToSearch.IndexOf(name) != 0)
                        Console.WriteLine("Generating Names:   -   " + namesToSearch.IndexOf(name2) + " / " + numberOfNames);
                }




            }


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Finished Name Generation, Starting...");
            foreach (var item in namesToSearch)
            {
                //make the request
                try
                {
                    var isAvailable = "none";
                    string searchQuery = item;
                    doc = web.Load("https://namemc.com/search?q=" + searchQuery);


                    //make sure the requests are not denied by sleeping between 11 requests
                    if (requestsMade >= 11 && requestsMade != 0)
                    {
                        requestsMade = 0;
                        System.Threading.Thread.Sleep(secondsToSleep * 1000);
                    }

                    //try and get the data
                    try
                    {
                        var getAvailability = doc.DocumentNode.SelectNodes("//*[@class = 'col-lg-7']");
                        if (getAvailability != null)
                        {
                            isAvailable = HttpUtility.HtmlDecode(getAvailability[0].NextSibling.NextSibling.InnerText);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("Unable to check due to request denied exception or invalid response... Retrying in 3 Seconds");
                            System.Threading.Thread.Sleep(3000);
                        }

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
                        Console.Clear();
                        Console.WriteLine("Testing item: " + item + ": unavailable " + "\nItems Tested: " + namesToSearch.IndexOf(item) + " / " + namesToSearch.Count);
                        Console.WriteLine($"Available Names With {numberOfLetters} Letters Found: {availableNamesFound}");
                    }
                    else if (isAvailable.ToLower().Contains("available later"))
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Clear();
                        Console.WriteLine("Testing item: " + item + ": available later " + " \nItems Tested: " + namesToSearch.IndexOf(item) + " / " + namesToSearch.Count);
                        Console.WriteLine($"Available Names With {numberOfLetters} Letters Found: {availableNamesFound}");

                        if (isCheckingAvailability)
                        {
                            ChromeOptions options = new ChromeOptions();
                            Console.WriteLine("Checking Availability Time for " + item + "...");

                            options.AddArgument("--log-level=OFF");
                            options.AddArgument("--silent");
                            options.AddArgument("--window-position=-32000,-32000");
                            ChromeDriver driver = new ChromeDriver(options);
                            driver.Navigate().GoToUrl("https://namemc.com/search?q=" + item);
                            Console.ForegroundColor = ConsoleColor.Yellow;
                            var timeAvailable = driver.FindElementByClassName("countdown-timer").GetAttribute("innerText");


                            if (LowestAvailableDays.Count <= 0)
                            {
                                mostRecentAvailableNameLog = File.CreateText("LowestAvailableTime.txt");
                                LowestAvailableDays.Add(Convert.ToInt32(timeAvailable.Split('d').First()));
                                LowestAvailableHours.Add(Convert.ToInt32(timeAvailable.Split('d').First()));
                                mostRecentAvailableNameLog.WriteLine(item + $" ({timeAvailable})");
                                mostRecentAvailableNameLog.Close();
                            }

                            driver.Close();
                            Console.Clear();
                            if (File.Exists("log.txt"))
                            {
                                log = File.AppendText("log.txt");
                                log.WriteLine(item + ": available later " + "  -  (https://namemc.com/search?q=" + item + ")" + " Time for Availability: " + timeAvailable);
                                log.Close();
                            }
                            else
                            {
                                log = File.CreateText("log.txt");
                                log.WriteLine(item + ": available later " + "  -  (https://namemc.com/search?q=" + item + ")" + " Time for Availability: " + timeAvailable);
                                log.Close();
                            }

                            if (LowestAvailableDays.Count < 0)
                            {
                                mostRecentAvailableNameLog = File.CreateText("LowestAvailableTime.txt");
                                foreach (var day in LowestAvailableDays)
                                {
                                    if (Convert.ToInt32(timeAvailable.Split('d').First()) >= day)
                                    {
                                        foreach (var hour in LowestAvailableHours)
                                        {
                                            if (Convert.ToInt32(timeAvailable.Split('h').First()) >= hour)
                                            {
                                                mostRecentAvailableNameLog.WriteLine(item + $" ({timeAvailable})");
                                                mostRecentAvailableNameLog.Close();
                                            }
                                        }
                                    }

                                }
                            }



                        }
                        else
                        {
                            if (File.Exists("log.txt"))
                            {
                                log = File.AppendText("log.txt");
                                log.WriteLine(item + ": available later " + "  -  (https://namemc.com/search?q=" + item + ")");
                                log.Close();
                            }
                            else
                            {
                                log = File.CreateText("log.txt");
                                log.WriteLine(item + ": available later " + "  -  (https://namemc.com/search?q=" + item + ")");
                                log.Close();
                            }
                        }


                    }
                    else if (isAvailable.ToLower().Contains("available"))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Clear();
                        Console.WriteLine("Testing item: " + item + ": available " + "\nItems Tested: " + namesToSearch.IndexOf(item) + " / " + namesToSearch.Count);
                        availableNamesFound++;
                        if (File.Exists("log.txt"))
                        {
                            log = File.AppendText("log.txt");
                        }
                        else
                        {
                            log = File.CreateText("log.txt");
                        }

                        log.WriteLine(item + ": available " + "  -  (https://namemc.com/search?q=" + item + ")");
                        log.Close();
                        Console.WriteLine($"Available Names With {numberOfLetters} Letters Found: {availableNamesFound}");

                    }

                }
                catch
                {

                }


            }





            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Finished!" + " Available Names Found: " + availableNamesFound);
            Console.ReadLine();
            Environment.Exit(1);
        }
    }
}
