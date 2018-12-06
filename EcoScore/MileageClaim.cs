public class MileageClaim
{
    public string Name { get; private set; }
    public int Miles { get; private set; }
    public int CustomerID { get; private set; }

    public MileageClaim(string name, int miles, int customerId)
    {
        this.Name = name;
        this.Miles = miles;
        this.CustomerID = customerId;
    }
}
