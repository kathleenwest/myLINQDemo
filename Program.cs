using System;
using System.IO;
using System.Reflection;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace myLinqDemo
{
	class Program
	{

        // Comments: Global Fields for Program Class
        // Static and Private for Program Class
        // Did not use properties per design choice
        
        private static bool play;                   // Sentinal for Program Loop
        private static StringBuilder menu;          // Holds Menu String Object
        private static string choice;               // User Menu Choice
        private static XDocument myXMLdata = null;  // For LINQ Query of XML
        private static XmlTextWriter writer = null; // to read the file pretty on the screen

        static void Main(string[] args)
		{
            Task runDemo = Task.Run(() => RunDemo());
            Task.WaitAll(runDemo);
		}


        #region RunDemo
        private static void RunDemo()
        {
            try
            {              
                // Generate Main Menu StringBuilder Object
                menu = CreateMenu();

                // Load the XML Document into Memory
                LoadData();

                // Set the entry for the program flow
                play = true;

                #region ConsoleAttributes
                Console.Title = "LINQ Demo - USDA Food Composition Database";
                Console.BackgroundColor = ConsoleColor.DarkBlue;
                Console.ForegroundColor = ConsoleColor.White;
                #endregion

                while (play)
                {
                    choice = String.Empty;

                    // Clear the Screen
                    Console.Clear();

                    // Write Menu
                    Console.WriteLine(menu);

                    // User Input
                    #region UserInput
                    while (choice == String.Empty)
                    {
                        // Read User Input
                        choice = Console.ReadLine();

                        switch (choice.ToUpper())
                        {
                            // Read Database
                            case "A":
                                ReadDatabase();
                                break;
                            // Add to Database
                            // LINQ Query
                            case "B":
                                SearchLINQ();
                                break;
                            // Exit the Program
                            case "X":
                                play = false;
                                return;
                            default:
                                choice = String.Empty;
                                break;
                        }
                    }
                    #endregion

                    Console.ReadLine();
                }
            }

            catch (Exception ex)
            {
                // Catches a General Exception
                Console.WriteLine("General Error Message. Check inner exception for additional details.");
                Console.WriteLine("There has been an error and exception: {0}, {1}, {2}", ex.Message, ex.StackTrace, ex.InnerException.Message);
                Console.WriteLine("The Demo program has terminated. Press any key");
                Console.Read();
            }

            finally

            {
                // Nothing Particular I want to go here 
            }

        }
        #endregion


        #region LoadXMLData
        private static void LoadData()
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            using (Stream stm = asm.GetManifestResourceStream("myLinqDemo.usda.xml"))
            {
                if(stm != null)
                {
                    myXMLdata = XDocument.Load(stm);
                }
                else
                {
                    throw new Exception("The project resource could not be found. Make sure you referenced the Resource by namespace.filename AND set the file properties to Embedded Resource. If you put the resource in a sub folder you need to reference the namespace.folder.filename");
                }             
            }
        }
        #endregion


        #region SearchLINQ
        private static void SearchLINQ()
        {
            string search = String.Empty;
            string nutrientname = String.Empty;

            try
            {
                Console.WriteLine(new string('=', 20));
                Console.WriteLine("LINQ Query: USDA Food Composition Database");
                Console.WriteLine(new string('=', 20));

                while (String.IsNullOrEmpty(search))
                {
                    Console.WriteLine("Enter a food name (example: Alcohol):");
                    search = Console.ReadLine();
                }


                while (String.IsNullOrEmpty(nutrientname))
                {
                    Console.WriteLine("Enter a nutrient name (example: Sugar:");
                    nutrientname = Console.ReadLine();
                }

                var query = from food in myXMLdata.Descendants("food")
                            where food.Attribute("name").Value.ToLower().Contains(search.ToLower())                           
                            where food.Descendants("nutrient").Count((nutrientElement => nutrientElement.Attribute("nutrient").Value.ToLower().Contains(nutrientname.ToLower()))) > 0
                            select new
                            {
                                Name = food.Attribute("name").Value ?? "",
                                NutrientValue = food.Descendants("nutrient").FirstOrDefault(nutrientElement => nutrientElement.Attribute("nutrient").Value.ToLower().Contains(nutrientname.ToLower())).Attribute("value").Value,
                                NutrientName = food.Descendants("nutrient").FirstOrDefault(nutrientElement => nutrientElement.Attribute("nutrient").Value.ToLower().Contains(nutrientname.ToLower())).Attribute("nutrient").Value
                            };
                
                var first = query.FirstOrDefault();
                if (first != null)
                {
                    foreach (var item in query)
                    {
                        Console.WriteLine("Name: {0}, Nutrient: {1}: {2}", item.Name, item.NutrientName, item.NutrientValue);
                    }
                }
                else
                {
                    Console.WriteLine("Sorry your search has no results. Please search again.");
                }

            }

            catch (Exception ex)
            {
                // Catches a General Exception
                Console.WriteLine("Database Query Error.");
                Console.WriteLine("There has been an error and exception: {0}, {1}", ex.Message, ex.StackTrace);
                Console.WriteLine("The request has been terminated. Press any key");
                Console.Read();
            }

            finally
            {
                // Nothing I want to do here yet
            }
        }
        #endregion


        #region ReadDatabase
        private static void ReadDatabase()
        {
            try
            {
                // READ Database    
                Console.WriteLine(new string('=', 20));
                Console.WriteLine("READ Database: USDA Food Composition Database");
                Console.WriteLine(new string('=', 20));

                    if (myXMLdata != null)
                    {                      
                        writer = new XmlTextWriter(Console.Out);
                        writer.Formatting = Formatting.Indented;
                        myXMLdata.WriteTo(writer);
                        writer.Flush();
                        Console.WriteLine();
                    }
                    else
                    {
                        throw new Exception("The project resource could not be found. Make sure you referenced the Resource by namespace.filename AND set the file properties to Embedded Resource. If you put the resource in a sub folder you need to reference the namespace.folder.filename");
                    }
                }

            catch (Exception ex)
            {
                // Catches a General Exception
                Console.WriteLine("Database Read Error Message. Check XML Document");
                Console.WriteLine("There has been an error and exception: {0}, {1}", ex.Message, ex.StackTrace);
                Console.WriteLine("The request has been terminated. Press any key");
                Console.Read();

            }

            finally
            {
                // Nothing I want to add here yet
            }
        }
        #endregion


        #region CreateMainMenu
        private static StringBuilder CreateMenu()
        {
            // Creates the Main Menu
            menu = new StringBuilder();
            menu.AppendLine("|============================================|");
            menu.AppendLine("|...............LINQ Demo....................|");
            menu.AppendLine("|============================================|");
            menu.AppendLine("|.......USDA Food Composition Database.......|");
            menu.AppendLine("|____________________________________________|");
            menu.AppendLine("| A.)...Read from Database (30 sec output)...|");
            menu.AppendLine("| B.)...Query Database (LINQ)................|");
            menu.AppendLine("| X.)...Exit.................................|");
            menu.AppendLine("|____________________________________________|");
            return menu;
        }
        #endregion
    }
}
