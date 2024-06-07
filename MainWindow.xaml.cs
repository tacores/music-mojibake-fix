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
using System.Windows.Forms;
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using System.Text.Json;
using System.Diagnostics;

namespace music_mojibake_fix
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<MojibakeFile> mojibakeFileList;
        private static string MojibakeFolderName = "mojibake";

        public MainWindow()
        {
            InitializeComponent();
        }

        private void buttonコピー__C__Click(object sender, RoutedEventArgs e)
        {

        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            using (FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog())
            {
                DialogResult result = folderBrowserDialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK && !string.IsNullOrWhiteSpace(folderBrowserDialog.SelectedPath))
                {
                    string selectedPath = folderBrowserDialog.SelectedPath;
                    labelFolderPath.Content = selectedPath;
                }
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = (string)labelFolderPath.Content;
            var files = Directory.EnumerateFiles(folderPath, "*.*", SearchOption.AllDirectories)
                    .Where(s => s.EndsWith(".mp3") || s.EndsWith(".m4a"));
            var mojibakeFileList = new List<MojibakeFile>();

            foreach (string file in files)
            {
                Console.WriteLine(file);
                var tfile = TagLib.File.Create(file);

                var mojibakeFile = new MojibakeFile();
                mojibakeFile.FilePath = file;
                mojibakeFile.Album = tfile.Tag.Album;
                mojibakeFile.Title = tfile.Tag.Title;
                mojibakeFile.Artist = tfile.Tag.Artists[0];

                char fullwidthTilde = '\uFF5E';
                bool needFix = false;
                if (tfile.Tag.Album != null && tfile.Tag.Album.Contains(fullwidthTilde))
                {
                    needFix = true;
                    Console.WriteLine("Album " + tfile.Tag.Album);
                    //tfile.Tag.Album = tfile.Tag.Album.Replace('\uFF5E', '\u301C');
                }
                if (tfile.Tag.Title != null && tfile.Tag.Title.Contains(fullwidthTilde))
                {
                    needFix = true;
                    Console.WriteLine("title " + tfile.Tag.Title);
                    //tfile.Tag.Title = tfile.Tag.Title.Replace('\uFF5E', '\u301C');
                }
                // Artistsは非推奨でPerformersを使えという警告が出るが、Performersはセッターの実装が空なので使えないと思われる。
                if (tfile.Tag.Artists != null && tfile.Tag.Artists[0] != null && tfile.Tag.Artists[0].Contains(fullwidthTilde))
                {
                    needFix = true;
                    Console.WriteLine("Artists " + tfile.Tag.Artists[0]);
                    // Artistsのセッターの実装上、配列要素を部分的に更新できないので、要素1個の配列に丸ごと置き換える。
                    string replacedArtist = tfile.Tag.Artists[0].Replace('\uFF5E', '\u301C');
                    //tfile.Tag.Artists = new string[] { replacedArtist };
                }
                //tfile.Save();

                if(needFix)
                {
                    mojibakeFileList.Add(mojibakeFile);
                }
            }
            this.mojibakeFileList = mojibakeFileList;

            if (this.mojibakeFileList.Count != 0)
            {
                //string jsonId = outputJson(this.mojibakeFileList);
                showMojibakeList();
            }
        }

        private string outputJson(List<MojibakeFile> fileList)
        {
            string jsonString = JsonSerializer.Serialize(fileList);
            
            string jsonId = DateTime.Now.ToString("yyyyMMddHHmmss");

            // フォルダが存在しない場合は作成
            if (!Directory.Exists(MojibakeFolderName))
            {
                Directory.CreateDirectory(MojibakeFolderName);
            }

            // ファイルに書き込み
            File.WriteAllText(MojibakeFolderName + "\\" + jsonId + ".json", jsonString);
            return jsonId;
        }

        private void showMojibakeList()
        {
            MojibakeConfirmWindow newWindow = new MojibakeConfirmWindow(mojibakeFileList);
            newWindow.Show();
        }
    }
}
