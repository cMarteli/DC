using Newtonsoft.Json;
using RestSharp;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Shared;
using Shared.Models;

namespace GUI {
    public partial class MainWindow : Window {
        private const string BaseUrl = "http://localhost:5147";
        RestClient restClient = new RestClient(BaseUrl);

        public MainWindow() {
            InitializeComponent();
            LoadTotalUsers();
        }

        private async void LoadTotalUsers() {
            int totalUsers = await GetTotalUsersAsync();
            total_text_block.Text = "Total Entries: " + totalUsers.ToString();
        }

        private async Task<int> GetTotalUsersAsync() {
            RestRequest request = new RestRequest("api/entries");
            RestResponse resp = await restClient.ExecuteGetAsync(request);
            return Int32.Parse(resp.Content);
        }

        private BitmapSource BytesToBitmapSource(byte[] byteArray) {
            BitmapImage bitmapImage = new BitmapImage();
            using (MemoryStream ms = new MemoryStream(byteArray)) {
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = ms;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        private async void Go_btnClick(object sender, RoutedEventArgs e) {
            // Check if index_text_box.Text is null, empty or not a valid integer
            if (string.IsNullOrEmpty(index_text_box.Text) || !Int32.TryParse(index_text_box.Text, out int index)) {
                status_label.Content = "Invalid input!";
                status_label.Visibility = Visibility.Visible;
                return;
            }

            RestRequest request = new RestRequest("api/userproxy/" + index.ToString());
            RestResponse resp = await restClient.ExecuteGetAsync(request);

            // Check if the response is successful
            if (resp.StatusCode != System.Net.HttpStatusCode.OK) {
                status_label.Content = "Search failed!";
                status_label.Visibility = Visibility.Visible;
                return;
            }

            User user = JsonConvert.DeserializeObject<User>(resp.Content);

            Dispatcher.Invoke(() => {
                if (user != null) {
                    fName_text_box.Text = user.firstName;
                    lName_text_box.Text = user.lastName;
                    acctNo_text_box.Text = user.acctNo.ToString();
                    pin_text_box.Text = user.pin.ToString();
                    balance_text_box.Text = user.balance.ToString();
                    image_box.Source = BytesToBitmapSource(user.imageBytes);
                    status_label.Content = "Search successful!";
                }
                else {
                    status_label.Content = "User not found!";
                }
                status_label.Visibility = Visibility.Visible;
            });
        }


        private async void Search_btnClick(object sender, RoutedEventArgs e) {
            SearchData mySearch = new SearchData {
                SearchStr = search_text_box.Text
            };
            RestRequest request = new RestRequest("api/searchproxy/");
            request.AddJsonBody(mySearch);

            RestResponse resp = await restClient.ExecutePostAsync(request);

            Dispatcher.Invoke(() => {
                if (resp.StatusCode == System.Net.HttpStatusCode.OK) {
                    User user = JsonConvert.DeserializeObject<User>(resp.Content);
                    if (user != null) {
                        fName_text_box.Text = user.firstName;
                        lName_text_box.Text = user.lastName;
                        acctNo_text_box.Text = user.acctNo.ToString();
                        pin_text_box.Text = user.pin.ToString();
                        balance_text_box.Text = user.balance.ToString();
                        image_box.Source = BytesToBitmapSource(user.imageBytes);
                        status_label.Content = "Search successful!";
                    }
                }
                //else if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) {
                //    var errorMsg = JsonConvert.DeserializeObject<dynamic>(resp.Content);
                //    status_label.Content = errorMsg?.message ?? "User not found.";
                //}
                else {
                    ErrorViewModel errorResponse = JsonConvert.DeserializeObject<ErrorViewModel>(resp.Content);
                    string displayMessage = errorResponse.ErrorMessage;

                    if (errorResponse.ShowRequestId) {
                        displayMessage += $" (Error ID: {errorResponse.RequestId})";
                    }

                    status_label.Content = displayMessage;
                }
                status_label.Visibility = Visibility.Visible;
            });
        }

    }
}
