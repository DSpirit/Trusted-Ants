using System.Collections.Generic;
using Assets.Scripts.Ant;
using Assets.Scripts.Jobs;
using UnityEngine;

namespace Assets.Scripts.Manager
{
    public class NewsManager
    {
        public List<Event> Events = new List<Event>();

        public void JobsVerified(Job j, bool success)
        {
            var e = new Event();
            e.Ant = j.JobContract.Debtor.AntType.ToString();
            e.Action = success ? "finished job for" : "failed finishing job for";
            e.Description = "Job Verification";
            e.Partner = j.JobContract.Debtor.AntType.ToString();
            var formattedTime = GlobalFunctions.GetTime();
            e.Time = formattedTime;
            Events.Add(e);
        }

        public void JobDistributed(AntMeeting m, Job j)
        {
            var e = new Event();
            e.Ant = m.Submitter.AntType.ToString();
            e.Action = "Distribution";
            e.Description = "Distribution Accepted";
            e.Partner = m.Worker.AntType.ToString();
            var formattedTime = GlobalFunctions.GetTime();
            e.Time = formattedTime;
            Events.Add(e);
        }

        public void NotDistributed(AntMeeting m)
        {
            var ev = new Event();
            ev.Ant = m.Submitter.AntType.ToString();
            ev.Action = "declined distribution to";
            ev.Partner = m.Worker.AntType.ToString();
            ev.Description = "Job preserved";
            ev.Time = GlobalFunctions.GetTime();
            Events.Add(ev);
        }

        public void NotAccepted(AntMeeting m)
        {
            var ev = new Event();
            ev.Ant = m.Worker.AntType.ToString();
            ev.Action = "declined job from";
            ev.Partner = m.Submitter.AntType.ToString();
            ev.Description = "Job not accepted";
            ev.Time = GlobalFunctions.GetTime();
            Events.Add(ev);
        }
    }

    public class Event
    {
        public string Ant;
        public string Action;
        public string Partner;
        public string Description;
        public string Time;
    }
}
