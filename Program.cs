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

    /**********************MADE BY JOIP********************************/
    class Program
    {
        static void Main(string[] args)
        {
            int availableNamesFound = 0;
            int requestsMade = 0;
            int secondsToSleep;
            bool isLogged = false;
            string username;
            string password;
            Console.WriteLine("Started! Remember, everything is saved, to exit just press Ctrl + C! \n Also the log will be apended so if you wish to generate complete new names please delete the log file!!! ");
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
            /*

            Console.WriteLine("Do you want Name Sniper to auto test the names for you? (Y/n)");
            if (Console.ReadLine().ToLower() == "y")
            {
                isLogged = true;
                Console.Write("Please Enter Your Minecraft Username or E-Mail:  ");
                username = Console.ReadLine();
                Console.Write("Please Enter Your Minecraft Password (we will not store this data):  ");
                password = Console.ReadLine();
                ChromeDriver driver = new ChromeDriver();
                driver.Navigate().GoToUrl("https://www.minecraft.net/pt-pt/login");
                driver.FindElementByXPath("//*[@id='CoreAppsApp']/main/div/div/a").Click();
                driver.FindElementById("email").SendKeys(username);
                driver.FindElementById("password").SendKeys(password + Keys.Enter);

            }
            else
            {
                isLogged = false;
            }*/
            

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
                        { 
                        isAvailable = HttpUtility.HtmlDecode(getAvailability[0].NextSibling.NextSibling.InnerText);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                            Console.WriteLine("Unable to check due to request denied exception or invalid response");
                            System.Threading.Thread.Sleep(5000);
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
                        Console.WriteLine(item + ": unavailable");
                    }else if(isAvailable.ToLower().Contains("available later"))
                    {
                        var getTime = doc.DocumentNode.SelectNodes("//*[@class = 'mb-3']");
                        var availableTime = HttpUtility.HtmlDecode(getTime[0].SelectSingleNode("//time[@class='text-nowrap']").InnerHtml);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine(item + " : available later ");
                        if (File.Exists("log.txt"))
                        {
                            log = File.CreateText("log.txt");
                        }
                        else
                        {
                            log = File.CreateText("log.txt");
                        }
                        log.WriteLine(item + ": available later " + "  -  (https://namemc.com/search?q=" + item + ")");
                        log.Close();
                    }
                    else if (isAvailable.ToLower().Contains("available"))
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine(item + ": available");
                        availableNamesFound++;
                        if (File.Exists("log.txt"))
                        {
                            log = File.CreateText("log.txt");
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
            Console.WriteLine("Finished, press enter to exit");
            Console.ReadLine();
        }
    }
}
