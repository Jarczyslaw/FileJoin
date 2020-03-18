using System.Diagnostics;
using System.Windows;

namespace FileJoin
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        private void btnAddFile_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(grid.SelectedItems.Count);
        }
    }
}