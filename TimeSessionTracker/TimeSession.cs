using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
