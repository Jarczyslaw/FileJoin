using JToolbox.WPF.Core.Base;
using System.IO;

namespace FileJoin
{
    public class FileEntry : BaseViewModel
    {
        private int number;

        public FileEntry()
        {
        }

        public FileEntry(string fullPath)
        {
            FullPath = fullPath;
        }

        public int Number
        {
            get => number;
            set => Set(ref number, value);
        }

        public string FullPath { get; set; }
        public string FileName => Path.GetFileName(FullPath);
        public string FilePath => Path.GetDirectoryName(FullPath);
    }
}