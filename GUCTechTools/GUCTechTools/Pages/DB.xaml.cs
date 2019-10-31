/// This is a sample of the GUCTech Tools developed by Michael Berger and Kyle Avery 
/// At the Academic Media Services of Washington State University between May 2019 and 
/// October 2019. All sensitive data has been removed. What remains is a demo of the
/// project's functionality. Backend integration is not included in this sample.    

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace GUCTechTools.Pages
{
    /// <summary>
    /// Interaction logic for DB.xaml
    /// </summary>
    public partial class DB : Page
    {
        

        public DB()
        {
            InitializeComponent();
        }

        private void ReadControllers()
        {
            ((MainWindow)Application.Current.MainWindow).LoadControllerDB();
            DBGrid.DataContext = ((MainWindow)Application.Current.MainWindow)._1337.DefaultView;
            DBGrid.ItemsSource = ((MainWindow)Application.Current.MainWindow)._1337;
        }

        private void ReadProjectors()
        {
            ((MainWindow)Application.Current.MainWindow).LoadProjectorDB();
            DBGrid.DataContext = ((MainWindow)Application.Current.MainWindow)._H4X0R.DefaultView;
            DBGrid.ItemsSource = ((MainWindow)Application.Current.MainWindow)._H4X0R;
        }

        private void ReadScheduler()
        {
            ((MainWindow)Application.Current.MainWindow).udpServer.LoadScheduler();
            DBGrid.DataContext = ((MainWindow)Application.Current.MainWindow).udpServer._ThisIsNotAServer.DefaultView;
            DBGrid.ItemsSource = ((MainWindow)Application.Current.MainWindow).udpServer._ThisIsNotAServer;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string selected = ((sender as ComboBox).SelectedItem as ComboBoxItem).Content.ToString();
            if(selected == "Controllers")
            {
                ReadControllers();
            }
            if (selected == "Projectors")
                ReadProjectors();
            if (selected == "Scheduler")
                ReadScheduler();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string selected = (cbDBs.SelectedItem as ComboBoxItem).Content.ToString();
            try
            {
                if (selected == "Controllers")
                {
                    ((MainWindow)Application.Current.MainWindow).SaveController();
                }
                if (selected == "Projectors")
                    ((MainWindow)Application.Current.MainWindow).SaveProjector();
                if (selected == "Scheduler")
                    ((MainWindow)Application.Current.MainWindow).SaveScheduler();
            }
            catch(Exception)
            {

            }
        }
    }
}
