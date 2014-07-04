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
using LocationAccess.Model;
using System.Net.Http.Headers;
using Windows.UI.Popups;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace LocationAccess
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class CommentPage : Page
    {
        public int AppID { get; set; }

        public CommentPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.
        /// This parameter is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
            AppID = (int)e.Parameter;
            LoadSecret(AppID);
        }

        private async void LoadSecret(int id)
        {
            List<Comment> comments = new List<Comment>();
            HttpClient httpClient = new HttpClient();
            string UrlParameter = "?aid="+ id.ToString() + "&latitude=40&longitude=32";
            HttpResponseMessage responseMessage = await httpClient.GetAsync(new Uri("http://scuinfo.com/Api/show/key/scucsharp" + UrlParameter));
            string responseText = await responseMessage.Content.ReadAsStringAsync();
            if (!string.IsNullOrEmpty(responseText))
            {
                JsonObject itemObject = JsonObject.Parse(responseText);
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

                Secret.Text = newSecret.content;

                responseMessage = await httpClient.GetAsync(new Uri("http://scuinfo.com/Api/comment/key/scucsharp" + UrlParameter));
                responseText = await responseMessage.Content.ReadAsStringAsync();
                if (!string.IsNullOrEmpty(responseText))
                {
                    JsonObject jsonObject = JsonObject.Parse(responseText);
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
                            JsonObject aitemObject = groupValue.GetObject();
                            Comment newComment = new Comment
                            {
                                cid = Int32.Parse(aitemObject["cid"].GetString()),
                                aid = Int32.Parse(aitemObject["aid"].GetString()),
                                uid = Int32.Parse(aitemObject["uid"].GetString()),
                                time = Int32.Parse(aitemObject["time"].GetString()),
                                content = aitemObject["content"].GetString()
                            };
                            comments.Add(newComment);
                        }
                        CommentList.ItemsSource = comments;
                    }
                }
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e) { 
            Windows.Phone.UI.Input.HardwareButtons.BackPressed -= HardwareButtons_BackPressed;
        }

        async void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            e.Handled = true; // We've handled this button press
            if (Frame.CanGoBack) Frame.GoBack(); 
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PostCommentMessage();
            LoadSecret(AppID);
        }

        private async void PostCommentMessage()
        {
            String streamContent = "content=" + textComment.Text + "&aid=" + AppID + "&uid=" + MainPage.UserID;
            HttpClient httpClient = new HttpClient();
            HttpContent content = new StringContent(streamContent);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/x-www-form-urlencoded");
            HttpResponseMessage response = await httpClient.PostAsync("http://scuinfo.com/Api/add_comment/key/scucsharp", content);
            string result = await response.Content.ReadAsStringAsync();
            var messageDialog = new MessageDialog(result);
            await messageDialog.ShowAsync();
        }

    }
}
