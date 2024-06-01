namespace TimeSessionTracker
{
    internal class TimeSession
    {
        internal int Id {  get; set; }
        internal DateTime Date { get; set; }
        internal DateTime StartTime { get; set; }
        internal DateTime EndTime { get; set; }
        internal TimeSpan SessionDuration { get; set; }
    }
}
