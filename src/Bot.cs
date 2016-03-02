using System;
using System.Collections.Generic;

namespace TODOBot
{
    class Bot
    {
        public Dictionary<int, string> Notes { get; set; }
        public List<int> Idents { get; set; } //List that preservers insertion order of Notes
        int IdCounter = 0; //Unique Id for every Note. Only ever do incrementation when adding a Note

        public Bot()
        {
            Notes = new Dictionary<int, string>();
            Idents = new List<int>();
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
    }
}