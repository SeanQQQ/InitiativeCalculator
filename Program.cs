using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace InitCalcCLI6
{
    class Program
    {
        static Random r = new Random();
        static List<Character> PCs = new List<Character>();
        static string PCFileName;
        static bool PCsRoll;
        static bool autosave;

        static void Main(string[] args)
        {
            Console.WriteLine("Welcome To Initiative Calculator Version 6.00! \n");

            GetConfig();

            LoadPlayers();

            if(args.Length == 1)
            {
                Console.WriteLine("");

                RollInitiative(LoadEncounter(args[0]));

                Console.WriteLine("Press Enter to Close");

                return;
            }

            MainMenu();
        }

        private static void GetConfig()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory;

            StreamReader config = new StreamReader(path + "config.txt");
            
            PCsRoll = bool.Parse(config.ReadLine().Split(':')[1]);
            autosave = bool.Parse(config.ReadLine().Split(':')[1]);
            PCFileName = config.ReadLine().Split(':')[1];
        }

        private static void MainMenu()
        {
            for (ConsoleKey selection = new ConsoleKey(); selection != ConsoleKey.Escape; selection = Console.ReadKey().Key)
            {
                switch (selection)
                {
                    case ConsoleKey.D1:
                        Console.WriteLine("");
                        RollInitiative(CreateEncounter());
                        Console.ReadKey();
                        break;
                    case ConsoleKey.D2:
                        Console.WriteLine("");
                        RollInitiative(LoadEncounter());
                        Console.ReadKey();
                        break;
                    case ConsoleKey.D3:
                        Console.WriteLine("");
                        AddPlayer();
                        Console.ReadKey();
                        break;
                    case ConsoleKey.D4:
                        Console.WriteLine("");
                        DeletePlayer();
                        Console.ReadKey();
                        break;
                    case ConsoleKey.D5:
                        Console.WriteLine("");
                        PrintPlayers();
                        Console.WriteLine("");
                        Console.ReadKey();
                        break;
                    case ConsoleKey.D6:
                        Console.WriteLine("\n");
                        LoadPlayers();
                        Console.WriteLine("");
                        Console.ReadKey();
                        break;
                }

                Console.WriteLine("");
                Console.WriteLine("Please Select \n");
                Console.WriteLine("1. Roll New Initiative");
                Console.WriteLine("2. Load Encounter");
                Console.WriteLine("3. Add Players");
                Console.WriteLine("4. Remove Players");
                Console.WriteLine("5. See Current Players");
                Console.WriteLine("6. Reload Players From File");
                Console.WriteLine("Esc To Quit");
                Console.WriteLine("");

            }
        }

        private static void RollInitiative(List<Tuple<NonPlayerCharacter, int>> Encounter)
        {

            Console.Write("");

            List<Tuple<string, int>> battlefield = new List<Tuple<string, int>>();

            foreach(Character pc in PCs)
            {
                int roll;
                if (PCsRoll)
                {
                    Console.Write($"Enter {pc.name}'s Initiative Roll > ");
                    roll = int.Parse(Console.ReadLine());
                }else
                {
                    roll = Roll20(r);
                }
                battlefield.Add(new Tuple<string, int>(pc.name, roll));
            }

            Console.WriteLine("\n\n\n");

            foreach(var npctup in Encounter)
            {
                for (int i = 1; i <= npctup.Item2; i++)
                    battlefield.Add( new Tuple<string, int>(npctup.Item1.name + " #" + i, Roll20(r) + npctup.Item1.bonus) );
            }

            battlefield = battlefield.OrderByDescending(a => a.Item2).ToList();

            foreach(var combattent in battlefield)
            {
                Console.WriteLine(combattent.Item1 + $"\t ({combattent.Item2})");
            }

            if (autosave)
            {
                SaveInitiative(battlefield);
            }
            else
            {
                Console.WriteLine("\nSave Initiative Order ? (Y/N)");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                    SaveInitiative(battlefield);
                Console.WriteLine("\nPress Any Key to Return to Menu");
            }

        }

        private static void SaveInitiative(List<Tuple<string, int>> battlefield)
        {

            string path = AppDomain.CurrentDomain.BaseDirectory;


            string filename = DateTime.Now.ToString("yyMMddhhmm") + ".txt";

            StreamWriter file = new StreamWriter(File.Create(path + "/SavedInitiativeOrders/" + filename));

            foreach (var combattent in battlefield)
            {
                file.WriteLine(combattent.Item1 + $"\t ({combattent.Item2})");
            }

            file.Close();

            Process.Start("notepad.exe", path + "/SavedInitiativeOrders/" + filename);
        }

        private static void SaveEncounter(List<Tuple<NonPlayerCharacter, int>> encounter)
        {
            try
            {
                Console.Write("\n Enter New Battlefield File Name >");
                string filename = Console.ReadLine();

                filename = filename.Split('.')[0];
                filename += ".ect";

                string path = AppDomain.CurrentDomain.BaseDirectory;


                if (System.IO.File.Exists(path + "/SavedEncounters/" + filename))
                {
                    Console.WriteLine("You are about to overwrite " + filename + " Continue ? (Y/N)");
                    if (Console.ReadKey().Key != ConsoleKey.Y)
                        return;
                }
                Console.WriteLine("");


                StreamWriter file = new StreamWriter(File.Create("./SavedEncounters/" + filename));

                foreach (var npctup in encounter)
                {
                    file.WriteLine(npctup.Item1.name + "," + npctup.Item1.bonus + "," + npctup.Item2);
                }

                Console.WriteLine("Encounter Saved Successfully\n");

                file.Close();
            }catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static List<Tuple<NonPlayerCharacter, int>> LoadEncounter()
        {
            List<Tuple<NonPlayerCharacter, int>> encounter = new List<Tuple<NonPlayerCharacter, int>>();
            Console.Write("Enter Encounter File Name >");

            string filename = Console.ReadLine();
            filename = filename.Split('.')[0];

            foreach (string line in System.IO.File.ReadAllLines("./SavedEncounters/" + filename + ".ect")) {
                string[] linearr = line.Split(',');
                
                if(linearr.Length > 3)
                {
                    Console.WriteLine("Error: File Format Incorrect");
                }

                NonPlayerCharacter NPC = new NonPlayerCharacter(linearr[0], int.Parse(linearr[1]));

                encounter.Add(new Tuple<NonPlayerCharacter, int>(NPC, int.Parse(linearr[2])));
            }
            return encounter;
        }

        private static List<Tuple<NonPlayerCharacter, int>> LoadEncounter(string path)
        {
            List<Tuple<NonPlayerCharacter, int>> encounter = new List<Tuple<NonPlayerCharacter, int>>();

            foreach (string line in System.IO.File.ReadAllLines(path))
            {
                string[] linearr = line.Split(',');

                if (linearr.Length > 3)
                {
                    Console.WriteLine("Error: File Format Incorrect");
                }

                NonPlayerCharacter NPC = new NonPlayerCharacter(linearr[0], int.Parse(linearr[1]));

                encounter.Add(new Tuple<NonPlayerCharacter, int>(NPC, int.Parse(linearr[2])));
            }
            return encounter;
        }

        private static List<Tuple<NonPlayerCharacter, int>> CreateEncounter()
        {
            List<Tuple<NonPlayerCharacter, int>> encounter = new List<Tuple<NonPlayerCharacter, int>>();

            Console.Write("Enter Enemy Types Amount >");
            try
            {
                int ETAmt = int.Parse(Console.ReadLine());

                for (int i = 1; i <= ETAmt; i++)
                {
                    Console.Write($"Enter Enemy Type #{i} Name > ");
                    string EName = Console.ReadLine();
                    Console.Write($"Enter {EName} Bonus > ");
                    int Ebonus = int.Parse(Console.ReadLine());
                    Console.Write($"Enter Amount of {EName}(s) in Encounter > ");
                    int Amt = int.Parse(Console.ReadLine());

                    NonPlayerCharacter NPC = new NonPlayerCharacter(EName, Ebonus);

                    encounter.Add(new Tuple<NonPlayerCharacter, int>(NPC, Amt));
                }

                Console.WriteLine("Would you like to save this Encounter ? (Y/N)");
                if (Console.ReadKey().Key == ConsoleKey.Y)
                    SaveEncounter(encounter);

                return encounter;
            }
            catch (FormatException e) {
                Console.WriteLine("\n **Incorrect Input**");

                return null;
            }
            
        }

        private static void AddPlayer()
        {
            Console.Write("Enter Player Character Name >");
            string name = Console.ReadLine();
            PCs.Add(new Character(name));
            Console.WriteLine(name + " added");
        }

        private static void DeletePlayer()
        {
            Console.Write("Enter Player Character Name >");
            string name = Console.ReadLine();
            if (PCs.Remove( PCs.Where(pc => pc.name == name ).FirstOrDefault()))
                Console.WriteLine(name + " deleted");
            else
                Console.WriteLine(name + " not found");

        }

        private static void PrintPlayers()
        {
            Console.WriteLine("");
            foreach(Character Char in PCs)
            {
                Console.WriteLine(Char);
            }
        }

        private static int Roll20(Random r)
        {
            return r.Next(1, 21);
        }

        private static void LoadPlayers()
        {
            PCs.Clear();

            string path = AppDomain.CurrentDomain.BaseDirectory;
            
            foreach(string x in System.IO.File.ReadAllLines(path + "/Characters.txt"))
            {
                Character n = new Character(x);

                PCs.Add(n);
                Console.WriteLine(x + " Loaded!");
            }
        }
    }
}
