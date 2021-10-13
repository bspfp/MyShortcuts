using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
    public partial class ToastWindow : Window {
        private Timer timer;
        private uint startTick = 0;

        public ToastWindow(string msg) {
            InitializeComponent();
            msglabel.Text = msg;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            startTick = (uint)Environment.TickCount;
            timer = new Timer(_ => {
                _ = Dispatcher.BeginInvoke(new Action(OnTick));
            }, null, 50, 50);
        }

        private void OnTick() {
            // 0.5초 -> 70%
            // 1초 -> 100%
            // 1.5초 -> 70%
            // 2초 -> 창 닫힘
            var elapsed = ((uint)Environment.TickCount) - startTick;
            if (elapsed < 500) {
                msglabel.Opacity = elapsed * 0.7 / 500.0;
            }
            else if (elapsed < 1000) {
                msglabel.Opacity = 0.7 + ((elapsed - 500) * 0.3 / 500.0);
            }
            else if (elapsed < 1500) {
                msglabel.Opacity = 1.0 - ((elapsed - 1000) * 0.3 / 500.0);
            }
            else if (elapsed < 2000) {
                msglabel.Opacity = 0.7 - ((elapsed - 1500) * 0.7 / 500.0);
            }
            else {
                timer.Dispose();
                timer = null;
                Close();
            }
        }
    }
}
