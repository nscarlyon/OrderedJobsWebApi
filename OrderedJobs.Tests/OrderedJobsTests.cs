using NUnit.Framework;
using Moq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;
using OrderedJobs.Data;
using OrderedJobs.Web;
using OrderedJobs.Domain;
using System.Linq;

namespace OrderedJobs.Tests
{
    [TestFixture]
    public class OrderedJobsTests
    {
        [Test]
        public void OrderEmptyString()
        {
            Assert.AreEqual(Domain.OrderedJobs.GetOrderedJobs(""), "");
        }

        [Test]
        public void OrderIndependentJobs()
        {
            Assert.AreEqual(Domain.OrderedJobs.GetOrderedJobs("a-|" +
                                                       "b-|" +
                                                       "c-"), "abc");
        }

        [Test]
        public void OrderIndependentJobsWithOneDependentJob()
        {
            Assert.AreEqual(Domain.OrderedJobs.GetOrderedJobs("a-|" +
                                                       "b-c|" +
                                                       "c-"), "acb");
        }

        [Test]
        public void OrderIndependentAndDependentJobs()
        {
            Assert.AreEqual(Domain.OrderedJobs.GetOrderedJobs("a-|" +
                                                       "b-c|" +
                                                       "c-f|" +
                                                       "d-a|" +
                                                       "e-b|" +
                                                       "f-"), "afcdbe");
        }

        [Test]
        public void ReturnsErrorForSelfDependency()
        {
            Assert.AreEqual(Domain.OrderedJobs.GetOrderedJobs("a-|" +
                                                       "b-b|" +
                                                       "c-f|" +
                                                       "d-a|" +
                                                       "e-b|" +
                                                       "f-"), "A job cannot depend on itself.");
        }

        [Test]
        public void ReturnsErrorForCircularDependency()
        {
            Assert.AreEqual(Domain.OrderedJobs.GetOrderedJobs("a-|" +
                                                       "b-c|" +
                                                       "c-f|" +
                                                       "d-a|" +
                                                       "e-|" +
                                                       "f-b"), "Jobs cannot have circular dependency.");
        }
    }

    [TestFixture]
    public class PermutationTests
    {
        TestCase testCase = new TestCase();
        List<string> expectedPermutations = new List<string>();

        [SetUp]
        public void InitializeTest()
        {
            expectedPermutations.Clear();
        }

        [Test]
        public void OnePermutation()
        {
            testCase.Dependency = "a-";
            expectedPermutations.Add("a-");

            Assert.AreEqual(expectedPermutations, Permutations.GetPermutations(testCase));
        }

        [Test]
        public void TwoPermutations()
        {
            testCase.Dependency = "a-|b-a";
            expectedPermutations.Add("a-|b-a");
            expectedPermutations.Add("b-a|a-");

            Assert.AreEqual(expectedPermutations, Permutations.GetPermutations(testCase));
        }

        [Test]
        public void ThreePermutations()
        {
            testCase.Dependency = "a-|b-a|c-b";
            expectedPermutations.Add("a-|b-a|c-b");
            expectedPermutations.Add("b-a|a-|c-b");
            expectedPermutations.Add("c-b|a-|b-a");
            expectedPermutations.Add("a-|c-b|b-a");
            expectedPermutations.Add("b-a|c-b|a-");
            expectedPermutations.Add("c-b|b-a|a-");

            Assert.AreEqual(expectedPermutations, Permutations.GetPermutations(testCase));
        }
    }

    [TestFixture]
    public class MockTests
    {
        public static Mock<IMongoDatabase> mockTestCaseDatabase = new Mock<IMongoDatabase>();
        public static Mock<IHttpClient> mockOrderedJobsHttp = new Mock<IHttpClient>();
        public TestCasesHelper testCaseResults = new TestCasesHelper(mockTestCaseDatabase.Object, mockOrderedJobsHttp.Object);

        [SetUp]
        public void SetUp()
        {
            mockTestCaseDatabase.Setup(x => x.GetAllJobs()).Returns(() => new List<TestCase>
            {
              new TestCase {Dependency = "a-|b-a", Id = "1"}
            });
        }

        [Test]
        public void DuplicateJobs()
        {
            mockOrderedJobsHttp.Setup(mock => mock.GetAsync(It.IsAny<string>())).Returns(
            Task.FromResult(new HttpResponseMessage()
            {
                Content = new StringContent("bb")
            }));
            var testCases = testCaseResults.GetPassFailResults("fakeurl");

            Assert.AreEqual("Fail with ab, expected not to have multiple jobs of b", testCases.Results.ElementAt(0).Results.ElementAt(0).result);
        }

        [Test]
        public void WrongJob()
        {
            mockOrderedJobsHttp.Setup(mock => mock.GetAsync(It.IsAny<string>())).Returns(
            Task.FromResult(new HttpResponseMessage()
            {
                Content = new StringContent("ac")
            }));
            var testCases = testCaseResults.GetPassFailResults("fakeurl");

            Assert.AreEqual("Fail with ab, expected to have job b", testCases.Results.ElementAt(0).Results.ElementAt(0).result);
        }

        [Test]
        public void UnequalLengths()
        {
            mockOrderedJobsHttp.Setup(mock => mock.GetAsync(It.IsAny<string>())).Returns(
            Task.FromResult(new HttpResponseMessage()
            {
                Content = new StringContent("abc")
            }));
            var testCases = testCaseResults.GetPassFailResults("fakeurl");

            Assert.AreEqual("Fail with ab and abc, expected lengths to be equal.", testCases.Results.ElementAt(0).Results.ElementAt(0).result);
        }

        [Test]
        public void SameResponseAsOrderedJobs()
        {
            mockOrderedJobsHttp.Setup(mock => mock.GetAsync(It.IsAny<string>())).Returns(
            Task.FromResult(new HttpResponseMessage()
            {
                Content = new StringContent("ab")
            }));
            var testCases = testCaseResults.GetPassFailResults("fakeurl");

            Assert.AreEqual("ab", Domain.OrderedJobs.GetOrderedJobs("a-|b-a"));
            Assert.AreEqual("Pass", testCases.Results.ElementAt(0).Results.ElementAt(0).result);
        }


        [Test]
        public void CircularDependency()
        {
            mockTestCaseDatabase.Setup(x => x.GetAllJobs()).Returns(() => new List<TestCase>
            {
              new TestCase {Dependency = "a-|b-c|c-e|d-a|e-b", Id = "1"}
            });
            mockOrderedJobsHttp.Setup(mock => mock.GetAsync(It.IsAny<string>())).Returns(
            Task.FromResult(new HttpResponseMessage()
            {
                Content = new StringContent("Jobs cannot have circular dependency.")
            }));
            var testCases = testCaseResults.GetPassFailResults("fakeurl");

            Assert.AreEqual("Pass", testCases.Results.ElementAt(0).Results.ElementAt(0).result);
        }

        [Test]
        public void SelfDependency()
        {
            mockTestCaseDatabase.Setup(x => x.GetAllJobs()).Returns(() => new List<TestCase>
            {
              new TestCase {Dependency = "a-a", Id = "1"}
            });
            mockOrderedJobsHttp.Setup(mock => mock.GetAsync(It.IsAny<string>())).Returns(
            Task.FromResult(new HttpResponseMessage()
            {
                Content = new StringContent("A job cannot depend on itself.")
            }));
            var testCases = testCaseResults.GetPassFailResults("fakeurl");

            Assert.AreEqual("Pass", testCases.Results.ElementAt(0).Results.ElementAt(0).result);
        }

        [Test]
        public void NoJobs()
        {
            mockTestCaseDatabase.Setup(x => x.GetAllJobs()).Returns(() => new List<TestCase>
            {
              new TestCase {Dependency = "", Id = "1"}
            });
            mockOrderedJobsHttp.Setup(mock => mock.GetAsync(It.IsAny<string>())).Returns(
            Task.FromResult(new HttpResponseMessage()
            {
                Content = new StringContent("")
            }));
            var testCases = testCaseResults.GetPassFailResults("fakeurl");

            Assert.AreEqual("Pass", testCases.Results.ElementAt(0).Results.ElementAt(0).result);
        }

        [Test]
        public void DependentJobsOutOfOrder()
        {
            mockOrderedJobsHttp.Setup(mock => mock.GetAsync(It.IsAny<string>())).Returns(
            Task.FromResult(new HttpResponseMessage()
            {
                Content = new StringContent("ba")
            }));
            var testCases = testCaseResults.GetPassFailResults("fakeurl");

            Assert.AreEqual("Fail with ab, expected job a to come before b", testCases.Results.ElementAt(0).Results.ElementAt(0).result);
        }

        [Test]
        public void ValidIndependentJobsOutofOrder()
        {
            mockTestCaseDatabase.Setup(x => x.GetAllJobs()).Returns(() => new List<TestCase>
            {
              new TestCase {Dependency = "a|b|c", Id = "1"}
            });

            mockOrderedJobsHttp.Setup(mock => mock.GetAsync(It.IsAny<string>())).Returns(
            Task.FromResult(new HttpResponseMessage()
            {
                Content = new StringContent("cab")
            }));

            var testCases = testCaseResults.GetPassFailResults("fakeurl");

            Assert.AreEqual("abc", Domain.OrderedJobs.GetOrderedJobs("a|b|c"));
            Assert.AreEqual("Pass", testCases.Results.ElementAt(0).Results.ElementAt(0).result);
        }

        [Test]
        public void ValidIndependentAndDependentJobsOutofOrder()
        {
            mockTestCaseDatabase.Setup(x => x.GetAllJobs()).Returns(() => new List<TestCase>
            {
              new TestCase {Dependency = "a|b|c-b", Id = "1"}
            });

            mockOrderedJobsHttp.Setup(mock => mock.GetAsync(It.IsAny<string>())).Returns(
            Task.FromResult(new HttpResponseMessage()
            {
                Content = new StringContent("bac")
            }));

            var testCases = testCaseResults.GetPassFailResults("fakeurl");

            Assert.AreEqual("abc", Domain.OrderedJobs.GetOrderedJobs("a|b|c"));
            Assert.AreEqual("Pass", testCases.Results.ElementAt(0).Results.ElementAt(0).result);
        }

        [Test]
        public void FailMovesUpTestCases()
        {
            mockTestCaseDatabase.Setup(x => x.GetAllJobs()).Returns(() => new List<TestCase>
            {
              new TestCase {Dependency = "a-|b-a", Id = "1"},
              new TestCase {Dependency = "a-", Id = "2"},
            });

            mockOrderedJobsHttp.Setup(mock => mock.GetAsync(It.IsAny<string>())).Returns(
            Task.FromResult(new HttpResponseMessage()
            {
                Content = new StringContent("ab")
            }));

            var testCases = testCaseResults.GetPassFailResults("fakeurl");

            Assert.AreEqual("Fail", testCases.Result);
            Assert.AreEqual("Pass", testCases.Results.ElementAt(0).Result);
            Assert.AreEqual("Fail", testCases.Results.ElementAt(1).Result);
        }
    }
}
