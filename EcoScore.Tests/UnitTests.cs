using NUnit.Framework;
using System;
using System.Text;
using System.Collections.Generic;

[TestFixture]
public class UnitTests
{
    MileageClaimBuilder mileageClaimBuilder;
    CustomerRevenueBuilder customerRevenueBuilder;

    IList<CustomerRevenue> customerRevenues;
    IList<MileageClaim> claims;

    [SetUp]
    public void Setup()
    {
        mileageClaimBuilder = new MileageClaimBuilder();
        customerRevenueBuilder = new CustomerRevenueBuilder();

        customerRevenues = new List<CustomerRevenue>();
        claims = new List<MileageClaim>();
    }

    [Test]
    public void milage_claim_can_be_created_from_strings()
    {
        var claim = new MileageClaim("David Allen", 37000, 99);
        Assert.AreEqual("David Allen", claim.Name);
        Assert.AreEqual(37000, claim.Miles);
        Assert.AreEqual(99, claim.CustomerID);
    }

    [Test]
    public void single_sales_person()
    {
        // Create FakeStatsService
        CreateCustomerRevenue();
        var service = CreateFakeStatsService(customerRevenues);

        // Create claims
        CreateMileageClaim(customerRevenues[0].ID);

        // Create ShoutyReportProcessor
        var processor = new ShoutyReportProcessor(claims, service);

        // Call process()
        var stats = processor.Process();

        // Assert contents
        Assert.AreEqual(1, stats.Count);
        AssertExpectedRevenue(claims[0], customerRevenues[0], stats[0]);
    }

    [Test]
    public void multiple_sales_people()
    {
       // Create FakeStatsService
        CreateCustomerRevenue();
        CreateCustomerRevenue();
        var service = CreateFakeStatsService(customerRevenues);

        // Create claims
        CreateMileageClaim(customerRevenues[0].ID);
        CreateMileageClaim(customerRevenues[1].ID);

        // Create ShoutyReportProcessor
        var processor = new ShoutyReportProcessor(claims, service);

        // Call process()
        var stats = processor.Process();

        // Assert contents
        Assert.AreEqual(2, stats.Count);
        AssertExpectedRevenue(claims[0], customerRevenues[0], stats[0]);
        AssertExpectedRevenue(claims[1], customerRevenues[1], stats[1]);
    }

    private IStatsService CreateFakeStatsService(IList<CustomerRevenue> customerRevenues)
    {
        var initialiser = new StringBuilder();

        foreach (var customerRevenue in customerRevenues)
        {
            if (initialiser.Length > 0)
            {
                initialiser.Append(";");
            }

            initialiser.Append(customerRevenue.ID);
            initialiser.Append(",");
            initialiser.Append(customerRevenue.Revenue);
        }

        return new FakeStatsService(initialiser.ToString());
    }

    private void CreateCustomerRevenue()
    {
        var customerRevenue = customerRevenueBuilder
                                .Build();
        customerRevenues.Add(customerRevenue);
    }

    private void CreateMileageClaim(int customerId)
    {
        var mileageClaim = mileageClaimBuilder
                            .WithCustomer(customerId)
                            .Build();
        claims.Add(mileageClaim);
    }

    private void AssertExpectedRevenue(MileageClaim claim, CustomerRevenue customerRevenue, EcoStat stat)
    {
        float EXPECTED_REVENUE_PER_MILE = (float)customerRevenue.Revenue/(float)claim.Miles;
        Assert.AreEqual(claim.Name, stat.SalespersonName);
        Assert.AreEqual(EXPECTED_REVENUE_PER_MILE, stat.RevenuePerMile);
    }
}

class MileageClaimBuilder
{
    private static Random rnd = new Random();
    private static int counter = 0;

    private string name = "Any Name";
    private int mileage = 0;
    private int customerId = 1;

    public MileageClaim Build()
    {
        mileage = rnd.Next(1, 100000);
        return new MileageClaim(name + counter++, mileage, customerId);
    }

    public MileageClaimBuilder WithCustomer(int id)
    {
        customerId = id;
        return this;
    }
}

class CustomerRevenueBuilder
{
    private static Random rnd = new Random();
    private static int customerId = 1;

    public CustomerRevenue Build()
    {
        customerId += rnd.Next(1, 20);
        decimal revenue = (decimal)rnd.Next(0, 100000);

        return new CustomerRevenue(customerId, revenue);
    }
}

class CustomerRevenue
{
    public int ID { get; private set; }
    public decimal Revenue { get; private set; }

    public CustomerRevenue(int id, decimal revenue)
    {
        this.ID = id;
        this.Revenue = revenue;
    }
}
