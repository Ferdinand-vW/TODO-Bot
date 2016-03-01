using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace W.C.TODOBot
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
            while (true)
            {
                string userInput = UInput();

                string[] ui = userInput.Split(new string[] { " " }, 2, StringSplitOptions.None);


                Commands(ui);
            }
            Console.ReadKey();
        }





        public static void Commands(string[] keyword)
        {

            Dictionary<int,string> noteList = new Dictionary<int, string>();



            switch (keyword[0].ToLower())
            {

                case "add":

                    Console.WriteLine("Keyword was {0}, rest of the note was {1}", keyword[0], keyword[1]);
                    Console.ReadKey();

                    break;

            }

        }

        // User input recognition method
        public static string UInput() // User Input
        {

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("User> ");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Yellow;
            string input = Console.ReadLine();
            Console.ResetColor();


            return input;
        }

    }



 } 

