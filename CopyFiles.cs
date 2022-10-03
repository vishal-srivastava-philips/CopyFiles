namespace CopyFiles
{
    public class Batch
    {
        public string Source { get; set; }
        public string Target { get; set; }
        public string[] Files { get; set; }
    }
    public class CopyFiles
    {
        public bool CreateBackup { get; set; }
        public Batch[] Batch { get; set; }
    }
}