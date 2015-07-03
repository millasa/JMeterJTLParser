using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System.Xml.XPath;

namespace cs_unique1
{
    class URLStats
    {
        private List<double> loadTimes_ = new List<double>();

        public URLStats()
        {
        }

        public void AddLoadTime(double time)
        {
            loadTimes_.Add(time);
        }

        public string GetLoadTime()
        {
            double sum = 0.0;
            double max = 0.0;
            double min = 0.0;

            foreach (double loadTime in loadTimes_)
            {
                if (loadTime > max)
                    max = loadTime;
                if (min == 0 || loadTime < min)
                    min = loadTime;
                sum += loadTime;
            }
            double average = sum / loadTimes_.Count;

            string values = string.Format("{0} \t {1} \t {2}", average, min, max);

            return values;
        }
    }

    class TestStats
    {
        private Dictionary<string, URLStats> statsByName_ = new Dictionary<string, URLStats>();

        public string[] GetURLs()
        {
            return statsByName_.Keys.ToArray<string>();
        }

        public URLStats GetStatsForURL(string url)
        {
            if (!statsByName_.ContainsKey(url))
            {
                statsByName_[url] = new URLStats();
            }
            return statsByName_[url];
        }

        public void AddStats(string url, double loadTime)
        {
            var urlStats = GetStatsForURL(url);
            urlStats.AddLoadTime(loadTime);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("Run as: app.exe test_name build_version");
                return;
            }

            string testName = args[0];
            string buildVersion = args[1];

            string testFileName = testName + ".jtl";

            XDocument doc = XDocument.Load(testFileName);

            var testStats = new TestStats();

            var query =
                from samp in doc.Root.XPathSelectElements("./httpSample")
                select new
                {
                    URL = samp.Attribute("lb").Value,
                    LoadTime = Convert.ToDouble(samp.Attribute("t").Value)
                };
            foreach (var xmlStat in query)
            {
                testStats.AddStats(xmlStat.URL, xmlStat.LoadTime);
            }

            string fileName = string.Format("{0}-{1}.txt", testName, buildVersion);

            string report = "";

            report = report + "URL \t Avg(ms) \t Min(ms) \t Max(ms) \n";

            foreach (string url in testStats.GetURLs())
            {
                report = report + string.Format("{0} \t {1} \n", url, testStats.GetStatsForURL(url).GetLoadTime());
                Console.WriteLine("'{0}': {1}", url, testStats.GetStatsForURL(url).GetLoadTime());
            }

            System.IO.File.WriteAllText(fileName, report);
        }
    }
}
