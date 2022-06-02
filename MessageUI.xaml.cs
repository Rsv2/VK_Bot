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
using System.Media;

namespace Bot
{
    /// <summary>
    /// Логика взаимодействия для MessageUI.xaml
    /// </summary>
    public partial class MessageUI : UserControl
    {
        /// <summary>
        /// Форма вывода сообщений.
        /// </summary>
        public MessageUI()
        {
            InitializeComponent();
            SystemSounds.Exclamation.Play();
        }
    }
}
