using NUnit.Framework;

[TestFixture]
public class StatsServiceContractTests
{
    private ProductionStatsService production = new ProductionStatsService();

    private static readonly int VALID_FAKE_CUSTOMER_ID1 = 22;
    private static readonly int VALID_FAKE_CUSTOMER_ID2 = 57;
    private static readonly decimal ARBITRARY_REVENUE = 765;
    private static readonly int INVALID_FAKE_CUSTOMER_ID = 23;
    private static readonly string FAKE_CUSTOMER_DATA =
        VALID_FAKE_CUSTOMER_ID1 + "," + ARBITRARY_REVENUE + " ; " +
        VALID_FAKE_CUSTOMER_ID2 + "," + ARBITRARY_REVENUE;
    private FakeStatsService fake = new FakeStatsService(FAKE_CUSTOMER_DATA);

    [Test]
    public void production_returns_revenue_for_a_valid_customer_id()
    {
        var validCustomerId = GetValidCustomerIdFromProduction();
        Assert.IsTrue(production.GetRevenue(validCustomerId) >= 0);
    }

    [Test]
    public void production_throws_exception_for_invalid_customer_id()
    {
        Assert.Throws<ShoutyStatsServiceException>(delegate ()
        {
            var invalidCustomerId = GetInvalidCustomerIdFromProduction();
            production.GetRevenue(invalidCustomerId);
        });
    }

    [Test]
    public void fake_returns_revenue_for_a_valid_customer_id()
    {
        Assert.IsTrue(fake.GetRevenue(VALID_FAKE_CUSTOMER_ID1) >= 0);
        Assert.IsTrue(fake.GetRevenue(VALID_FAKE_CUSTOMER_ID2) >= 0);
    }

    [Test]
    public void fake_throws_exception_for_invalid_customer_id()
    {
        Assert.Throws<ShoutyStatsServiceException>(delegate ()
        {
            production.GetRevenue(INVALID_FAKE_CUSTOMER_ID);
        });
    }

    private int GetValidCustomerIdFromProduction()
    {
        return GetCustomerIdFromProduction(true);
    }

    private int GetInvalidCustomerIdFromProduction()
    {
        return GetCustomerIdFromProduction(false);
    }

    private int GetCustomerIdFromProduction(bool requestedCustomerStatus)
    {
        for (var id = 0; id < 999; id++)
        {
            if (production.IsValidCustomer(id) == requestedCustomerStatus)
            {
                return id;
            }
        }

        throw new System.Exception("No customer ID with requested status found");
    }
}
