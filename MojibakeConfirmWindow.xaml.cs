using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
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
        private static string MojibakeFolderName = "mojibake";

        public MojibakeConfirmWindow(List<MojibakeFile> fileList)
        {
            InitializeComponent();
            _fileList = fileList;

            dataGrid.ItemsSource = _fileList;
        }

        private void fixButton_Click(object sender, RoutedEventArgs e)
        {
            outputJson();
            foreach (MojibakeFile file in _fileList)
            {
                Console.WriteLine(file);
                var tfile = TagLib.File.Create(file.FilePath);

                char fullwidthTilde = '\uFF5E';
                bool needFix = false;
                if (tfile.Tag.Album != null && tfile.Tag.Album.Contains(fullwidthTilde))
                {
                    needFix = true;
                    tfile.Tag.Album = tfile.Tag.Album.Replace('\uFF5E', '\u301C');
                }
                if (tfile.Tag.Title != null && tfile.Tag.Title.Contains(fullwidthTilde))
                {
                    needFix = true;
                    tfile.Tag.Title = tfile.Tag.Title.Replace('\uFF5E', '\u301C');
                }
                // Artistsは非推奨でPerformersを使えという警告が出るが、Performersはセッターの実装が空なので使えないと思われる。
                if (tfile.Tag.Artists != null && tfile.Tag.Artists[0] != null && tfile.Tag.Artists[0].Contains(fullwidthTilde))
                {
                    needFix = true;
                    // Artistsのセッターの実装上、配列要素を部分的に更新できないので、要素1個の配列に丸ごと置き換える。
                    string replacedArtist = tfile.Tag.Artists[0].Replace('\uFF5E', '\u301C');
                    tfile.Tag.Artists = new string[] { replacedArtist };
                }
                tfile.Save();
            }
            System.Windows.MessageBox.Show("正常に終了しました", "確認", MessageBoxButton.OK, MessageBoxImage.Information);
            chkConfirm.IsEnabled = false;
            fixButton.IsEnabled = false;
        }

        private void outputJson()
        {
            string jsonString = JsonSerializer.Serialize(_fileList);
            string jsonId = DateTime.Now.ToString("yyyyMMddHHmmss");

            // フォルダが存在しない場合は作成
            if (!Directory.Exists(MojibakeFolderName))
            {
                Directory.CreateDirectory(MojibakeFolderName);
            }

            // ファイルに書き込み
            File.WriteAllText(MojibakeFolderName + "\\" + jsonId + ".json", jsonString);
        }
    }
}
