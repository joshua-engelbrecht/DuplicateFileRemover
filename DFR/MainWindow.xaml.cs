using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.Collections;
using Hashing;
using FileFunctions;


namespace DFR
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private fileFunctions _fileFunctions = new fileFunctions();
        private RemoveFiles _removeFiles = new RemoveFiles();
        private readonly FindDuplicateFiles _duplicateFiles = new FindDuplicateFiles();
        private SearchOption _dirChoice = SearchOption.AllDirectories;
        private bool _permanentDelete = false;
        private readonly ArrayList _listOfFiles = new ArrayList();
        private readonly CompFiles _cmp = new CompFiles();
        private readonly ArrayList _deleteTheseFiles = new ArrayList();
        private delegate void UpdateProgressBarDelegate(DependencyProperty dp, Object value);
        private delegate void UpdateLabelDelegate(DependencyProperty dp, Object value);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void FindFilesClick(object sender, RoutedEventArgs e)
        {
            if(!Directory.Exists(searchDir.Text)){
                var msg = "Directory " + searchDir.Text + " does not exists.";
                MessageBox.Show(msg, "No Such Directory");
                return;
            }

            if(string.IsNullOrEmpty(searchPattern.Text))
                searchPattern.Text = "*";
            var di = new DirectoryInfo(searchDir.Text);
            var files = di.GetFiles(searchPattern.Text, _dirChoice);

            double value = 0;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = files.Length;
            progressBar1.Value = 0;
            
            var updatePbDelegate = new UpdateProgressBarDelegate(progressBar1.SetValue);
            var updateCurFile = new UpdateLabelDelegate(curFileLabel.SetValue);

            foreach (var file in files)
            {
                value += 1;
                Dispatcher.Invoke(updatePbDelegate,
                    System.Windows.Threading.DispatcherPriority.Background,
                    new object[] { ProgressBar.ValueProperty, value });
                Dispatcher.Invoke(updateCurFile,
                    System.Windows.Threading.DispatcherPriority.Background,
                    new object[] { TextBox.TextProperty, file.FullName });
                var findHash = new findMD5();
                var md5 = findHash.getFilesMD5Hash(file.FullName);
                var fStruct = new fileStruct{ checksum = md5, fileName = file.Name, fullFileName = file.FullName };
                _listOfFiles.Add(fStruct);
            }

            curFileLabel.Content = "";
            _listOfFiles.Sort(_cmp);

            var duplicates = _duplicateFiles.findDuplicates(_listOfFiles);

            foreach(fileStruct file in duplicates){
                fileStructListView.Items.Add(file);
            }

            progressBar1.Value = 0;
        }

        private void CloseBtnClick(object sender, RoutedEventArgs e)
        {
            //Need to kill all threads.
            Close();
        }

        private void ClearBtnClick(object sender, RoutedEventArgs e)
        {
            searchDir.Text = "";
            searchPattern.Text = "";
            fileStructListView.Items.Clear();
            progressBar1.Value = 0;
            _listOfFiles.RemoveRange(0, _listOfFiles.Count);
            _deleteTheseFiles.RemoveRange(0, _deleteTheseFiles.Count);
        }

        private void DirBtnClick(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
                searchDir.Text = dialog.SelectedPath;
        }

        private void SearchOptionChecked(object sender, RoutedEventArgs e)
        {
            if (TopDir.IsChecked == true)
                _dirChoice = SearchOption.TopDirectoryOnly;
            else if (AllDirs.IsChecked == true)
                _dirChoice = SearchOption.AllDirectories;
            else
                _dirChoice = SearchOption.AllDirectories;
        }

        private void MoveToChecked(object sender, RoutedEventArgs e)
        {
            if(toBin.IsChecked == true)
                _permanentDelete = false;
            if (toGone.IsChecked == true)
                _permanentDelete = true;
        }

        private void RemoveClick(object sender, RoutedEventArgs e)
        {
            _removeFiles.removeFiles(fileStructListView.SelectedItems, _permanentDelete);
            ClearBtnClick(sender, e);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            System.Windows.Data.CollectionViewSource fileStructViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("fileStructViewSource")));
            // Load data by setting the CollectionViewSource.Source property:
            // fileStructViewSource.Source = [generic data source]
        }
    }
}