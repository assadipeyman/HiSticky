using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiSticky
{
    public class NoteFileManager
    {
        private String path;
        private String fileName;
        public NoteFileManager(string path, string fileName)
        {
            this.path = path;
            this.fileName = fileName;
        }

        public string getData()
        {
            StreamWriter sr;
            if (!File.Exists(getFilePath()))
            {
                sr = File.CreateText(getFilePath());
                sr.Close();
            }
            return File.ReadAllText(getFilePath());
        }

        public void writeFile(string notes)
        {
            StreamWriter sr;
            if (!File.Exists(getFilePath()))
            {
                sr = File.CreateText(getFilePath());
                sr.Close();
            }
            File.WriteAllText(getFilePath(), notes);
        }
        public string getFilePath()
        {
            return path + "/" + fileName;
        }
    }
}
