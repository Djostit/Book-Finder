using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Book_Finder
{
    /// <summary>
    /// Логика взаимодействия для MorePage.xaml
    /// </summary>
    public class Validation : IDataErrorInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string this[string colomnName]
        {
            get
            {
                string error = string.Empty;
                switch (colomnName)
                {
                    case "Name":
                        if (Name == null || Name.Length == 0)
                        {
                            error = "Обязательное поле";
                        }
                        break;
                }
                return error;
            }
        }
        public string Error
        {
            get { throw new NotImplementedException(); }
        }
    }
    public class ViewModel
    {
        public Validation Validation { get; } = new Validation();
        public Book Book { get; set; }
    }

    public partial class MorePage : Page
    {
        List<Book> list;
        int index;
        public MorePage(List<Book> list, int index)
        {
            InitializeComponent();
            DataContext = new ViewModel();
            ViewModel mode = new ViewModel();
            mode.Book = list[index];
            this.list = list;
            this.index = index;
            if (list[index].BuyLink.Contains("None") || list[index].BuyLink.Contains("Not found"))
            {
                Btn_BuyBook.IsEnabled = false;
            }
            if (list[index].WebReaderLink.Contains("None") || list[index].WebReaderLink.Contains("Not found"))
            {
                Btn_Read.IsEnabled = false;
            }
        }

        private void Btn_BuyBook_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(list[index].BuyLink);
        }

        private void Btn_Read_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(list[index].WebReaderLink);
        }
    }
}
