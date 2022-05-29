using System.Windows;
using System.Windows.Input;
using System.Diagnostics;

namespace Book_Finder
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Btn_About_Click(object sender, RoutedEventArgs e) => Process.Start("https://github.com/Djostit/Book-Finder/blob/master/README.md");

        private void Btn_Contact_Click(object sender, RoutedEventArgs e) => Process.Start("https://github.com/Djostit");

        private void CloseCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void CloseCommand_Executed(object sender, ExecutedRoutedEventArgs e) => Close();

        private void MinimizeCommand_CanExecute(object sender, CanExecuteRoutedEventArgs e) => e.CanExecute = true;

        private void MinimizeCommand_Executed(object sender, ExecutedRoutedEventArgs e) =>  WindowState = WindowState.Minimized;

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void Btn_GoNow_Click(object sender, RoutedEventArgs e) => frame.NavigationService.Navigate(new SearchPage());
    }
}
