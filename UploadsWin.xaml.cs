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

namespace Bot
{
    /// <summary>
    /// Логика взаимодействия для UploadsWin.xaml
    /// </summary>
    public partial class UploadsWin : Window
    {
        public UploadsWin()
        {
            InitializeComponent();
        }

        private void ClosingWindow(object sender, System.ComponentModel.CancelEventArgs e)
        {
            UploadsList.Children.Clear();
        }
    }
}
