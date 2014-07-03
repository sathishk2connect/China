
using LocationAccess.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace LocationAccess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        Geolocator geo = null;

        Windows.Storage.StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
        Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;

        private async void LoadListViewData()
        {
            List<Secret> items = new List<Secret>();
            var _Folder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            _Folder = await _Folder.GetFolderAsync("SampleJSON");

            var _File = await _Folder.GetFileAsync("Sample.json");

            var readData = await Windows.Storage.FileIO.ReadTextAsync(_File);

            JsonObject jsonObject = JsonObject.Parse(readData);
            JsonArray jsonArray = jsonObject["Secrets"].GetArray();

            foreach (JsonValue groupValue in jsonArray)
            {
                JsonObject itemObject = groupValue.GetObject();
                Secret newSecret = new Secret
                {
                    aid = (int)itemObject["aid"].GetNumber(),
                    uid = (int)itemObject["uid"].GetNumber(),
                    longitude = itemObject["longitude"].GetNumber(),
                    latitude = itemObject["latitude"].GetNumber(),
                    content = itemObject["content"].GetString(),
                    time = (int)itemObject["time"].GetNumber(),
                    distance = (int)itemObject["distance"].GetNumber(),
                    comments_count = (int)itemObject["comments_count"].GetNumber(),
                    favourite_count = (int)itemObject["favourites_count"].GetNumber()
                };
                items.Add(newSecret);
            }
            MyList.ItemsSource = items;
        }

        private async void LoadUserID()
        {
            HttpClient httpClient = new HttpClient();
            // Optionally, define HTTP headers 
            httpClient.DefaultRequestHeaders.Accept.TryParseAdd("application/json");
            // Make the call 
            string responseText = await httpClient.GetStringAsync(new Uri("http://scuinfo.com/Api/add_user?key=scucsharp"));
            textUserID.Text = responseText;
            JsonObject jsonObject = JsonObject.Parse(responseText);
            localSettings.Values["UserID"] = jsonObject["uid"].GetNumber();
            textUserID.Text = localSettings.Values["UserID"].ToString();
        }

        public MainPage()
        {
            this.InitializeComponent();

            this.NavigationCacheMode = NavigationCacheMode.Required;

            Object value = localSettings.Values["UserID"];

            if (value == null)
            {
                LoadUserID();
            }
            else
            {
                textUserID.Text = value.ToString();
            }

            LoadListViewData(); 
        }



        private async void button1_Click(
            object sender, RoutedEventArgs e)
        {
            
            if (geo == null)
            {
                geo = new Geolocator();
            }

            Geoposition pos = await geo.GetGeopositionAsync();
            textLatitude.Text = "Latitude: " + pos.Coordinate.Point.Position.Latitude.ToString();
            textLongitude.Text = "Longitude: " + pos.Coordinate.Point.Position.Longitude.ToString();
            textAccuracy.Text = "Accuracy: " + pos.Coordinate.Accuracy.ToString();
            
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // TODO: Prepare page for display here.

            // TODO: If your application contains multiple pages, ensure that you are
            // handling the hardware Back button by registering for the
            // Windows.Phone.UI.Input.HardwareButtons.BackPressed event.
            // If you are using the NavigationHelper provided by some templates,
            // this event is handled for you.
        }

        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {

        }
    }
}
