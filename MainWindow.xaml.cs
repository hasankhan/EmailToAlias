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
using System.DirectoryServices;

namespace WpfApplication1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var emails = input.Text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var aliases = await Task.Run(() => emails.Select(email => FindAccountByEmail(email) ?? email));
            string list = string.Join(Environment.NewLine, aliases);
            output.Text = list;            
        }


        public static string FindAccountByEmail(string pEmailAddress)
        {
            string filter = string.Format("(proxyaddresses=SMTP:{0})", pEmailAddress);

            using (DirectoryEntry gc = new DirectoryEntry("GC:"))
            {
                foreach (DirectoryEntry z in gc.Children)
                {
                    using (DirectoryEntry root = z)
                    {
                        using (DirectorySearcher searcher = new DirectorySearcher(root, filter, new string[] { "mailNickname" }))
                        {
                            searcher.ReferralChasing = ReferralChasingOption.All;
                            SearchResultCollection result = searcher.FindAll();
                            foreach (SearchResult item in result)
                            {
                                foreach (object value in item.Properties["mailNickName"])
                                {
                                    return value.ToString();
                                }
                            }

                        }
                    }
                }
            }
            return null;
        }
    }
}
