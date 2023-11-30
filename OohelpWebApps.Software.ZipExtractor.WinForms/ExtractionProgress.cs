
namespace OohelpWebApps.Software.ZipExtractor
{
    public readonly struct ExtractionProgress
    {
        public string Operation { get; }
        public int Progress { get; }
        public ExtractionProgress(int progress, string operation)
        {
            this.Progress = progress;
            this.Operation = operation;
        }
        public ExtractionProgress(int progress) : this(progress, null) { }
        public ExtractionProgress(string operation) : this(-1, operation) { }
    }
}
