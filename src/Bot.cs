/*
Copyright 2016 Ferdinand-vW VaggelisD
TODO-Bot is distributed under the terms of the GNU General Public License v3.0
*/

using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading;

namespace TODOBot
{
    class Bot
    {
        ConcurrentQueue<string> Reminders { get; set; }
        object locker_ = new object();
        bool newReminders = false;
        public ConcurrentDictionary<long, string> Notes { get; set; }
        public List<long> Idents { get; set; } //List that preservers insertion order of Notes
        
        public List<string> MarkedNotes { get; set; }
        
        Database db;
        Timer timer;

        
        public Bot()
        {
            Reminders = new ConcurrentQueue<string>();
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

            TimeSpan interval = new TimeSpan(0,0,20); //Remind every 20 seconds after remindTime
                                                      //Normally we don't set the remind time this low
                                                      //But otherwise it's a bit difficult 
            //Asynchronously wait until it is time, then call Remind() and keep calling it after interval amount of time
            timer = new Timer(_ => Remind(), null, interval, interval);
        }
        
        private void Remind()
        {
            lock(locker_)
            {
                //Clear any remaining reminders and add new ones
                Reminders = new ConcurrentQueue<string>();
                foreach(long key in Idents)
                {
                    Reminders.Enqueue(Notes[key]);
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