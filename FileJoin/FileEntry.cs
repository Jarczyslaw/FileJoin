using System.IO;

namespace FileJoin
{
    public class FileEntry
    {
        public string FullPath { get; set; }
        public string FileName => Path.GetFileName(FullPath);
        public string FilePath => Path.GetDirectoryName(FullPath);
    }
}