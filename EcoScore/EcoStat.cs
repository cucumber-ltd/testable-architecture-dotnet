public class EcoStat
{
    public string SalespersonName { get; private set; }
    public float RevenuePerMile { get; private set; }

    public EcoStat(string salespersonName, float revenuePerMile)
    {
        this.SalespersonName = salespersonName;
        this.RevenuePerMile = revenuePerMile;
    }
}
