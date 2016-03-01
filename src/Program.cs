using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TODOBot
{

    /// <summary>
    /// 
    /// TODO List
    /// 
    /// 1) Pare to input apo ton xrhsth  DONE 
    /// 2) Kanto split se array me kena anamesa stis lekseis DONE 
    /// 3) Vale 'case' gia kathe keyword san array[1] DONE 
    /// 4) ???? 
    /// 5) Profit
    /// 
    /// </summary>
    /// 

    class Program
    {
        static void Main(string[] args)
        {
            Bot bot = new Bot();
            while (true)
            {
                string userInput = UserInput();
                string[] keywords = userInput.Split(new string[] { " " }, 2, StringSplitOptions.None);
                Commands(bot, keywords);
            }
        }

        public static void Commands(Bot bot, string[] keywords)
        {
            Dictionary<int,string> noteList = new Dictionary<int, string>();

            switch (keywords[0].ToLower())
            {
                case "add":
                    //Console.WriteLine("Keyword was {0}, rest of the note was {1}", keywords[0], keywords[1]);
                    bot.AddNote(keywords[1]);
                    break;
                case "ls":
                    ShowNotes(bot.Notes);
                    break;
                default:
                    Console.WriteLine("Unrecognizable command");
                    break;
            }
        }

        // User input recognition method
        public static string UserInput() // User Input
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("User> ");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            string input = Console.ReadLine();
            Console.ResetColor();

            return input;
        }

        public static void ShowNotes(Dictionary<int,string> notes)
        {
            Console.WriteLine("Notes:");
            foreach (KeyValuePair<int, string> kvp in notes)
            {
                Console.Write(kvp.Key + ") ");
                Console.WriteLine(kvp.Value);
            }
        }

    }



 } 

