using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using WindowSticker.Services;

namespace WindowSticker.ViewModels
{
    public class WindowLayoutGroup : ReactiveObject
    {
        public List<SavedWindow> SavedWindows 
        { 
            get; 
            private set; 
        }

        public string SavedDetails
        {
            get { return SavedWindows.Count + " saved windows"; }
        }

        private string _name;
        public string Name
        {
            get { return _name; }
            set { this.RaiseAndSetIfChanged(ref _name, value); }
        }

        private bool _isEditingName = false;
        public bool IsEditingName
        {
            get { return _isEditingName; }
            set { this.RaiseAndSetIfChanged(ref _isEditingName, value); }
        }

        public string CreatedDate { get; private set; }

        private WindowManagerService _windowService;

        public WindowLayoutGroup(WindowManagerService wndService)
        {
            CreatedDate = DateTime.Now.ToShortTimeString();
            Name = CreatedDate;
            SavedWindows = new List<SavedWindow>();

            _windowService = wndService;

            SaveAllWindows();
        }

        private void SaveAllWindows()
        {
            SavedWindows.Clear();

            var windows = _windowService.GetAllActiveWindows();
            foreach (var wnd in windows)
            {
                Rect rect = new Rect();
                if (WindowManagerService.GetWindowRect(wnd, ref rect))
                {
                    SavedWindows.Add(new SavedWindow(wnd, rect, WindowManagerService.IsWindowMaximized(wnd)));
                }
            }
        }

        public void RestoreAllWindows()
        {
            foreach (var wnd in SavedWindows)
            {
                _windowService.SetWindowPosition(wnd.WndHandle, wnd.Rectangle, wnd.IsMaximized);
            }
        }
    }

    public class SavedWindow
    {
        public IntPtr WndHandle { get; private set; }
        public Rect Rectangle { get; private set; }

        public bool IsMaximized { get; private set; }

        public SavedWindow(IntPtr handle, Rect rect, bool isMax)
        {
            WndHandle = handle;
            Rectangle = rect;
            IsMaximized = isMax;
        }
    }
}