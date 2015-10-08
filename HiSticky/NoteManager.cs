using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSticky
{
    public class NoteManager
    {
        private List<String> notes;
        private String splitter = "";
        public NoteManager(String splitter) {
            notes = new List<string>();
            this.splitter = splitter; 
        }

        public NoteManager(String splitter, List<String> noteList)
            : this(splitter)
        {
            foreach (String note in noteList)
                this.notes.Add(note);
        }
        public int addNote(String note)
        {
            notes.Add(note.Replace(splitter, "-"));
            return notes.Count;
        }
        public void updateNote(int item, string note)
        {
            notes[item - 1] = note;
        }
        public void deleteNote(int item)
        {
            notes.RemoveAt(item-1);
        }

        public bool isNoteExist(int item)
        {
            return (notes.Count > item-1);
        }
        public string getNote(int item)
        {
            if (isNoteExist(item))
                return notes[item-1];
            else
                return null;
        }
        public List<string> getNotes()
        {
            return notes;
        }
        public string getSplitter()
        {
            return splitter;
        }
    }
}
