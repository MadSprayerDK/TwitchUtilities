using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TwitchUtilities.Extensions;

namespace TwitchUtilities.UserInterface.StatusPanels
{
    /// <summary>
    /// Interaction logic for DonationDisplay.xaml
    /// </summary>
    public partial class DonationDisplay
    {
        public DonationDisplay(string name, string message, int timestamp, string currency, double amount)
        {
            InitializeComponent();
            var datetime = UnixTime.UnixTimeStampToDateTime(timestamp);

            NameDisplay.Content = name;
            MessageDisplay.Content = message;
            TimeDisplay.Content = datetime.ToLocalTime().ToString("HH:mm");

            var cultureDictionary = new Dictionary<string, CultureInfo>();
            var cultureInfos = CultureInfo.GetCultures(CultureTypes.AllCultures).Where(x => !x.IsNeutralCulture);
            foreach (var cultureInfo in cultureInfos)
            {
                RegionInfo regionInfo;

                try
                {
                    regionInfo = new RegionInfo(cultureInfo.LCID);
                }
                catch(Exception)
                {
                    continue;
                }

                if(!cultureDictionary.ContainsKey(regionInfo.ISOCurrencySymbol))
                    cultureDictionary.Add(regionInfo.ISOCurrencySymbol, cultureInfo);
            }

            var culture = cultureDictionary.FirstOrDefault(x => x.Key == currency);

            AmountDisplay.Content = amount.ToString("C2", culture.Key != null ? culture.Value : new CultureInfo("en-US"));
        }
    }
}
