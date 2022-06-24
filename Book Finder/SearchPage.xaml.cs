using System;
using System.Windows;
using System.Windows.Controls;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Windows.Input;
using System.Windows.Threading;

namespace Book_Finder
{
    /// <summary>
    /// Логика взаимодействия для SearchPage.xaml
    /// </summary>
    public class Book
    {
        public string Title { get; set; }
        public string Authors { get; set; }
        public string PublishedDate { get; set; }
        public string Publisher { get; set; }
        public string Image { get; set; }
        public string PreviewLink { get; set; }
        public string Description { get; set; }
        public string PageCount { get; set; }
        public string BuyLink { get; set; }
        public string WebReaderLink { get; set; }
        public string Id { get; set; }
    }

    public partial class SearchPage : Page
    {
        public SearchPage()
        {
            InitializeComponent();
            bookClient.BaseAddress = new Uri("https://www.googleapis.com/books/v1/volumes?q=");
            BookCount.Visibility = Visibility.Hidden;
        }
        readonly HttpClient bookClient = new HttpClient();
        List<Book> list;
        readonly Dispatcher mainthread = Dispatcher.CurrentDispatcher;
        private async Task ProcessRepositories(string text)
        {
            HttpResponseMessage response = await bookClient.GetAsync($"https://www.googleapis.com/books/v1/volumes?q={text}&maxResults=40");
            // Console.WriteLine(response.StatusCode);
            JObject bookJson = JObject.Parse(await response.Content.ReadAsStringAsync());
            Count.Text = bookJson["totalItems"].ToString();
            if(Count.Text == 0.ToString())
            {
                listView.ItemsSource = "";
                return;
            }
            BookCount.Visibility = Visibility.Visible;
            JArray books = (JArray)bookJson["items"];
            list = new List<Book>();
            if (books is null)
            {
                return;
            }

            foreach (var book in books)
            {
                var r  = await Task.Run(() =>
                {
                    JObject volumeInfoObject = (JObject)book["volumeInfo"];
                    JObject imageLinksObject = (JObject)volumeInfoObject["imageLinks"];
                    JObject buyLinkObject = (JObject)book["saleInfo"];
                    JObject accessInfoObject = (JObject)book["accessInfo"];
                    return (volumeInfoObject, imageLinksObject, buyLinkObject, accessInfoObject);
                });

                await mainthread.Invoke(async () => list.Add(new Book { Title = r.volumeInfoObject["title"].ToString(), Authors = await ParseString(r.volumeInfoObject, "authors"), PublishedDate = await ParseImage(r.volumeInfoObject, "publishedDate", 2),
                    Publisher = await ParseImage(r.volumeInfoObject, "publisher", 2), Image = await ParseImage(r.imageLinksObject, "thumbnail", 1), PreviewLink = r.volumeInfoObject["previewLink"].ToString(),
                    Description = await ParseImage(r.volumeInfoObject, "description", 2), PageCount = await ParseImage(r.volumeInfoObject, "pageCount", 2), BuyLink = await ParseImage(r.buyLinkObject, "buyLink", 2), WebReaderLink = await ParseImage(r.accessInfoObject, "webReaderLink", 2),
                    Id = book["id"].ToString()}
                ));
            }
            listView.ItemsSource = list;

        }
        private async Task<string> ParseImage(JObject text, string index, int choice)
        {
            return await Task.Run(() =>
            {
                if (text == null && choice == 1)
                {
                    return "pack://application:,,,/Book Finder;component/Image/Book.png";
                }
                else if (text == null && choice == 2)
                {
                    return "–";
                }
                else if (text[index] == null)
                {
                    return "–";
                }
                else
                {
                    return text[index].ToString();
                }
            });
        }
        private async Task<string> ParseString(JObject text, string index)
        {
            return await Task.Run(() => { 
                if (text[index] == null)
                {
                    return "–";
                }
                else
                {
                    string temp = string.Empty;
                    int temp_count = 0;
                    foreach (var item in text[index])
                    {
                        if (temp_count == 0)
                        {
                            temp += item;
                            temp_count++;
                        }
                        else
                        {
                            temp += $", {item}";
                        }
                    }
                    return temp;
                }
            });
        }   
        protected async void Btn_Search_Click(object sender, RoutedEventArgs e)
        {
            if (Search.Text.Length != 0)
            {
                Keyboard.ClearFocus();
                await ProcessRepositories(Search.Text);
            }
        }

        private void TextTitle_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                var item = (sender as FrameworkElement).DataContext;
                int index = listView.Items.IndexOf(item);
                Process.Start(list[index].PreviewLink);
            }
        }

        private void Btn_More_Click(object sender, RoutedEventArgs e)
        {
            var item = (sender as FrameworkElement).DataContext;
            int index = listView.Items.IndexOf(item);
            NavigationService.Navigate(new MorePage(list, index));
        }

        private void Search_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Search.Text.Length != 0 && e.Key == Key.Enter)
            {
                Btn_Search_Click(sender, e);
            }
        }
    }
}
