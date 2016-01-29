using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WindowSticker
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindowViewModel ViewModel { get; set; }
        private readonly TrayIcon _trayIcon;

        public MainWindow()
        {
            InitializeComponent();

            _trayIcon = new TrayIcon();
            _trayIcon.Invoked += TrayIcon_Invoked;

            SourceInitialized += (s, e) => UpdateTheme();

            ViewModel = new MainWindowViewModel();
            ViewModel.AddLayoutCmd.Subscribe(_ => UpdateWindowPosition());
            ViewModel.DeleteLayoutCmd.Subscribe(_ => UpdateWindowPosition());

            this.DataContext = ViewModel;

            LayoutList.MouseDoubleClick += LayoutList_MouseDoubleClick;
        }

        void TrayIcon_Invoked()
        {
            UpdateTheme();
            UpdateWindowPosition();
            this.ShowwithAnimation();
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            StopEditing();
        }

        void LayoutList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (ViewModel.SelectedLayout != null)
                ViewModel.SelectedLayout.IsEditingName = true;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.HideWithAnimation();
            StopEditing();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                this.HideWithAnimation();
            }
            if (e.Key == Key.Enter)
            {
                StopEditing();
            }
        }

        private void StopEditing()
        {
            if (ViewModel.SelectedLayout != null)
                ViewModel.SelectedLayout.IsEditingName = false;
        }

        private void UpdateTheme()
        {
            if (Environment.OSVersion.Version.Minor > 1 || Environment.OSVersion.Version.Major > 6)
            {
                ThemeService.UpdateThemeResources(Resources);
                this.EnableBlur();
            }
            UpdateWindowPosition();
        }

        private void UpdateWindowPosition()
        {
            LayoutRoot.UpdateLayout();
            LayoutRoot.Measure(new Size(double.PositiveInfinity, MaxHeight));
            Height = LayoutRoot.DesiredSize.Height;

            var taskbarScreenWorkArea = TaskbarService.TaskbarScreen.WorkingArea;
            var taskbarPosition = TaskbarService.TaskbarPosition;
            Left = (taskbarPosition == TaskbarPosition.Left) ? (taskbarScreenWorkArea.Left / this.DpiWidthFactor()) : (taskbarScreenWorkArea.Right / this.DpiWidthFactor()) - Width;
            Top = (taskbarPosition == TaskbarPosition.Top) ? (taskbarScreenWorkArea.Top / this.DpiHeightFactor()) : (taskbarScreenWorkArea.Bottom / this.DpiHeightFactor()) - Height;
        }
    }
}
