# myLINQDemo
I released another project to my portfolio http://www.katiegirl.net/projects.htm

My LINQ Demo: 

A simple windows console application that demonstrates some very complex LINQ (Language-Integrated Query) code where you can query an XML file (as an embedded resource) at the parent level and descendant nodes and their attributes to display the output to the console. The program also allows you to read the entire XML file on the console screen as a reference. The sample XML is a listing of approximately 100+ food items and their nutrient compositions from the USDA Food Composition Databases. 

Comments: I chose the USDA Food Composition Databases as an XML data source and only a tiny percentage of the database for this project. You can download the data to XML or consume through Web API (see WEB API for project examples). This code may help you in your XML LINQ coding challenges as a reference. 

Samples: 
project XML File downloaded from USDA Food Composition Databases
Program.cs (Main Program)
Pictures: 
Main Menu
Read Database
Query Database
Application: myLINQDemo_APP.zip
Visual Studio Solution: myLINQDemo_VSS.zip 

Need LINQ code to Query XML file ?

Solution 

Here is some sample XML

<report sr="28" groups="All groups" subset="All foods" end="150" start="0" total="8489">
	<foods>
		<food ndbno="09427" name="Abiyuch, raw" weight="114.0" measure="0.5 cup">
			<nutrients>
				<nutrient nutrient_id="208" nutrient="Energy" unit="kcal" value="79" gm="69.0"/>
				<nutrient nutrient_id="269" nutrient="Sugars, total" unit="g" value="9.75" gm="8.55"/>
				<nutrient nutrient_id="204" nutrient="Total lipid (fat)" unit="g" value="0.11" gm="0.1"/>
				<nutrient nutrient_id="205" nutrient="Carbohydrate, by difference" unit="g" value="20.06" gm="17.6"/>
			</nutrients>
		</food>
		</foods>
</reports>

Goal: I want to query by food attribute name (example: beer) and get only the nutrient listing for that item that I want to see (ex: sugar). So give me the list of food items that have beer in the name and what their sugar value is.

LINQ Code


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




Screen Output


Enter a food name (example: Alcohol):
beer
Enter a nutrient name (example: Sugar:
sug
Name: Alcoholic beverage, beer, light, Nutrient: Sugars, total: 0.03
Name: Alcoholic beverage, beer, light, BUD LIGHT, Nutrient: Sugars, total: --
Name: Alcoholic beverage, beer, light, BUDWEISER SELECT, Nutrient: Sugars, total: --
Name: Alcoholic beverage, beer, light, higher alcohol, Nutrient: Sugars, total: 0.32
Name: Alcoholic beverage, beer, light, low carb, Nutrient: Sugars, total: --
Name: Alcoholic beverage, beer, regular, all, Nutrient: Sugars, total: --
Name: Alcoholic beverage, beer, regular, BUDWEISER, Nutrient: Sugars, total: --
Name: Alcoholic beverage, malt beer, hard lemonade, Nutrient: Sugars, total: 32.73
Name: Alcoholic beverages, beer, higher alcohol, Nutrient: Sugars, total: --

