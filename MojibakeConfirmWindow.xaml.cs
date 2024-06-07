using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace music_mojibake_fix
{
    /// <summary>
    /// ConfirmWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MojibakeConfirmWindow : Window
    {
        private List<MojibakeFile> _fileList;

        public MojibakeConfirmWindow(List<MojibakeFile> fileList)
        {
            InitializeComponent();
            _fileList = fileList;

            dataGrid.ItemsSource = _fileList;
        }

        private void fixButton_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
