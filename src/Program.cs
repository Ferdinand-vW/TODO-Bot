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
        static object writeLock = new object();
        static void Main(string[] args)
        {
            Bot bot = new Bot();
            
            Task t = new Task(() => ShowReminders(bot));
            t.Start();
            
            while (true)
            {
                string userInput = Console.ReadLine();
                string[] keywords = userInput.Split(new string[] { " " }, 2, StringSplitOptions.None);
                Commands(bot, keywords);
            }
        }

        public static void Commands(Bot bot, string[] keywords)
        {
            switch (keywords[0].ToLower())
            {
                case "add":
                    //Console.WriteLine("Keyword was {0}, rest of the note was {1}", keywords[0], keywords[1]);
                    bot.AddNote(keywords[1]);
                    break;
                case "ls":
                    ShowNotes(bot.Idents, bot.Notes);
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

        public static void ShowNotes(List<int> ids, Dictionary<int,string> notes)
        {
            WriteOutput(() => 
            {
                Console.WriteLine("Notes:");
                for(int i = 1; i <= ids.Count; i++)
                {
                    Console.Write(i + ") ");
                    Console.WriteLine(notes[i - 1]);
                }
            });
        }
        
        public static void ShowReminders(Bot bot)
        {
            while(true)
            {
                List<string> reminders = bot.GetRemindersBlocking(); //Waits until there are new reminders
                WriteOutput(() =>
                {
                    Console.WriteLine("This is a reminder for the following notes:");
                    reminders.ForEach(x => Console.WriteLine(" " + x));
                });
            }
        }
        
        //Simple locking mechanism to ensure that no output can't be written by
        //multiple threads at the same time
        public static void WriteOutput(Action action)
        {
            lock(writeLock)
            {
                action.Invoke();
            }
        }

    }
 } 

