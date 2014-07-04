using LocationAccess.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Data.Json;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace LocationAccess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class FavoritePage : Page
    {

        List<string> randomColors = new List<string>();

        public FavoritePage()
        {
            this.InitializeComponent();

            randomColors.Add("#00989E");
            randomColors.Add("#FFE67A");
            randomColors.Add("#D8220D");
            randomColors.Add("#F6B01A");
            randomColors.Add("#B4D78D");
        }

        private async void LoadListViewData()
        {
            List<Secret> items = new List<Secret>();
            HttpClient httpClient = new HttpClient();
            string UrlParameter = "?longitude=0" + "&latitude=0&uid=" + MainPage.UserID ;

            // Optionally, define HTTP headers 
            httpClient.DefaultRequestHeaders.Accept.TryParseAdd("application/json");

            // Make the call 
            HttpResponseMessage responseMessage = await httpClient.GetAsync(new Uri("http://scuinfo.com/Api/favorite/key/scucsharp" + UrlParameter));
            string responseText = await responseMessage.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(responseText))
            {
                 JsonObject jsonObject = JsonObject.Parse(responseText);
                 int randomNum = 0;
                 if (jsonObject["secret"].ValueType == JsonValueType.Object)
                 {
                     JsonObject secretMessage = jsonObject["secret"].GetObject();
                     return;
                 }
                 else
                 {

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
                             distance = Math.Round(itemObject["distance"].GetNumber()/1000,2),
                             comments_count = Int32.Parse(itemObject["comments_count"].GetString()),
                             favorites_count = Int32.Parse(itemObject["favorites_count"].GetString()),
                             color = randomColors[randomNum%4]
                         };
                         items.Add(newSecret);
                         randomNum++;
                     }
                     MyList.ItemsSource = items;
                 }
            }
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            LoadListViewData();
        }

        private void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var itemId = ((Secret)e.ClickedItem).aid;
            Frame.Navigate(typeof(CommentPage), itemId);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        async void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true; // We've handled this button press
            if (Frame.CanGoBack) Frame.GoBack();
        }
    }
}
