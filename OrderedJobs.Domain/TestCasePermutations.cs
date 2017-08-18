using System;
using System.Collections.Generic;
using OrderedJobs.Data;

namespace OrderedJobs.Domain
{
    public class Permutations
    {
        public static List<string> GetPermutations(TestCase testCase)
        {
            List<string> permutations = new List<string>();
            string[] Jobs = testCase.Dependency.Split('|');
            int NumOfJobs = Jobs.Length;
            GeneratePermutations(NumOfJobs, Jobs, permutations);
            return permutations;
        }

        private static void GeneratePermutations(int numOfJobs, string[] Jobs, List<string>permutations)
        {
            if (numOfJobs == 1)
            {
                permutations.Add(String.Join("|", Jobs));
            }
            else
            {
                for (int i = 0; i < numOfJobs - 1; i++)
                {
                    GeneratePermutations(numOfJobs - 1, Jobs, permutations);
                    Swap(numOfJobs % 2 == 0 ? i : 0, Jobs, numOfJobs);
                }
                GeneratePermutations(numOfJobs - 1, Jobs, permutations);
            }
        }

        private static void Swap(int i, string[] jobs, int numOfJobs)
        {
            string tmp = jobs[i];
            jobs[i] = jobs[numOfJobs - 1];
            jobs[numOfJobs - 1] = tmp;
        } 
    }
}
