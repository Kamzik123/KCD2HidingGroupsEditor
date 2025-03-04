using KCD2HidingGroupsEditor.Skin;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace KCD2HidingGroupsEditor
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

        public void TestFile()
        {
            SkinFile file = new(@"C:\Users\James\Desktop\m_head_042.skin");
            file.PatchColors(0xFA);
            file.Write(@"C:\Users\James\Desktop\m_head_042_patched.skin");
        }

        private void Button_Patch_Click(object sender, RoutedEventArgs e)
        {
            bool IsInputFolder = false;
            bool IsOutputFolder = false;

            if (Directory.Exists(TextBox_Input.Text))
            {
                IsInputFolder = true;
            }

            if (Directory.Exists(TextBox_Output.Text))
            {
                IsOutputFolder = true;
            }

            if (!IsOutputFolder)
            {
                if (!TextBox_Output.Text.EndsWith(".skin", StringComparison.InvariantCultureIgnoreCase))
                {
                    IsOutputFolder = true;

                    if (!Directory.Exists(TextBox_Output.Text))
                    {
                        Directory.CreateDirectory(TextBox_Output.Text);
                    }
                }
            }

            if (IsInputFolder && !IsOutputFolder)
            {
                MessageBox.Show("When an input folder is specified, you must specify an output folder instead of a file.", "Error");
            }

            bool[] groups = new bool[] { !(bool)CheckBox_Bit1.IsChecked!, !(bool)CheckBox_Bit2.IsChecked!, !(bool)CheckBox_Bit3.IsChecked!, !(bool)CheckBox_Bit4.IsChecked!,
                                         !(bool)CheckBox_Bit5.IsChecked!, !(bool)CheckBox_Bit6.IsChecked!, !(bool)CheckBox_Bit7.IsChecked!, !(bool)CheckBox_Bit8.IsChecked!};
            uint hidingGroups = groups.ConstructByte();

            if (IsInputFolder)
            {
                BatchProcessFiles(TextBox_Input.Text, TextBox_Output.Text, hidingGroups);
            }
            else
            {
                ProcessFile(TextBox_Input.Text, TextBox_Output.Text, hidingGroups, IsOutputFolder);
            }
        }

        private void BatchProcessFiles(string inputFolder, string outputFolder, uint hidingGroups)
        {
            foreach (var file in Directory.EnumerateFiles(inputFolder, "*.skin"))
            {
                ProcessFile(file, outputFolder, hidingGroups, true);
            }
        }

        private void ProcessFile(string fileName, string outputFileName, uint hidingGroups, bool IsOutputFolder)
        {
            try
            {
                if (IsOutputFolder)
                {
                    outputFileName = Path.Combine(outputFileName, Path.GetFileName(fileName));
                }

                SkinFile file = new(fileName);
                file.PatchColors(hidingGroups);
                file.Write(outputFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void Button_InputPicker_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new();
            dialog.Filter = "Kingdom Come: Deliverance II Model file (*.skin)|*.skin";
            dialog.Multiselect = false;
            dialog.Title = "Input file";
            dialog.CheckFileExists = true;
            dialog.CheckPathExists = true;
            
            if ((bool)dialog.ShowDialog()!)
            {
                TextBox_Input.Text = dialog.FileName;
            }
        }

        private void Button_OutputPicker_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new OpenFolderDialog();
            dialog.Multiselect = false;
            dialog.Title = "Output folder";

            if ((bool)dialog.ShowDialog()!)
            {
                TextBox_Output.Text = dialog.FolderName;
            }
        }

        private void TextBox_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                if (files.Length > 0)
                {
                    TextBox? textBox = sender as TextBox;

                    textBox!.Text = files[0];
                }
            }
        }

        private void TextBox_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
    }
}