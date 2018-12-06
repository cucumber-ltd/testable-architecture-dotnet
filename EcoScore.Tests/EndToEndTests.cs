using NUnit.Framework;
using System.Diagnostics;
using System.IO;

[TestFixture]
public class EndToEndTests
{
    static string dir = Path.Combine(TestContext.CurrentContext.TestDirectory, "..", "..");
    //static string reportXml = Path.Combine(dir, "report.xml");
    static string exe = Path.Combine(dir, "..", "EcoScore", "bin", "Debug", "ShoutyReportJob.exe");

    [SetUp]
    public void cd() 
    {
        Directory.SetCurrentDirectory(dir);    
    }

    [Test]
    public void zero_mileage_claim()
    {
        // run report
        Run("test-case-3-input.csv");

        // check XML report has been generated
        Assert.IsTrue(File.Exists("report.xml"));
        Assert.AreEqual(
            File.ReadAllLines("test-case-3-output.xml"),
            File.ReadAllLines("report.xml")
        );
    }

    [Test]
    public void multiple_mileage_claims_from_salesperson()
    {
        // run report
        Run("test-case-4-input.csv");

        // check XML report has been generated
        Assert.IsTrue(File.Exists("report.xml"));
        Assert.AreEqual(
            File.ReadAllLines("test-case-4-output.xml"),
            File.ReadAllLines("report.xml")
        );
    }

    private void Run(string csvName)
    {
        string csv = Path.Combine(dir, csvName);
        string fileName;
        string args;
        if (System.Environment.OSVersion.Platform.ToString().Equals("Unix")) {
            fileName = "mono";
            args = exe + " " + csv;
        } 
        else 
        {
            fileName = exe;
            args = csv;
        }

        Process proc = new Process();
        proc.EnableRaisingEvents = false;
        proc.StartInfo.FileName = fileName;
        proc.StartInfo.Arguments = args;
        proc.StartInfo.EnvironmentVariables.Add("FAKE_INITIALISATION_DATA",
            "1,234.50 ; 19,123456.78 ; 22,123456.78 ; 57, 123456.78");
        proc.StartInfo.UseShellExecute = false;
        proc.Start();
        proc.WaitForExit();
    }
}
