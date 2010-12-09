using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.IO;
using System.Collections;
using Hashing;
using FileFunctions;
using System.Diagnostics;

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
        private bool _stop = false;
        private readonly ArrayList _listOfFiles = new ArrayList();
        private readonly CompFilesByCheckSum _cmpByCheckSum = new CompFilesByCheckSum();
        private readonly ArrayList _deleteTheseFiles = new ArrayList();
        private delegate void UpdateProgressBarDelegate(DependencyProperty dp, Object value);
        private delegate void UpdateLabelDelegate(DependencyProperty dp, Object value);

        public MainWindow()
        {
            InitializeComponent();
            AllDirs.IsChecked = true;
            toBin.IsChecked = true;
        }

        private void FindFilesClick(object sender, RoutedEventArgs e)
        {
            ClearVars();
            stopBtn.IsEnabled = true;
            clearBtn.IsEnabled = false;
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
                if (_stop)
                {
                    _stop = false;
                    break;
                }
                value += 1;
                Dispatcher.Invoke(updatePbDelegate,
                    System.Windows.Threading.DispatcherPriority.Background,
                    new object[] { ProgressBar.ValueProperty, value });

                var progressMessage = "Scanning: " + file.FullName;
                Dispatcher.Invoke(updateCurFile,
                    System.Windows.Threading.DispatcherPriority.Background,
                    new object[] { TextBox.TextProperty, progressMessage }); 


                var findHash = new findMD5();
                var md5 = findHash.getFilesMD5Hash(file.FullName);
                var fStruct = new fileStruct{ checksum = md5, fileName = file.Name, fullPath = file.FullName, creationDate=file.CreationTime };
                _listOfFiles.Add(fStruct);
            }

            curFileLabel.Text = "";
            _listOfFiles.Sort(_cmpByCheckSum);

            var duplicates = _duplicateFiles.findDuplicates(_listOfFiles);

            foreach(fileStruct file in duplicates){
                fileStructListView.Items.Add(file);
            }
            stopBtn.IsEnabled = false;
            clearBtn.IsEnabled = true;
            selectOldestBtn.IsEnabled = true;
            selectNewestBtn.IsEnabled = true;
            progressBar1.Value = 0;
        }

        private void CloseBtnClick(object sender, RoutedEventArgs e)
        {
            stopButtonClick(sender, e);
            Close();
        }

        private void ClearBtnClick(object sender, RoutedEventArgs e)
        {
            searchDir.Text = "";
            searchPattern.Text = "";
            ClearVars();
        }

        private void ClearVars()
        {
            _deleteTheseFiles.Clear();
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

        private void RemoveBtnClick(object sender, RoutedEventArgs e)
        {
            if (fileStructListView.SelectedItems.Count < 1)
            {
                MessageBox.Show("No files selected.", "No Files Selected");
                return;
            }
            var updatePbDelegate = new UpdateProgressBarDelegate(progressBar1.SetValue);
            var updateCurFile = new UpdateLabelDelegate(curFileLabel.SetValue);
            
            double value = 0;
            progressBar1.Minimum = 0;
            progressBar1.Maximum = fileStructListView.SelectedItems.Count;
            progressBar1.Value = 0;
            foreach (fileStruct file in fileStructListView.SelectedItems)
            {
                if (_stop)
                {
                    ClearVars();
                    _stop = false;
                    break;
                }
                Dispatcher.Invoke(updatePbDelegate,
                    System.Windows.Threading.DispatcherPriority.Background,
                    new object[] { ProgressBar.ValueProperty, value });

                var progressMessage = "Deleting: " + file.fullPath;
                Dispatcher.Invoke(updateCurFile,
                    System.Windows.Threading.DispatcherPriority.Background,
                    new object[] { TextBox.TextProperty, progressMessage }); 
                
                _removeFiles.removeFiles(file, _permanentDelete);
            }

            selectOldestBtn.IsEnabled = false;
            selectNewestBtn.IsEnabled = false;
            curFileLabel.Text = "";
            ClearVars();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            System.Windows.Data.CollectionViewSource fileStructViewSource = ((System.Windows.Data.CollectionViewSource)(this.FindResource("fileStructViewSource")));
        }

        private void stopButtonClick(object sender, RoutedEventArgs e)
        {
            _stop = true;
        }

        private void fileStructListViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (fileStructListView.SelectedItems.Count > 0)
            {
                removeBtn.IsEnabled = true;
                clearSelectionBtn.IsEnabled = true;
            }
            else
            {
                removeBtn.IsEnabled = false;
                clearSelectionBtn.IsEnabled = false;
            }
        }

        private void clearSelectionButtonClick(object sender, RoutedEventArgs e)
        {
            fileStructListView.SelectedItems.Clear();
        }

        private void selectNewestButtonClick(object sender, RoutedEventArgs e)
        {
            fileStructListView.SelectedItems.Clear();
            var toSelect = _fileFunctions.selectFiles(fileStructListView.Items, false);
            foreach(fileStruct file in toSelect){
                fileStructListView.SelectedItems.Add(file);
            }
        }

        private void selectOldestButtonClick(object sender, RoutedEventArgs e)
        {
            fileStructListView.SelectedItems.Clear();
            var toSelect = _fileFunctions.selectFiles(fileStructListView.Items, true);
            foreach (fileStruct file in toSelect)
            {
                fileStructListView.SelectedItems.Add(file);
            }
        }
    }
}