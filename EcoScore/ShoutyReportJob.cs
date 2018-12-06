using System.IO;
using System.Collections.Generic;
using System.Xml;

class ShoutyReportJob
{
    public static void Main(string[] args)
    {
        var path = args[0];
        var mileageClaims = ReadMileageClaims(path);
        var statsService = CreateStatsService();

        var job = new ShoutyReportProcessor(mileageClaims, statsService);
        WriteEcoStatsReport(job.Process());
    }

    private static IList<MileageClaim> ReadMileageClaims(string claimsPath)
    {
        var mileageClaims = new List<MileageClaim>();
        using (var reader = new StreamReader(claimsPath))
        {
            while (!reader.EndOfStream)
            {
                var values = reader.ReadLine().Split(',');
                mileageClaims.Add(new MileageClaim(values[0], int.Parse(values[1]), int.Parse(values[2])));
            }
        }
        return mileageClaims;
    }

    private static void WriteEcoStatsReport(IList<EcoStat> stats)
    {
        XmlDocument reportXml = new XmlDocument();
        reportXml.LoadXml("<?xml version='1.0' encoding='UTF-8' ?><ecoReport/>");

        foreach (var stat in stats)
        {
            XmlElement node = reportXml.CreateElement(string.Empty, "ecoStat", string.Empty);
            node.SetAttribute("SalespersonName", stat.SalespersonName);
            node.SetAttribute("RevenuePerMile", System.Convert.ToString(stat.RevenuePerMile));
            reportXml.DocumentElement.AppendChild(node);
        }
 
        reportXml.Save("report.xml");
    }

    private static IStatsService CreateStatsService()
    {
        if (System.Environment.GetEnvironmentVariable("FAKE_INITIALISATION_DATA") != null)
            return new FakeStatsService(System.Environment.GetEnvironmentVariable("FAKE_INITIALISATION_DATA"));

        return new ProductionStatsService();
    }
}
