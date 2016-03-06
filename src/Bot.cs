using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

namespace TODOBot
{
    class Bot
    {
        ConcurrentBag<string> Reminders { get; set; }
        object locker_ = new object();
        bool newReminders = false;
        public ConcurrentDictionary<long, string> Notes { get; set; }
        public List<long> Idents { get; set; } //List that preservers insertion order of Notes
        TimeSpan remindTime = new TimeSpan(19,59,0); //Remind at 6PM
        
        public List<string> MarkedNotes { get; set; }
        
        Database db;
        Timer timer;

        
        public Bot()
        {
            Reminders = new ConcurrentBag<string>();
            Notes = new ConcurrentDictionary<long, string>();
            Idents = new List<long>();
            MarkedNotes = new List<string>();
            
            db = new Database();
            db.Open();
            FillNotes();
            StartReminding();
        }
        
        private void FillNotes()
        {
            MarkedNotes = db.SelectMarkedNotes();
            foreach(KeyValuePair<long,string> kvp in db.SelectRemainingNotes())
            {
                //Only one thread is ever inserting, so there's no need to check if we succeeded
                Notes.TryAdd(kvp.Key,kvp.Value);  
                Idents.Add(kvp.Key); 
            }
        }

        public void AddNote(string note)
        {
            long id = db.InsertNote(note);
            Notes.TryAdd(id, note);
            Idents.Add(id);
        }

        public bool RemoveNote(int num)
        {
            if (NumIsValid(num))
            {
                long id = Idents[num - 1];
                db.DeleteNote(id);
                string ignore;
                Notes.TryRemove(id, out ignore);
                Idents.RemoveAt(num - 1);
                
                return true;
            }
            
            return false;
        }

        public bool EditNote(int num, string descr)
        {
            if(NumIsValid(num))
            {
                long id = Idents[num - 1];
                db.EditNote(id,descr);
                Notes[id] = descr;
                
                return true;
            }
            
            return false;
        }

        public bool MarkNote(int num)
        {
            
            if(NumIsValid(num))
            {
                long id = Idents[num - 1];
                db.MarkNote(id);
                MarkedNotes.Add(Notes[id]);
                RemoveNote(num);
                
                return true;
            }
            
            return false;
        }
        
        private bool NumIsValid(int num)
        {
            return num <= Idents.Count && num > 0;
        }
        
        public void ClearMarkedNotes()
        {
            db.DeleteMarkedNotes();
            MarkedNotes.Clear();
        }
        
        public void Reset()
        {
            db.DeleteAllNotes();
            Notes.Clear();
            Idents.Clear();
            MarkedNotes.Clear();
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
            timer = new Timer(_ => Remind(), null, timeToGo, interval);
        }
        
        private void Remind()
        {
            lock(locker_)
            {
                //Clear any remaining reminders and add new ones
                Reminders = new ConcurrentBag<string>();
                foreach(string val in Notes.Values)
                {
                    Reminders.Add(val);
                } 
                
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
            return new List<string>(Reminders.ToArray());
        }
        
        public void Shutdown()
        {
            //Close connection to database
            db.Close();
            //Shutdown timer thread
            timer.Change(Timeout.Infinite,Timeout.Infinite);
        }
    }
}