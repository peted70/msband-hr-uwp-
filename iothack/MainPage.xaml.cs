using Microsoft.Band;
using Windows.UI.Xaml.Controls;
using System.Linq;
using System;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Band.Sensors;
using System.Text;
using Newtonsoft.Json;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace iothack
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private IBandClient _bandClient;
        private IBandInfo _bandInfo;

        public MainPage()
        {
            this.InitializeComponent();
            Loaded += OnLoaded;
        }

        private async void OnLoaded(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            if (_bandClient != null)
                return;

            var bands = await BandClientManager.Instance.GetBandsAsync();
            _bandInfo = bands.First();

            _bandClient = await BandClientManager.Instance.ConnectAsync(_bandInfo);

            var uc = _bandClient.SensorManager.HeartRate.GetCurrentUserConsent();
            bool isConsented = false;
            if (uc == UserConsent.NotSpecified)
            {
                isConsented = await _bandClient.SensorManager.HeartRate.RequestUserConsentAsync();
            }

            if (isConsented || uc == UserConsent.Granted)
            {
                _bandClient.SensorManager.HeartRate.ReadingChanged += async (obj, ev) =>
                {
                    await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                                        {
                                            HeartRateDisplay.Text = ev.SensorReading.HeartRate.ToString();
                                        });
                };
                await _bandClient.SensorManager.HeartRate.StartReadingsAsync();
            }
        }
    }
}
