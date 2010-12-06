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
        private SearchOption dirChoice = SearchOption.AllDirectories;
        private ArrayList listOfFiles = new ArrayList();
        private CompFiles cmp = new CompFiles();

        public MainWindow()
        {
            InitializeComponent();
        }
        private delegate void UpdateProgressBarDelegate(System.Windows.DependencyProperty dp, Object value);

        private void findFiles_Click(object sender, RoutedEventArgs e)
        {
            
            //            var listOfFiles = fFunctions.getFiles(searchPattern.Text, searchDir.Text, dirChoice);
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

            table.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            table.VerticalAlignment = System.Windows.VerticalAlignment.Top;
            table.ShowGridLines = true;
            table.Background = new SolidColorBrush(Colors.LightSteelBlue);
            //Build Grid
            ColumnDefinition delCol = new ColumnDefinition();
            ColumnDefinition nameCol = new ColumnDefinition();
            ColumnDefinition fullPathCol = new ColumnDefinition();
            ColumnDefinition groupCol = new ColumnDefinition();
            
            table.ColumnDefinitions.Add(delCol);
            table.ColumnDefinitions.Add(nameCol);
            table.ColumnDefinitions.Add(fullPathCol);
            table.ColumnDefinitions.Add(groupCol);

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

            foreach (fileStruct file in listOfFiles)
            {
                var newRow = new RowDefinition();
                table.RowDefinitions.Add(newRow);
                var chb = new CheckBox();
                chb.Name = "delete_" + row;
                chb.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                chb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                Grid.SetRow(chb, row);
                Grid.SetColumn(chb, 0);
                table.Children.Add(chb);

                var name = new TextBox();
                name.Name = "name_" + row;
                name.Text = file.fileName;
                name.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                name.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                Grid.SetColumn(name, 1);
                Grid.SetRow(name, row);
                table.Children.Add(name);

                var path = new TextBox();
                path.Name = "path_" + row;
                path.Text = file.fullFileName;
                path.VerticalAlignment = System.Windows.VerticalAlignment.Center;
                path.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                Grid.SetColumn(path, 2);
                Grid.SetRow(path, row);
                table.Children.Add(path);

                var grp = new TextBox();
                grp.Name = "path_" + row;
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

        private void button2_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            progressBar1.Value = 0;
        }

        private void button4_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
                searchDir.Text = dialog.SelectedPath;
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            if (TopDir.IsChecked == true)
                dirChoice = SearchOption.TopDirectoryOnly;
            else if (AllDirs.IsChecked == true)
                dirChoice = SearchOption.AllDirectories;
            else
                dirChoice = SearchOption.AllDirectories;
        }
    }
}