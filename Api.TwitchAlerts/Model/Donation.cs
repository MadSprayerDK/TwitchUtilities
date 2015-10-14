namespace Api.TwitchAlerts.Model
{
    public class Donation
    {
        public int Donation_Id { set; get; }
        public int Created_At { set; get; }
        public string Currency { set; get; }
        public double Amount { set; get; }
        public string Name { set; get; }
        public string Message { set; get; }
    }
}
