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
using Microsoft.Win32;
using System.Collections;
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
        private fileFunctions fFunctions = new fileFunctions();
        private FindDuplicateFiles duplicateFiles = new FindDuplicateFiles();
        private SearchOption dirChoice = SearchOption.AllDirectories;
        private bool permanentDelete = false;
        private ArrayList listOfFiles = new ArrayList();
        private CompFiles cmp = new CompFiles();
        private ArrayList deleteTheseFiles = new ArrayList();

        public MainWindow()
        {
            InitializeComponent();
        }
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);

        private void findFiles_Click(object sender, RoutedEventArgs e)
        {
            var di = new DirectoryInfo(searchDir.Text);
            var files = di.GetFiles(searchPattern.Text, dirChoice);

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
                listOfFiles.Add(fStruct);
            }

            //Sort Files According to Hash
            listOfFiles.Sort(cmp);
            var duplicates = duplicateFiles.findDuplicates(listOfFiles);
//            var duplicates = listOfFiles;
            table.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            table.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            table.ShowGridLines = false;
            table.Background = new SolidColorBrush(Colors.White);
            //Build Grid
            ColumnDefinition delCol = new ColumnDefinition();
            ColumnDefinition nameCol = new ColumnDefinition();
            ColumnDefinition fullPathCol = new ColumnDefinition();
            ColumnDefinition groupCol = new ColumnDefinition();
            
            table.ColumnDefinitions.Add(delCol);
            table.ColumnDefinitions.Add(nameCol);
            table.ColumnDefinitions.Add(fullPathCol);
            table.ColumnDefinitions.Add(groupCol);

            var firstRow = new RowDefinition();
            table.RowDefinitions.Add(firstRow);
            //Add Column Headers
            TextBlock header1 = new TextBlock();
            header1.Text = "Delete";
            header1.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            header1.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            Grid.SetColumn(header1, 0);
            Grid.SetRow(header1, 0);
            table.Children.Add(header1);

            var header2 = new TextBlock();
            header2.Text = "File Name";
            header2.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            header2.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            Grid.SetColumn(header2, 1);
            Grid.SetRow(header2, 0);
            table.Children.Add(header2);

            var header3 = new TextBlock();
            header3.Text = "Full Path";
            header3.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            header3.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            Grid.SetColumn(header3, 2);
            Grid.SetRow(header3, 0);
            table.Children.Add(header3);

            var header4 = new TextBlock();
            header4.Text = "DFR #";
            header4.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            header4.VerticalAlignment = System.Windows.VerticalAlignment.Center;

            Grid.SetColumn(header4, 3);
            Grid.SetRow(header4, 0);
            table.Children.Add(header4);

            var row = 1;
            

            foreach (fileStruct file in duplicates)
            {
                var newRow = new RowDefinition();
                newRow.Name = "row_" + row.ToString();
                table.RowDefinitions.Add(newRow);

                var chb = new CheckBox();
                chb.Name = "delete_" + row;
                chb.Click += checkbox_Click;
                chb.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                chb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                Grid.SetRow(chb, row);
                Grid.SetColumn(chb, 0);
                
                table.Children.Add(chb);

                var name = new TextBox();
                name.IsReadOnly = true;
                name.Width = (double)450;
                name.BorderThickness = new Thickness((double)0);
                name.Name = "name_" + row;
                name.Text = file.fileName;
                name.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                name.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                Grid.SetColumn(name, 1);
                Grid.SetRow(name, row);
                table.Children.Add(name);

                var path = new TextBox();
                path.IsReadOnly = true;
                path.BorderThickness = new Thickness((double)0);
                path.Width = (double)450;
                path.Name = "path_" + row;
                path.Text = file.fullFileName;
                path.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                path.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
                Grid.SetColumn(path, 2);
                Grid.SetRow(path, row);
                table.Children.Add(path);

                var grp = new TextBox();
                grp.BorderThickness = new Thickness((double)0);
                grp.IsReadOnly = true;
                grp.Width = (double)50;
                grp.Name = "grp_" + row;
                grp.Text = file.duplicationNumber.ToString();
                grp.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                grp.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                Grid.SetColumn(grp, 3);
                Grid.SetRow(grp, row);
                table.Children.Add(grp);

                row++;
            }

            progressBar1.Value = 0;
        }

        private void closeBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            searchDir.Text = "";
            searchPattern.Text = "";
            table.RowDefinitions.Clear();
            progressBar1.Value = 0;

        }

        private void dirBtn_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
                searchDir.Text = dialog.SelectedPath;
        }

        private void searchOption_Checked(object sender, RoutedEventArgs e)
        {
            if (TopDir.IsChecked == true)
                dirChoice = SearchOption.TopDirectoryOnly;
            else if (AllDirs.IsChecked == true)
                dirChoice = SearchOption.AllDirectories;
            else
                dirChoice = SearchOption.AllDirectories;
        }

        private void moveTo_Checked(object sender, RoutedEventArgs e)
        {
            if(toBin.IsChecked == true)
                permanentDelete = false;
            if (toGone.IsChecked == true)
                permanentDelete = false;
        }

        private void remove_Click(object sender, RoutedEventArgs e)
        {
            var deleteThese = new ArrayList();
            var numOfRows = table.RowDefinitions.Count;
        }

        private void checkbox_Click(object sender, RoutedEventArgs e)
        {
            var snd = (CheckBox)sender;
            var nm = snd.Name;
            var splits = nm.Split('_');
            var row = int.Parse(splits[1]);
            var elements = from UIElement element in table.Children
                          where element is TextBox &&
                          Grid.GetRow(element) == row &&
                          Grid.GetColumn(element) == 2
                          select element as TextBox;
            elements.ToList<TextBox>();
            if (snd.IsChecked == true)
            {
                var toAdd = elements.First<TextBox>();
                deleteTheseFiles.Add(toAdd.Text);
            }
            if (snd.IsChecked == false)
            {
                var toRemove = elements.First<TextBox>();
                deleteTheseFiles.Remove(toRemove.Text);
            }
            logBlock.Text = "";
            foreach (string name in deleteTheseFiles)
            {
                logBlock.Text += name + "\n";
            }
        }

    }
}