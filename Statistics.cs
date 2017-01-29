//------------------------------------------------------------------------------
// Singleton Class to collect statiscts about Jobs
// Severin Wünsch
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Ant;
using Assets.Scripts.Jobs;
using UnityEngine;

namespace Assets.Scripts
{
    public class Statistics
    {
        private static Statistics _instance;
        public List<JobStatistics> JobStatisticsList = new List<JobStatistics> ();
        private Dictionary<JobPart, JobStatistics> _linksToJobStatistic = new Dictionary<JobPart, JobStatistics> ();
        public const float StartTime = 0;
        private Statistics ()
        {
            
        }

        public static Statistics Instance ()
        {
            return _instance ?? (_instance = new Statistics());
        }

        public void RegisterJob (Job job)
        {
            JobStatisticsList.Add (new JobStatistics (job));
        }
     
        public void FinishedJobPart (JobPart jobPart, bool successfully)
        {
            JobStatistics jobStat;
            _linksToJobStatistic.TryGetValue(jobPart, out jobStat);
            if (jobStat != null) jobStat.FinishedJobPart (jobPart, successfully);
        }

        public int RegisteredJobs ()
        {
            return JobStatisticsList.Count;
        }

        public int RegisteredJobParts ()
        {
            return JobStatisticsList.Sum(jobStatistics => jobStatistics.JobPartStatisticsDict.Count);
        }

        public int SuccessfullyFinishedJobs ()
        {
            return JobStatisticsList.Count(jobStatistics => jobStatistics.IsSuccessfullyFinished());
        }

        public int FinshedJobs ()
        {
            return JobStatisticsList.Count(jobStatistics => !jobStatistics.GetFinishTimeStamp().Equals(0.0f));
        }

        public List<Vector2> FinishedJobPartsPerTimeSpan (TimeSpan d, float start)
        {
            List<Vector2> result = new List<Vector2> ();
            Dictionary<int, int> resultDict = new Dictionary<int, int> ();
            foreach (JobStatistics jobStatistics in JobStatisticsList) {
                foreach (JobPartStatistics jobPartStatistics in 
                    jobStatistics.JobPartStatisticsDict.Values) {
                        float time = jobPartStatistics.FinishTimestamp;
                        // Exclude default value if not set
                        if (time > start && 
                            jobPartStatistics.Successfully) { 
                                TimeSpan t = TimeSpan.FromSeconds(time - start);
                                int tint = (int)t.TotalSeconds;
                                tint = tint - tint % (int)d.TotalSeconds;
                                //time = roundUp(time, d);
                                if (resultDict.ContainsKey (tint)) {
                                    resultDict [tint] = 
                                        resultDict [tint] + 1;
                                } else {
                                    resultDict [tint] = 1;
                                }
                            }
                    }
            }

            for (int key = 0; key <=(Time.time-start); 
                key += (int) d.TotalSeconds) {
                    int keyvalue = 0;
                    resultDict.TryGetValue (key, out keyvalue);
                    result.Add (new Vector2 (key, keyvalue));
                }

            return result;
		
        }

        public List<Vector2> FinishedJobsPerTimeSpan (TimeSpan d, float start)
        {
            List<Vector2> result = new List<Vector2> ();
            Dictionary<int, int> resultDict = new Dictionary<int, int> ();
            foreach (JobStatistics jobStatistics in JobStatisticsList) {
                bool sucess = true;
                float time = 0.0f;
                foreach (JobPartStatistics jobPartStatistics 
                    in jobStatistics.JobPartStatisticsDict.Values) {
                        if (!jobPartStatistics.Successfully) {
                            sucess = false;
                            break;
                        }
                        if (jobPartStatistics.FinishTimestamp > time) {
                            time = jobPartStatistics.FinishTimestamp;
                        }
                    }
                if (!sucess) {
                    continue;
                }
                if (!(time > start)) continue;
                TimeSpan t = TimeSpan.FromSeconds(time - start);
                int tint = (int)t.TotalSeconds;
                tint = tint - tint % (int)d.TotalSeconds;
                if (resultDict.ContainsKey (tint)) {
                    resultDict [tint] = resultDict [tint] + 1;
                } else {
                    resultDict [tint] = 1;
                }
            }

            for (int key = 0; key <=(Time.time-start);
                key += (int) d.TotalSeconds) {
                    int keyvalue = 0;
                    resultDict.TryGetValue (key, out keyvalue);
                    //TimeSpan t = (entry.Key - startime);
                    result.Add (new Vector2 (key, keyvalue));
                }


            return result;
		
        }

        public List<Vector2> FailedJobsPerTimeSpan (TimeSpan d, float start)
        {
            List<Vector2> result = new List<Vector2> ();
            Dictionary<int, int> resultDict = new Dictionary<int, int> ();
            foreach (JobStatistics jobStatistics in JobStatisticsList) {
                bool success = true;
                float time = 0.0f;
                foreach (JobPartStatistics jobPartStatistics in 
                    jobStatistics.JobPartStatisticsDict.Values) {
                        if (jobPartStatistics.FinishTimestamp > time) {
                            time = jobPartStatistics.FinishTimestamp;
                        }
                        if (jobPartStatistics.Successfully || jobPartStatistics.FinishTimestamp == 0.0f)
                            continue;
                        success = false;
                        break;
                    }
                if (success) {
                    continue;
                }
                if (time > start) {
                    TimeSpan t = TimeSpan.FromSeconds(time - start);
                    int tint = (int)t.TotalSeconds;
                    tint = tint - tint % (int)d.TotalSeconds;
                    //time = roundUp(time, d);
                    if (resultDict.ContainsKey (tint)) {
                        resultDict [tint] = resultDict [tint] + 1;
                    } else {
                        resultDict [tint] = 1;
                    }
                }
            }
            for (int key = 0; key <= (Time.time-start); 
                key += (int) d.TotalSeconds) {
                    int keyvalue = 0;
                    resultDict.TryGetValue (key, out keyvalue);
                    result.Add (new Vector2 (key, keyvalue));
                }
		
            return result;
        }

	
        // Reseting all Saved statistics. This function should only be called
        // if all jobs get deleted to avoid exceptions
        public void ResetStatistics ()
        {
            JobStatisticsList = new List<JobStatistics> ();
            _linksToJobStatistic = new Dictionary<JobPart, JobStatistics> ();
        }


        public class JobStatistics
        {
            public Guid Id;
            public float StartedAt;
            public float FinishedAt;
            public List<JobPartStatistics> Parts = new List<JobPartStatistics>();
            private Job _job;
            public readonly Dictionary<JobPart, JobPartStatistics> JobPartStatisticsDict = new Dictionary<JobPart, JobPartStatistics> ();

            public JobStatistics (Job job)
            {
                Id = job.Id;
                this._job = job;
                StartedAt = job.StartTimestamp;
            }

            public void RegisterJobPart(JobPart jobPart)
            {
                var partStats = new JobPartStatistics(jobPart);
                JobPartStatisticsDict.Add(jobPart, partStats);
                Parts.Add(partStats);
                Instance()._linksToJobStatistic.Add(jobPart, this);
            }

            public void FinishedJobPart (JobPart jobPart, bool successfully)
            {
                JobPartStatisticsDict[jobPart].StartTimestamp = jobPart.StartTimestamp;
                JobPartStatisticsDict[jobPart].FinishTimestamp = jobPart.FinishTimeStamp;
                JobPartStatisticsDict[jobPart].Successfully = successfully;
                GetFinishTimeStamp();
            }

            public bool IsSuccessfullyFinished ()
            {
                bool finished = JobPartStatisticsDict.Values.All(jobPartStatistics => jobPartStatistics.Successfully);
                return finished;
            }

            // Returns DateTime of last ended JobPart or DateTime.MinValue
            // if not alls Jobparts are finished
            // Function does not return if all JobParts where 
            // finshed successfully
            public float GetFinishTimeStamp ()
            {
                float lastTime = 0.0f;
                foreach (JobPartStatistics jobPartStatistics in JobPartStatisticsDict.Values) {
                    if (jobPartStatistics.FinishTimestamp == 0) {
                        return 0.0f;
                    } 
                    if (jobPartStatistics.FinishTimestamp > lastTime) {
                        lastTime = jobPartStatistics.FinishTimestamp;
                    }
                }
                FinishedAt = lastTime;
                return lastTime;
            }
        }

        public class JobPartStatistics
        {
            public Guid Id;
            public float StartTimestamp;
            public float FinishTimestamp;
            public bool Successfully;
            private JobPart _jobPart;

            public JobPartStatistics (JobPart jobPart)
            {
                Id = jobPart.Id;
                StartTimestamp = jobPart.StartTimestamp;
                this._jobPart = jobPart;
            }

        }

    }
}

