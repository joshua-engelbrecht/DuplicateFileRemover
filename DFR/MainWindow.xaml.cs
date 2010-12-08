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

        public MainWindow()
        {
            InitializeComponent();
            moveOrDelete.Content = "";
        }
        private delegate void UpdateProgressBarDelegate(DependencyProperty dp, Object value);

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

            foreach (var file in files)
            {
                value += 1;
                Dispatcher.Invoke(updatePbDelegate,
                    System.Windows.Threading.DispatcherPriority.Background,
                    new object[] { ProgressBar.ValueProperty, value });
                var findHash = new findMD5();
                var md5 = findHash.getFilesMD5Hash(file.FullName);
                var fStruct = new fileStruct{ checksum = md5, fileName = file.Name, fullFileName = file.FullName };
                _listOfFiles.Add(fStruct);
            }

            table.HorizontalAlignment = HorizontalAlignment.Left;
            table.VerticalAlignment = VerticalAlignment.Top;
            table.ShowGridLines = false;
            table.Background = new SolidColorBrush(Colors.White);
            //Build Grid
            var delCol = new ColumnDefinition();
            var nameCol = new ColumnDefinition();
            var fullPathCol = new ColumnDefinition();
            var groupCol = new ColumnDefinition();
            
            table.ColumnDefinitions.Add(delCol);
            table.ColumnDefinitions.Add(nameCol);
            table.ColumnDefinitions.Add(fullPathCol);
            table.ColumnDefinitions.Add(groupCol);

            var firstRow = new RowDefinition();
            table.RowDefinitions.Add(firstRow);
            //Add Column Headers
            var header1 = new TextBlock
                              {
                                  Text = "Delete",
                                  HorizontalAlignment = HorizontalAlignment.Center,
                                  VerticalAlignment = VerticalAlignment.Center
                              };

            Grid.SetColumn(header1, 0);
            Grid.SetRow(header1, 0);
            table.Children.Add(header1);

            var header2 = new TextBlock
                              {
                                  Text = "File Name",
                                  HorizontalAlignment = HorizontalAlignment.Center,
                                  VerticalAlignment = VerticalAlignment.Center
                              };

            Grid.SetColumn(header2, 1);
            Grid.SetRow(header2, 0);
            table.Children.Add(header2);

            var header3 = new TextBlock
                              {
                                  Text = "Full Path",
                                  HorizontalAlignment = HorizontalAlignment.Center,
                                  VerticalAlignment = VerticalAlignment.Center
                              };

            Grid.SetColumn(header3, 2);
            Grid.SetRow(header3, 0);
            table.Children.Add(header3);

            var header4 = new TextBlock
                              {
                                  Text = "DFR #",
                                  HorizontalAlignment = HorizontalAlignment.Center,
                                  VerticalAlignment = VerticalAlignment.Center
                              };

            Grid.SetColumn(header4, 3);
            Grid.SetRow(header4, 0);
            table.Children.Add(header4);

            var row = 1;

            //Sort Files According to Hash
            _listOfFiles.Sort(_cmp);
            var duplicates = _duplicateFiles.findDuplicates(_listOfFiles);

            foreach (fileStruct file in duplicates)
            {
                var newRow = new RowDefinition {Name = "row_" + row};
                table.RowDefinitions.Add(newRow);

                var chb = new CheckBox {Name = "delete_" + row};
                chb.Click += CheckboxClick;
                chb.VerticalAlignment = VerticalAlignment.Center;
                chb.HorizontalAlignment = HorizontalAlignment.Center;
                Grid.SetRow(chb, row);
                Grid.SetColumn(chb, 0);
                
                table.Children.Add(chb);

                var name = new TextBox
                               {
                                   IsReadOnly = true,
                                   Width = (double) 450,
                                   BorderThickness = new Thickness(0),
                                   Name = "name_" + row,
                                   Text = file.fileName,
                                   VerticalAlignment = VerticalAlignment.Center,
                                   HorizontalAlignment = HorizontalAlignment.Left
                               };
                Grid.SetColumn(name, 1);
                Grid.SetRow(name, row);
                table.Children.Add(name);

                var path = new TextBox
                               {
                                   IsReadOnly = true,
                                   BorderThickness = new Thickness(0),
                                   Width = (double) 450,
                                   Name = "path_" + row,
                                   Text = file.fullFileName,
                                   VerticalAlignment = VerticalAlignment.Center,
                                   HorizontalAlignment = HorizontalAlignment.Left
                               };
                Grid.SetColumn(path, 2);
                Grid.SetRow(path, row);
                table.Children.Add(path);

                var grp = new TextBox
                              {
                                  BorderThickness = new Thickness(0),
                                  IsReadOnly = true,
                                  Width = (double) 50,
                                  Name = "grp_" + row,
                                  Text = file.duplicationNumber.ToString(),
                                  VerticalAlignment = VerticalAlignment.Center,
                                  HorizontalAlignment = HorizontalAlignment.Center
                              };
                Grid.SetColumn(grp, 3);
                Grid.SetRow(grp, row);
                table.Children.Add(grp);

                row++;
            }

            progressBar1.Value = 0;
        }

        private void CloseBtnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ClearBtnClick(object sender, RoutedEventArgs e)
        {
            searchDir.Text = "";
            searchPattern.Text = "";
            logBlock.Text = "";
            table.RowDefinitions.Clear();
            table.ColumnDefinitions.Clear();
            progressBar1.Value = 0;

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
            _removeFiles.removeFiles(_deleteTheseFiles, _permanentDelete);
        }

        private void CheckboxClick(object sender, RoutedEventArgs e)
        {
            if (_permanentDelete)
                moveOrDelete.Content = "You are Permanently Deleting: \n";
            else
                moveOrDelete.Content = "You are Moving to the Recycle Bin: \n";

            var snd = (CheckBox)sender;
            var nm = snd.Name;
            var splits = nm.Split('_');
            var row = int.Parse(splits[1]);
            var elements = from UIElement element in table.Children
                          where element is TextBox &&
                          Grid.GetRow(element) == row &&
                          Grid.GetColumn(element) == 2
                          select element as TextBox;
            elements.ToList();
            if (snd.IsChecked == true)
            {
                var toAdd = elements.First();
                _deleteTheseFiles.Add(toAdd.Text);
            }
            if (snd.IsChecked == false)
            {
                var toRemove = elements.First();
                _deleteTheseFiles.Remove(toRemove.Text);
            }

            if(_deleteTheseFiles.Count > 0)
                remove.IsEnabled = true;
            logBlock.Text = "";
            foreach (string name in _deleteTheseFiles)
            {
                logBlock.Text += name + "\n";
            }
        }

    }
}