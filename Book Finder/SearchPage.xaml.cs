using System;
using System.Windows;
using System.Windows.Controls;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Windows.Documents;

namespace Book_Finder
{
    /// <summary>
    /// Логика взаимодействия для SearchPage.xaml
    /// </summary>
    public class Book
    {
        public string authors { get; set; }
        public string title { get; set; }
    }

    public class BookListResponse
    {
        public string Kind { get; set; }
        public List<Book> Items { get; set; }
        public int TotalItems { get; set; }
        public List<BookListResponse> Books { get; set; }
    }

    public partial class SearchPage : Page
    {
        public SearchPage()
        {
            InitializeComponent();
        }
        private static async Task ProcessRepositories(ListView listView, string text, TextBlock Count)
        {
            HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create($"https://www.googleapis.com/books/v1/volumes?q={text}:keyes&key=AIzaSyAM3ztiQNUU2L0JYQ1VAZc1JffOhonTWoU");
            HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync();
            string answer = string.Empty;
            using (Stream stream = response.GetResponseStream())
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    //Console.WriteLine(reader.ReadToEnd());
                    answer = reader.ReadToEnd();
                }
            }
            response.Close();

            BookListResponse bookListResponse = JsonConvert.DeserializeObject<BookListResponse>(answer);
            Count.Text = $"Количество: {bookListResponse.TotalItems}";
            foreach (var item in bookListResponse.Items)
            {
                Console.WriteLine(item.title);
            }

            //client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            //var streamTask = client.GetStreamAsync($"https://www.googleapis.com/books/v1/volumes?q={text}:keyes&key=AIzaSyAM3ztiQNUU2L0JYQ1VAZc1JffOhonTWoU");
            //string json = new StreamReader(await streamTask).ReadToEnd();
            //var repositories = JsonConvert.DeserializeObject<List<BookListResponse>>(json);
            //foreach (var item in repositories)
            //    Debug.WriteLine(item);

        }
        private void Request(string text)
        {
            string url = $"https://www.googleapis.com/books/v1/volumes?q={text}:keyes&key=AIzaSyAM3ztiQNUU2L0JYQ1VAZc1JffOhonTWoU";
            using (var webClient = new WebClient())
            {
                // Выполняем запрос по адресу и получаем ответ в виде строки
                var response = webClient.DownloadString(url);
                Debug.WriteLine(response);
            }
        }
        private async void Btn_Search_Click(object sender, RoutedEventArgs e)
        {
           await ProcessRepositories(listView, Search.Text, Count);
        }
    }
}
