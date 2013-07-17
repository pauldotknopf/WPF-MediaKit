using System;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace SampleApplication.Controls
{
    public class FileDialog : FrameworkElement
    {
        #region FilePath

        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(FileDialog),
                new FrameworkPropertyMetadata("",
                    FrameworkPropertyMetadataOptions.None));

        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        #endregion

        public static readonly RoutedCommand SelectFileCommand = new RoutedCommand();

        public FileDialog()
        {
            CommandBindings.Add(new CommandBinding(SelectFileCommand, OnSelectFileCommandExecuted, OnCanExecuteFileCommand));
        }

        private void OnSelectFileCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SelectFile();
        }

        private void OnCanExecuteFileCommand(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        public void SelectFile()
        {
            var dialog = new OpenFileDialog();
            
            if(dialog.ShowDialog(Window.GetWindow(this)).Value)
            {

                FilePath = dialog.FileName;
            }
        }
    }
}
