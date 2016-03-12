/*
Copyright 2016 Ferdinand-vW VaggelisD
TODO-Bot is distributed under the terms of the GNU General Public License v3.0
*/

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
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
        static bool keepRunning = true;
        static void Main(string[] args)
        {
            Bot bot = new Bot();
            
            Task t = new Task(() => ShowReminders(bot));
            t.Start();
            
            while (keepRunning)
            {
                string userInput = UserInput();
                string[] keywords = userInput.Split(new char[] { ' ' });
                Commands(bot, keywords);
            }
            
            bot.Shutdown();
        }

        public static void Commands(Bot bot, string[] keywords)
        {
            switch (keywords[0].ToLower())
            {
                case "add":
                    bot.AddNote(AppendKeywords(1,keywords).Trim(new char[] {'"'}));
                    break;
                case "remove":
                    IfInputIsValidDo(keywords[1],new Func<int,bool>(bot.RemoveNote));
                    break;
                case "edit":
                    IfInputIsValidDo(keywords[1],x => 
                      bot.EditNote(x,AppendKeywords(2,keywords).Trim(new char[] {'"'})));
                    break;
                case "mark":
                    IfInputIsValidDo(keywords[1],new Func<int,bool>(bot.MarkNote));
                    break;
                case "notes":
                    ShowNotes(bot.Idents, bot.Notes);
                    break;
                case "markednotes":
                    ShowMarkedNotes(bot.MarkedNotes);
                    break;
                case "clear":
                    bot.ClearMarkedNotes();
                    break;
                case "reset":
                    bot.Reset();
                    break;
                case "help":
                    ShowHelpMessage();
                    break;
                case "shutdown":
                    keepRunning = false;
                    break;
                default:
                    WriteOutput(() => 
                    {
                        Console.WriteLine("Unrecognizable command. Type help to see a list of possible commands");
                    });
                    break;
            }
        }
        
        private static string AppendKeywords(int start,string[] keywords)
        {
            return String.Join(" ",keywords.Skip(start));
        }
        
        private static void IfInputIsValidDo(String keyword, Func<int,bool> func)
        {
            int num;
            if(Int32.TryParse(keyword, out num)) //First try to parse the input
            {
                if(!func.Invoke(num)) //Checks if the num is valid and if it is executes the function
                {
                    WriteOutput(() => Console.WriteLine("Input did not correspond to a Note"));
                }
            }
            else
            {
                WriteOutput(() => Console.WriteLine("Input was not of type Int"));
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

        public static void ShowNotes(List<long> ids, ConcurrentDictionary<long,string> notes)
        {
            WriteOutput(() => 
            {
                Console.WriteLine("Notes:");
                for(int i = 1; i <= ids.Count; i++)
                {
                    Console.Write(i + ") ");
                    Console.WriteLine(notes[ids[i - 1]]);
                }
            });
        }
        
        public static void ShowMarkedNotes(List<string> notes)
        {
            WriteOutput(() =>
            {
                Console.WriteLine("Marked Notes:");
                notes.ForEach(x => Console.WriteLine(" " + x));
            });
        }
        
        public static void ShowHelpMessage()
        {
            WriteOutput(() =>
            {
                Console.WriteLine("Possible commands: ");
                Console.WriteLine("  Add \"String\"      --Adds a Note to the Note List");
                Console.WriteLine("  Remove Int        --Removes Note from Note List");
                Console.WriteLine("  Edit Int \"String\" --Change the description");
                Console.WriteLine("  Mark Int          --Mark the Note as completed");
                Console.WriteLine("  Notes             --List all unfinished Notes");
                Console.WriteLine("  MarkedNotes       --List as completed Notes");
                Console.WriteLine("  Clear             --Remove all completed Notes");
                Console.WriteLine("  Reset             --Remove all data");
                Console.WriteLine("  Shutdown          --Shutsdown the bot");
            });
        }
        
        public static void ShowReminders(Bot bot)
        {
            while(keepRunning)
            {
                List<string> reminders = bot.GetRemindersBlocking(); //Waits until there are new reminders
                WriteOutput(() =>
                {
                    Console.WriteLine("This is a reminder for the following notes:");
                    reminders.ForEach(x => Console.WriteLine(" " + x));
                });
            }
        }
        
        //Simple locking mechanism to ensure that output can't be written by
        //multiple threads at the same time
        private static void WriteOutput(Action action)
        {
            lock(writeLock)
            {
                action.Invoke();
            }
        }
    }
 } 

