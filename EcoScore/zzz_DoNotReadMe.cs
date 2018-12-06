// DO NOT LOOK IN THIS FILE
// DO NOT LOOK IN THIS FILE
// DO NOT LOOK IN THIS FILE
// DO NOT LOOK IN THIS FILE























































using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;


public class ShoutyStatsService
{
    private static Random rnd = new Random();

    private int latestEcoStatsMonth;
    private int latestEcoStatsYear;
    private IDictionary<string, IDictionary<string, float>> ecoStatsStore
        = new Dictionary<string, IDictionary<string, float>>();


    private readonly Dictionary<int, decimal> revenueByCustomerId
        = new Dictionary<int, decimal>
        {
            {1, 234.50m},
            {19, 123456.78m},
            {22, 123456.78m},
            {57, 123456.78m}
        };

    public ShoutyStatsService()
    {
        if (System.Environment.GetEnvironmentVariable("VOLATILE_STATS_DATA") != null)
        {
            revenueByCustomerId = new Dictionary<int, decimal>();
            for (int i=0; i<10; i++)
            {
                revenueByCustomerId[rnd.Next(1, 1000)]
                     = (decimal)rnd.Next(0, 99999999)/100m;
            }
        }
    }

    public string GetRevenueForCustomer(int customerID)
    {
        CheckServiceConnection();
        if (!revenueByCustomerId.ContainsKey(customerID))
            throw new ShoutyStatsServiceException("No customer found with ID '" + customerID.ToString() + "'");

        return "<CustomerStats id=\"" +
            customerID.ToString() +
            "\" revenue=\"" +
            revenueByCustomerId[customerID] +
            "\"/>";
    }

    public string GetCustomerIDs()
    {
        var result = new StringBuilder();
        result.Append("<Customers>");
        foreach (int id in revenueByCustomerId.Keys)
        {
            result.Append("<Customer id=\"");
            result.Append(id.ToString());
            result.Append("\"/>");
        }
        result.Append("</Customers>");

        return result.ToString();
    }

    public string IsValidCustomer(string customerXml)
    {
        var doc = new XmlDocument();
        doc.LoadXml(customerXml);
        int id = int.Parse(doc.DocumentElement.Attributes["id"].Value);

        if (revenueByCustomerId.ContainsKey(id))
        {
            return "<booleanResponse result=\"TRUE\" />";
        }
        else
        {
            return "<booleanResponse result=\"FALSE\" />";
        }
    }

    public string GetLatestEcoStatsDate()
    {
        return "<LatestEcoStatsDate year=\"" +
            latestEcoStatsYear.ToString("D4") +
            "\" month=\"" +
            latestEcoStatsMonth.ToString("D2")  +
            "\" />";
    }

    public void SetEcoStats(string ecoStatsXml)
    {
        var doc = new XmlDocument();
        doc.LoadXml(ecoStatsXml);
        int month = int.Parse(doc.DocumentElement.Attributes["month"].Value);
        int year = int.Parse(doc.DocumentElement.Attributes["year"].Value);

        string key = CreateKey(year, month);

        if (year < latestEcoStatsYear ||
                (year == latestEcoStatsYear && month < latestEcoStatsMonth))
        {
            throw new ShoutyStatsServiceException("EcoStats for a later month have already been set");
        }

        StoreEcoStats(key, doc.DocumentElement);
        latestEcoStatsYear = year;
        latestEcoStatsMonth = month;
    }

    public string GetEcoStatsWinnerFor(string dateXml)
    {
        string winnersName = "Nobody";
        float winningRevenuePerMile = 0;

        var doc = new XmlDocument();
        doc.LoadXml(dateXml);
        int month = int.Parse(doc.DocumentElement.Attributes["month"].Value);
        int year = int.Parse(doc.DocumentElement.Attributes["year"].Value);
        string key = CreateKey(year, month);


        foreach (var stat in ecoStatsStore[key])
        {
            if (stat.Value > winningRevenuePerMile)
            {
                winnersName = stat.Key;
                winningRevenuePerMile = stat.Value;
            }
        }

        return "<ecoStatsWinner SalespersonName=\"" +
                winnersName +
                "\" />";
    }

    private string CreateKey(int year, int month)
    {
        return string.Format("{0:D4}-{1:D2}", year, month);
    }

    private void StoreEcoStats(string key, XmlNode root)
    {
        if (root.HasChildNodes)
        {
            var ecoStats = new Dictionary<string, float>();

            XmlNode ecoStatNode = root.FirstChild;

            while (ecoStatNode != null)
            {
                ecoStats[ecoStatNode.Attributes["SalespersonName"].Value] = float.Parse(ecoStatNode.Attributes["RevenuePerMile"].Value);
                ecoStatNode = ecoStatNode.NextSibling;
            }

            ecoStatsStore[key] = ecoStats;
        }
    }

    private void CheckServiceConnection()
    {
        if (System.Environment.GetEnvironmentVariable("RELIABLE_CONNECTION") != null)
            return;

        int parityControl = rnd.Next(1,5);
        if (parityControl == 1)
            throw new ShoutyStatsServiceException("ShoutyStatsService connection error - please wait a few moments and try your request again");
    }
}

public class ShoutyStatsServiceException : Exception
{
    public ShoutyStatsServiceException( string message ) : base (message)
    {
    }
}
