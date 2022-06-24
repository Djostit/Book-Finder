using MaterialDesignThemes.Wpf;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Book_Finder
{
    /// <summary>
    /// Логика взаимодействия для MorePage.xaml
    /// </summary>
    public class Feedback
    {
        public string Name { get; set; }
        public string RatingBar { get; set; }
        public string Description { get; set; }
        public string Id_Feedback { get; set; }
    }

    public partial class MorePage : Page
    {
        readonly List<Book> list;
        readonly int index;
        List<Feedback> feedbacks;
        List<Feedback> item_feed;
        string jsonFeed;
        DispatcherTimer timer;
        DispatcherTimer timer2;
        public MorePage(List<Book> list, int index)
        {
            InitializeComponent();
            jsonFeed = File.ReadAllText(Path.GetFullPath("Jsons/Feedback.json").Replace(@"bin\Debug\", ""));
            feedbacks = JsonConvert.DeserializeObject<List<Feedback>>(jsonFeed);
            item_feed = JsonConvert.DeserializeObject<List<Feedback>>(jsonFeed);
            item_feed.Clear();
            foreach (var feedback in feedbacks)
            {
                if (feedback.Id_Feedback == list[index].Id)
                {
                    item_feed.Add(feedback);
                }
            }
            listView.ItemsSource = item_feed;
            this.list = list;
            this.index = index;
            DataContext = list[index];
            if (list[index].BuyLink.Contains("–"))
            {
                Btn_BuyBook.IsEnabled = false;
            }
            if (list[index].WebReaderLink.Contains("–"))
            {
                Btn_Read.IsEnabled = false;
            }
            int count = 0;
            int sqrt = 0;
            foreach (var item in feedbacks)
            {
                if (item.Id_Feedback.Contains(list[index].Id))
                {
                    count++;
                    sqrt += int.Parse(item.RatingBar);
                }
            }
            if (count == 0) { BasicRatingBar.Value = 0; }
            else { BasicRatingBar.Value = sqrt / count; }
            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timerTick;

            timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromSeconds(1);
            timer2.Tick += timerTick2;
        }

        private void Btn_BuyBook_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(list[index].BuyLink);
        }

        private void Btn_Read_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(list[index].PreviewLink);
        }

        private void Btn_Accept_Click(object sender, RoutedEventArgs e)
        {
            if (NameTextBox.Text.Replace("  ", " ").Length < 3 || NameTextBox.Text.Trim() == String.Empty)
            {
                if (Properties.Settings.Default.DefaultLanguage.ToString().Contains("ru-RU"))
                {
                    TextDrawer.Text = "Пустое имя";
                }
                else
                {
                    TextDrawer.Text = "Empty name";
                }
                DrawerHost.IsBottomDrawerOpen = true;
                Btn_Accept.IsEnabled = false;
                timer.Start();
                return;
            }
            else if (TextBoxDescription.Text.Trim() == string.Empty || TextBoxDescription.Text.Length <= 0)
            {
                if (Properties.Settings.Default.DefaultLanguage.ToString().Contains("ru-RU"))
                {
                    TextDrawer.Text = "Пустое описание";
                }
                else
                {
                    TextDrawer.Text = "Empty description";
                }
                DrawerHost.IsBottomDrawerOpen = true;
                Btn_Accept.IsEnabled = false;
                timer.Start();
                return;
            }
            else if (TextBoxDescription.Text.Replace("  ", " ").Length < 20)
            {
                if (Properties.Settings.Default.DefaultLanguage.ToString().Contains("ru-RU"))
                {
                    TextDrawer.Text = "Меньше 20 символов";
                }
                else
                {
                    TextDrawer.Text = "Under 20 characters";
                }
                DrawerHost.IsBottomDrawerOpen = true;
                Btn_Accept.IsEnabled = false;
                timer.Start();
                return;

            }
            feedbacks.Add(new Feedback { Name = NameTextBox.Text, RatingBar = RatingBar.Value.ToString(), Description = TextBoxDescription.Text.Replace("  ", " "), Id_Feedback = list[index].Id });
            var convertedJson = JsonConvert.SerializeObject(feedbacks, Formatting.Indented);

            File.WriteAllText(Path.GetFullPath("Jsons/Feedback.json").Replace(@"bin\Debug\", ""), convertedJson);

            var jsonFeed = File.ReadAllText(Path.GetFullPath("Jsons/Feedback.json").Replace(@"bin\Debug\", ""));
            feedbacks = JsonConvert.DeserializeObject<List<Feedback>>(jsonFeed);
            int count = 0;
            int sqrt = 0;
            List<Feedback> item_feed = JsonConvert.DeserializeObject<List<Feedback>>(jsonFeed);
            item_feed.Clear();
            foreach (var item in feedbacks)
            {
                if (item.Id_Feedback.Contains(list[index].Id))
                {
                    count++;
                    sqrt += int.Parse(item.RatingBar);
                    item_feed.Add(item);
                }
            }
            listView.ItemsSource = item_feed;
            if (count == 0) { BasicRatingBar.Value = 0; }
            else { BasicRatingBar.Value = sqrt / count; }

            timer2.Start();
        }

        private void Btn_Cancel_Click(object sender, RoutedEventArgs e)
        {
            timer2.Start();
        }

        private void timerTick(object sender, EventArgs e)
        {
            DrawerHost.IsBottomDrawerOpen = false;
            Btn_Accept.IsEnabled = true;
            timer.Stop();
        }

        private void timerTick2(object sender, EventArgs e)
        {
            NameTextBox.Clear();
            TextBoxDescription.Clear();
            RatingBar.Value = 3;
            timer2.Stop();
        }

        private void TextDrawer_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                if (!char.IsLetter(e.Text, 0))
                {
                    e.Handled = true;
                }
            }
            catch { }
        }
    }
}
