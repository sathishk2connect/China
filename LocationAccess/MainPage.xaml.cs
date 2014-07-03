
using LocationAccess.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
             HttpClient httpClient = new HttpClient();

             // Optionally, define HTTP headers 
             httpClient.DefaultRequestHeaders.Accept.TryParseAdd("application/json");

             // Make the call 
            HttpResponseMessage responseMessage = await httpClient.GetAsync(new Uri("http://scuinfo.com/Api/home_timeline/key/scucsharp?longitude=88&latitude=88"));
            string responseText = await responseMessage.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(responseText))
            {
                JsonObject jsonObject = JsonObject.Parse(responseText);
                JsonArray jsonArray = jsonObject["secret"].GetArray();

                foreach (JsonValue groupValue in jsonArray)
                {
                    JsonObject itemObject = groupValue.GetObject();
                    Secret newSecret = new Secret
                    {
                        aid = Int32.Parse(itemObject["aid"].GetString()),
                        uid = Int32.Parse(itemObject["uid"].GetString()),
                        longitude = Double.Parse(itemObject["longitude"].GetString()),
                        latitude = Double.Parse(itemObject["latitude"].GetString()),
                        content = itemObject["content"].GetString(),
                        time = Int32.Parse(itemObject["time"].GetString()),
                        distance = itemObject["distance"].GetNumber(),
                        comments_count = Int32.Parse(itemObject["comments_count"].GetString()),
                        favorites_count = Int32.Parse(itemObject["favorites_count"].GetString())
                    };
                    items.Add(newSecret);
                }
                MyList.ItemsSource = items;
            }
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
            var itemId = ((Secret)e.ClickedItem).aid; 
            Frame.Navigate(typeof(CommentPage), itemId);
        }

        private async void PostContentMessage()
        {
            String streamContent = "content=I+am+good&latitude=25&longitude=34&uid=1&sub=Submit";
            HttpClient httpClient = new HttpClient();
            HttpContent content = new StringContent(streamContent);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            //var messageDialog1 = new MessageDialog(content.ToString());
            //await messageDialog1.ShowAsync();
            HttpResponseMessage response = await httpClient.PostAsync("http://scuinfo.com/Api/add_content/key/scucsharp",content);
            string result = await response.Content.ReadAsStringAsync();
            var messageDialog = new MessageDialog(result);
            await messageDialog.ShowAsync();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PostContentMessage();

        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
