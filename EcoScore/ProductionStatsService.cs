using System.Xml;
 
public class ProductionStatsService : IStatsService
{
    ShoutyStatsService service = new ShoutyStatsService();
 
    public decimal GetRevenue(int customerId)
    {
        return ExtractRevenue(service.GetRevenueForCustomer(customerId));
    }

    public bool IsValidCustomer(int customerId)
    {
        string responseXml = service.IsValidCustomer("<customer id=\"" + customerId + "\" />");

        var doc = new XmlDocument();
        doc.LoadXml(responseXml);
        return doc.DocumentElement.Attributes["result"].Value.Equals("TRUE");
    }
 
    private decimal ExtractRevenue(string xml)
    {
        var doc = new XmlDocument();
        doc.LoadXml(xml);
        return decimal.Parse(doc.DocumentElement.Attributes["revenue"].Value);
    }
}
