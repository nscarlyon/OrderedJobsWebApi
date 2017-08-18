using System.Linq;

namespace OrderedJobs.Domain
{
    public class VerifiedJobs
    {
        private static string _result;
        private static string _correctOrderedJobs;
        private static string _unverifiedHttpResponse;

        public static string GetVerifiedResult(string permutation, string unverifiedHttpResponse)
        {
            _unverifiedHttpResponse = unverifiedHttpResponse;
            _correctOrderedJobs = OrderedJobs.GetOrderedJobs(permutation);
            _result = "Pass";

            if (IsSame()) return _result;
            if (LengthsNotEqual()) return _result;
            if (DuplicateJobs()) return _result;
            if (WrongJob()) return _result;
            if (DependentJobsOutOfOrder(permutation)) return _result;
            return "Pass";
        }

        private static bool IsSame()
        {
            return _unverifiedHttpResponse == _correctOrderedJobs;
        }

        private static bool LengthsNotEqual()
        {
            if (_unverifiedHttpResponse.Length != _correctOrderedJobs.Length) {
                _result = "Fail with " + _correctOrderedJobs + " and " + _unverifiedHttpResponse + ", expected lengths to be equal.";
                return true;
            } 
            return false;
        }

        private static bool DuplicateJobs()
        {
            foreach(var job in _unverifiedHttpResponse)
            {
                if ((_unverifiedHttpResponse.Count(x => x == job)) > 1) {
                    _result = "Fail with " + _correctOrderedJobs + ", expected not to have multiple jobs of " + job;
                    return true;
                } 
            }
            return false;
        }

        private static bool WrongJob()
        {
            foreach(var job in _correctOrderedJobs)
            {
                if(_unverifiedHttpResponse.IndexOf(job) == -1)
                {
                    _result = "Fail with " + _correctOrderedJobs + ", expected to have job " + job;
                    return true;
                }
            }
            return false;
        }

        private static bool DependentJobsOutOfOrder(string permutation)
        {
            var jobs = permutation.Split('|');
            var dependentJobs = jobs.Where(job => job.Length == 3);

            foreach (var job in dependentJobs)
            {
                if(_unverifiedHttpResponse.IndexOf(job[0]) < _unverifiedHttpResponse.IndexOf(job[2]))
                {
                    _result = "Fail with " + _correctOrderedJobs + ", expected job " + job[2] + " to come before " + job[0];
                    return true;
                }
            }
            return false;
        }
    }
}
