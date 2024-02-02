namespace InfoGatherer.api.Configuration
{
    public class HangfireConfig
    {
        public bool Enabled { get; set; }
        public TaskStatus[] Tasks { get; set; }
    }
    public class TaskStatus
    {
        public string Name { get; set; }
        public bool IntervalExecution { get; set; }
        public bool Daily { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
    }
}
