using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bot
{
    /// <summary>
    /// Логика взаимодействия для AttachmentUI.xaml
    /// </summary>
    public partial class AttachmentUI : UserControl
    {
        /// <summary>
        /// Интерфейс для вложений.
        /// </summary>
        public AttachmentUI()
        {
            InitializeComponent();
        }
        /// <summary>
        /// Копирование ссылки в буфер.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SendPhotoLink(object sender, MouseButtonEventArgs e)
        {
            Num.Text = "Clicked";
        }
        /// <summary>
        /// Копирование номера элемента в буфер.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CopyNum(object sender, RoutedEventArgs e)
        {
            Num.Text = "Clicked";
        }
    }
}
