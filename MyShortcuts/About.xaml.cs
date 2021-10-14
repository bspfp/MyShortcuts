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
using System.Windows.Shapes;

namespace MyShortcuts {
    /// <summary>
    /// About.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class About : Window {
        public About() {
            InitializeComponent();
            title.Content = string.Format("MyShortcuts v{0}", typeof(About).Assembly.GetName().Version.ToString());
        }

        private void Button_Click(object sender, RoutedEventArgs e) {
            DialogResult = true;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            DialogResult = true;
        }

        private void Hyperlink_Click(object sender, RoutedEventArgs e) {
            if (sender is Hyperlink link) {
                System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo() {
                    Verb = "open",
                    FileName = link.NavigateUri.ToString()
                };
                _ = System.Diagnostics.Process.Start(startInfo);
            }
        }
    }
}
