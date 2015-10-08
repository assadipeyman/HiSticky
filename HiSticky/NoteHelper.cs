using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSticky
{
    public class NoteHelper
    {
        public static string serialize(NoteManager noteManager)
        {
            string ret = "";
            foreach (string note in noteManager.getNotes())
            {
                if (ret != "")
                    ret += noteManager.getSplitter();
                ret += note;
            }
            return ret;
        }
        public static List<string> deserialize(String hashedNote, string splitter)
        {
            List<string> notes = new List<string>();
            string[] noteArray = hashedNote.Split(new string[] { splitter }, StringSplitOptions.RemoveEmptyEntries);
            notes.Clear();
            foreach (String note in noteArray)
                notes.Add(note);
            return notes;
        }
    }
}
