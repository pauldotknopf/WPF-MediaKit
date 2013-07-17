using System;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using IWin32Window=System.Windows.Forms.IWin32Window;

namespace SampleApplication.Controls
{
    public class FolderDialog : FrameworkElement, IWin32Window
    {
        #region FolderPath

        public static readonly DependencyProperty FolderPathProperty =
            DependencyProperty.Register("FolderPath", typeof(string), typeof(FolderDialog),
                new FrameworkPropertyMetadata("",
                    FrameworkPropertyMetadataOptions.None));

        public string FolderPath
        {
            get { return (string)GetValue(FolderPathProperty); }
            set { SetValue(FolderPathProperty, value); }
        }

        #endregion

        public static readonly RoutedCommand SelectFolderCommand = new RoutedCommand();

        public FolderDialog()
        {
            CommandBindings.Add(new CommandBinding(SelectFolderCommand, OnSelectFolderCommandExecuted));
        }

        private void OnSelectFolderCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            SelectFolder();
        }

        public void SelectFolder()
        {
            using(var dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                FolderPath = dialog.SelectedPath;
            }
        }

        #region IWin32Window Members

        IntPtr IWin32Window.Handle
        {
            get 
            {
                IntPtr ptr = new WindowInteropHelper(Window.GetWindow(this)).Handle;
                return ptr;
            }
        }

        #endregion
    }
}
