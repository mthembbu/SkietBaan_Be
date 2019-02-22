using SkietbaanBE.Models;
using System;
using System.IO;
using System.Net;
using System.Threading;

namespace SkietbaanBE.Lib {
    public class ScheduleJob {

        private ModelsContext context;
        public ScheduleJob(ModelsContext context) {
            this.context = context;
        }

        private long GetMilliSecondsLeftToNextMonth() {
            //Date to first day of next month from now
            DateTime targetTime = new DateTime(DateTime.Today.Year, DateTime.Today.AddMonths(1).Month, 01, 0, 1, 0);
            DateTime currentTime = DateTime.Now;
            return (long)targetTime.Subtract(currentTime).TotalMilliseconds;
        }

        //NEW IDEA: WRITE A FUNCTION FOR RANKING USERS AFTER SCORE UPDATES. 
        //          IF THERE IS A NEW LEARDER, READ THE RECORDTRACKER DB OF THE PREVIOUS LEARDER AND CALCULATE 
        //          NUMBER OF DAYS SPENT AT THE TOP. IF THE NUMBER OF DAYS ARE > 31 (OR 93 - 3 MONTHS), ISSUE AWARD
        //          THEN OVERWRITE DATETIME OF THE PREVIOUS LEARDER IN DB WITH NEW LEADER'S CURRENT DATETIME

        //THE JOB SCHEDULER WILL BE USED FOR CHECKING IF IT IS END OF YEAR, THEN ISSUE AWARD FOR BEST PERFORMER OF THE YEAR
    }
}
