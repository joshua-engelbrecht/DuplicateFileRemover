using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Threading;
using Hashing;
using FileFunctions;

namespace DFR
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

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Text = "";
            ListFiles();
            progressBar1.Value = 0;
        }

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);
        private delegate void UpdateTextBoxDelegate(System.Windows.DependencyProperty dp, Object value);

        private void ListFiles()
        {
            var list = "";
            var dir = "F:/Music/Amazon MP3/";
            DirectoryInfo di = new DirectoryInfo(dir);
            FileInfo[] rgFiles = di.GetFiles("*.mp3", SearchOption.AllDirectories);
            //FileInfo[] rgFiles = di.GetFiles("*.mp3");
            var fDF = new FileFuncations.FindDupFiles();
            double value = 0;

            progressBar1.Minimum = 0;
            progressBar1.Maximum = rgFiles.Length;
            progressBar1.Value = 0;
            var updatePbDelegate = new UpdateProgressBarDelegate(progressBar1.SetValue);
            var updateTbDelegate = new UpdateTextBoxDelegate(textBox1.SetCurrentValue);
            var hashFun = new Hashing.findHash();
            foreach (FileInfo fi in rgFiles)
            {
                var md = hashFun.getFilesMD5Hash(fi.FullName);
                value += 1;
                list += fi.Name + " : " + md + "\n";
                Dispatcher.Invoke(updatePbDelegate,
                    System.Windows.Threading.DispatcherPriority.Background,
                    new object[] { ProgressBar.ValueProperty, value });
            }

        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Text = "";
            progressBar1.Value = 0;
        }
    }
}
