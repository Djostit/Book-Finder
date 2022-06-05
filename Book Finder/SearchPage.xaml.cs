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
using Newtonsoft.Json.Linq;
using System.Text;
using System.Windows.Media.Imaging;
using System.Linq;

namespace Book_Finder
{
    /// <summary>
    /// Логика взаимодействия для SearchPage.xaml
    /// </summary>
    public class BookListResponse
    {
        public string Kind { get; set; }
        public int TotalItems { get; set; }
        public List<BookListResponse> Books { get; set; }
        public List<string> authors { get; set; }
    }
    public class BookObject
    {
        public BookObject()
        {
            authors = new List<string>(); // init authors in constructor
        }
        public string id { get; set; }
        public string title { get; set; }
        public List<string> authors { get; set; }
        public string publisher { get; set; }
        public string publishedDate { get; set; }
        public string description { get; set; }
        public int pageCount { get; set; }
        public string blurb { get; set; }

        // Availability
        public bool pdfAvailable { get; set; }
        public string pdfLink { get; set; }
        public bool epubAvailable { get; set; }
        public string epubLink { get; set; }
        public string forSale { get; set; }
        public string saleLink { get; set; }
    }

    public partial class SearchPage : Page
    {
        public SearchPage()
        {
            InitializeComponent();
            bookClient.BaseAddress = new Uri("https://www.googleapis.com/books/v1/volumes?q=");
        }
        HttpClient bookClient = new HttpClient();
        private void ProcessRepositories(string text)
        {
            HttpResponseMessage response = bookClient.GetAsync($"https://www.googleapis.com/books/v1/volumes?q={text}").Result;
            Console.WriteLine(response.StatusCode);
            JObject bookJson = JObject.Parse(response.Content.ReadAsStringAsync().Result);
            Count.Text = $"Count book: {bookJson["totalItems"]}";

            JArray books = (JArray)bookJson["items"];
            foreach (var book in books)
            {
                JObject volumeInfoObject = (JObject)book["volumeInfo"];
                //Console.WriteLine(volumeInfoObject["title"]);
                //Console.WriteLine(volumeInfoObject["publishedDate"]);
                //Console.WriteLine(volumeInfoObject["description"]);
                //Console.WriteLine(volumeInfoObject["publisher"]);
                //Console.WriteLine(ParseString(volumeInfoObject, "authors"));
                MessageBox.Show($"{volumeInfoObject["title"]}" +
                    $"\nАвторы: {ParseString(volumeInfoObject, "authors")}" +
                    $"\nДата: {volumeInfoObject["publishedDate"]}" +
                    $"\nПубликатор: {volumeInfoObject["publisher"]}" +
                    $"\nОписание: {volumeInfoObject["description"]}");
            }

        }
        private string ParseString(JObject text, string index)
        {
            if (text[index] == null)
            {
                return "";
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
        }   
        private void Btn_Search_Click(object sender, RoutedEventArgs e)
        {
           ProcessRepositories(Search.Text);
        }
    }
}
