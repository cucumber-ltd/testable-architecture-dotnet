using System.Collections.Generic;

public class ShoutyReportProcessor
{
    IList<MileageClaim> mileageClaims;
    IStatsService statsService;

    public ShoutyReportProcessor(IList<MileageClaim> mileageClaims, IStatsService statsService)
    {
        this.mileageClaims = mileageClaims;
        this.statsService = statsService;
    }

    public IList<EcoStat> Process()
    {
        var stats = new List<EcoStat>();
        var groupedClaims = GroupBySalesperson(mileageClaims);

        foreach (var salespersonName in groupedClaims.Keys)
        {
            var stat = CalculateStatForSalesperson(salespersonName, groupedClaims[salespersonName]);
            stats.Add(stat);
        }

        return stats;
    }

    private static IDictionary<string, IList<MileageClaim>> GroupBySalesperson(IList<MileageClaim> claims)
    {
        var groupedClaims = new Dictionary<string, IList<MileageClaim>>();
        foreach (var claim in claims)
        {
            if (claim.Miles > 0)
            {
                if (!groupedClaims.ContainsKey(claim.Name))
                    groupedClaims.Add(claim.Name, new List<MileageClaim>());

                groupedClaims[claim.Name].Add(claim);
            }
        }

        return groupedClaims;
    }

    private EcoStat CalculateStatForSalesperson(string name, IList<MileageClaim> claims)
    {
        var totalMiles = 0;
        var totalRevenue = 0m;
        foreach (var claim in claims) {
            totalRevenue += statsService.GetRevenue(claim.CustomerID);
            totalMiles += claim.Miles;
        }
        var revenuePerMile = (float)totalRevenue / (float)totalMiles;

        return new EcoStat(name, revenuePerMile);
    }
}
