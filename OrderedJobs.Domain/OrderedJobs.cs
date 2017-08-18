using System.Collections.Generic;
using System.Linq;

namespace OrderedJobs.Domain
{
    public class OrderedJobs
    {
        public static string OrderedJobsList;

        public static string GetOrderedJobs(string jobs)
        {
            if (jobs == "") return "";
            var createdJobs = jobs.Split('|')
                .Select(job => new Job(job))
                .ToArray();

            if (AJobDependsOnItself(createdJobs)) return "A job cannot depend on itself.";

            return CanOrderJobs(createdJobs)
                ? OrderedJobsList
                : "Jobs cannot have circular dependency.";
        }

        private static bool AJobDependsOnItself(IEnumerable<Job> createdJobs)
        {
            return createdJobs.Any(job => job.Dependent == job.Dependency);
        }

        private static bool CanOrderJobs(Job[] createdJobs)
        {
            OrderIndependentJobs(createdJobs);
            OrderDependentJobs(createdJobs);
            return OrderedJobsList.Length == createdJobs.Length;
        }

        private static void OrderIndependentJobs(IEnumerable<Job> jobs)
        {
            OrderedJobsList = jobs.Where(job => !job.HasDependency())
                .Aggregate("", (acc, job) => acc + job.Dependent);
        }

        private static void OrderDependentJobs(IEnumerable<Job> createdJobs)
        {
            var dependentJobs = createdJobs.Where(job => job.HasDependency());
            var jobsAreBeingAdded = true;

            while (jobsAreBeingAdded)
            {
                var countOfJobsOrderedBeforeLastPass = OrderedJobsList.Length;
                foreach (var job in dependentJobs)
                {
                    AddNextJob(job);
                }
                jobsAreBeingAdded = JobsWereAdded(countOfJobsOrderedBeforeLastPass);
            }
        }

        private static bool JobsWereAdded(int jobsAdded)
        {
            return jobsAdded != OrderedJobsList.Length;
        }

        private static void AddNextJob(Job job)
        {
            if (OrderedJobsList.IndexOf(job.Dependent) == -1 && OrderedJobsList.IndexOf(job.Dependency) >= 0)
                OrderedJobsList += job.Dependent;
        }
    }

    internal class Job
    {
        public char Dependency;
        public char Dependent;

        public Job(string jobString)
        {
            Dependent = jobString[0];
            Dependency = jobString.Length == 3 ? jobString[2] : '0';
        }

        public bool HasDependency()
        {
            return Dependency != '0';
        }
    }
}
