using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Threading;

namespace TODOBot
{
    class Bot
    {
        List<string> Reminders { get; set; }
        object locker_ = new object();
        bool newReminders = false;
        public Dictionary<int, string> Notes { get; set; }
        public List<int> Idents { get; set; } //List that preservers insertion order of Notes
        int IdCounter = 0; //Unique Id for every Note. Only ever do incrementation when adding a Note
        TimeSpan remindTime = new TimeSpan(19,59,0); //Remind at 6PM
        
        public Bot()
        {
            Reminders = new List<string>();
            Notes = new Dictionary<int, string>();
            Idents = new List<int>();
            Console.WriteLine(remindTime.ToString());
            
            StartReminding();
        }

        public void AddNote(string note)
        {
            Notes.Add(IdCounter, note);
            Idents.Add(IdCounter);
            IdCounter++;
        }

        public void RemoveNote(int id)
        {

        }

        public void EditNote(int id)
        {

        }

        public void MarkNote(int id)
        {

        }
        
        public void StartReminding()
        {
            DateTime current = DateTime.Now;
            TimeSpan timeToGo = remindTime - current.TimeOfDay;
            TimeSpan interval = new TimeSpan(0,0,10); //Remind every 10 minutes after remindTime
            while(timeToGo.TotalSeconds < 0)
            {
                timeToGo = timeToGo.Add(interval); //Determine how much time is left until the next reminder
            }
            
            //Asynchronously wait until it is time, then call Remind() and keep calling it after interval amount of time
            Timer timer = new Timer(_ => Remind(), null, timeToGo, interval);
        }
        
        private void Remind()
        {
            lock(locker_)
            {
                Reminders.Clear(); //Clear any remaining reminders and add new ones
                Reminders.AddRange(Notes.Values);
                
                if(Reminders.Count > 0) //If there are reminders
                {
                    newReminders = true; //Tell any waiting thread that there are new reminders
                    Monitor.PulseAll(locker_);
                }
            }
        }
        
        //Blocking function that gets all generated reminders. If there aren't any
        //then wait until there are
        public List<string> GetRemindersBlocking()
        {
            lock(locker_)
            {
                while(newReminders != true) //We have to make sure that we wait for a pulse
                {
                    Monitor.Wait(locker_); //Release the lock and sleep until we receive a pulse
                }
                newReminders = false;
            }
            return Reminders;
        }
    }
}