using System;
using System.Collections.Generic;

namespace TODOBot
{
    class Bot
    {
        public Dictionary<int, string> Notes { get; set; }
        int IdCounter = 0; //Unique Id for every Note. Only ever do incrementation when adding a Note

        public Bot()
        {
            Notes = new Dictionary<int, string>();
        }

        public void AddNote(string note)
        {
            Notes.Add(IdCounter, note);
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
    }
}