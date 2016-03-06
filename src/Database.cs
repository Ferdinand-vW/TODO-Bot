using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SQLite;

namespace TODOBot
{
    class Database
    {
        SQLiteConnection conn;
        public Database()
        {
            conn = new SQLiteConnection("Data Source=notes.db");
        }
        
        public void Open()
        {
            conn.Open();
            InitDatabase();
        }
        
        public void Close()
        {
            conn.Close();
        }
        
        private void InitDatabase()
        {
            string sql = @"CREATE TABLE IF NOT EXISTS Notes (
                NoteID INTEGER PRIMARY KEY,
                Description VARCHAR(255) NOT NULL,
                Marked INTEGER NOT NULL
            );";
            SQLiteCommand comm = new SQLiteCommand(sql, conn);
            comm.ExecuteNonQuery();
        }
        
        public long InsertNote(string descr)
        {
            string sql = @"INSERT INTO Notes (Description, Marked) VALUES ($descr,0);";
            SQLiteCommand comm = new SQLiteCommand(sql, conn);
            comm.Parameters.AddWithValue("$descr",descr);
            comm.ExecuteNonQuery();
            return conn.LastInsertRowId;
        }
        
        public void DeleteNote(long noteID)
        {
            string sql = @"DELETE FROM Notes WHERE NoteID = $noteID";
            SQLiteCommand comm = new SQLiteCommand(sql, conn);
            comm.Parameters.AddWithValue("$noteID",noteID);
            comm.ExecuteNonQuery();
        }
        
        public void DeleteMarkedNotes()
        {
            string sql = @"DELETE FROM Notes WHERE Marked = 1";
            SQLiteCommand comm = new SQLiteCommand(sql, conn);
            comm.ExecuteNonQuery();
        }
        
        public void DeleteAllNotes()
        {
            string sql = @"DELETE FROM Notes";
            SQLiteCommand comm = new SQLiteCommand(sql, conn);
            comm.ExecuteNonQuery();
        }
        
        public void MarkNote(long noteID)
        {
            string sql = @"UPDATE Notes SET Marked = 1 WHERE NoteID = $noteID";
            SQLiteCommand comm = new SQLiteCommand(sql, conn);
            comm.Parameters.AddWithValue("$noteID",noteID);
            comm.ExecuteNonQuery();
        }
        
        public void EditNote(long noteID, string descr)
        {
            string sql = @"UPDATE Notes SET Description = $descr WHERE NoteID = $noteID";
            SQLiteCommand comm = new SQLiteCommand(sql, conn);
            comm.Parameters.AddWithValue("$descr",descr);
            comm.Parameters.AddWithValue("$noteID",noteID);
            comm.ExecuteNonQuery();
        }
        
        public Dictionary<long,string> SelectRemainingNotes()
        {
            string sql = @"SELECT NoteID,Description FROM Notes WHERE Marked = 0";
            SQLiteCommand comm = new SQLiteCommand(sql, conn);
            SQLiteDataReader reader = comm.ExecuteReader();
            
            Dictionary<long,string> notes = new Dictionary<long,string>();
            while(reader.Read())
            {
                long id = reader.GetInt64(0);
                string descr = reader.GetString(1);
                notes.Add(id,descr);
            }
            
            return notes;
        }
        
        public List<string> SelectMarkedNotes()
        {
            string sql = @"SELECT Description FROM Notes WHERE Marked = 1";
            SQLiteCommand comm = new SQLiteCommand(sql, conn);
            SQLiteDataReader reader = comm.ExecuteReader();
            
            List<string> notes = new List<string>();
            while(reader.Read())
            {
                notes.Add(reader.GetString(0));
            }
            
            return notes;
        }
    }
}