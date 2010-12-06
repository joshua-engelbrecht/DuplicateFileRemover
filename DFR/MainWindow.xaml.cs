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

namespace DFR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DateTime start; 
        private DateTime stop;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            start = DateTime.Now;
            textBox2.Text = "Start Time: " + start + "\n";
            textBox1.Text = "";
            ListFiles();
            stop = DateTime.Now;
            textBox2.Text += "Stop Time: " + stop + "\n";
            var totalTime = stop - start;
            textBox2.Text += "Total Time: " + Math.Floor(totalTime.TotalMinutes) + " minutes\n";
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
            var dir = "C:/Users/joshe/Music/";
            DirectoryInfo di = new DirectoryInfo(dir);
            FileInfo[] rgFiles = di.GetFiles("*.mp3", SearchOption.AllDirectories);
            //FileInfo[] rgFiles = di.GetFiles("*.mp3");
            var fDF = new FindDupFiles();
            double value = 0;

            progressBar1.Minimum = 0;
            progressBar1.Maximum = rgFiles.Length;
            progressBar1.Value = 0;
            var updatePbDelegate = new UpdateProgressBarDelegate(progressBar1.SetValue);
            var updateTbDelegate = new UpdateTextBoxDelegate(textBox1.SetCurrentValue);
            foreach (FileInfo fi in rgFiles)
            {
                var md = fDF.getFilesMD5Hash(fi.FullName);
                value += 1;
                list += fi.Name + " : " + md + "\n";
                var entry = fi.Name + " : " + md + "\n";
                Dispatcher.Invoke(updatePbDelegate,
                    System.Windows.Threading.DispatcherPriority.Background,
                    new object[] { ProgressBar.ValueProperty, value });
                Dispatcher.Invoke(updateTbDelegate,
                    System.Windows.Threading.DispatcherPriority.Background,
                    new object[] {TextBox.TextProperty, list});
            }
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            textBox1.Text = "";
            progressBar1.Value = 0;
        }
    }
}
