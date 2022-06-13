using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;

namespace Book_Finder
{
    /// <summary>
    /// Логика взаимодействия для MorePage.xaml
    /// </summary>
    public partial class MorePage : Page
    {
        List<Book> list;
        int index;
        public MorePage(List<Book> list, int index)
        {
            InitializeComponent();
            this.DataContext = list[index];
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
