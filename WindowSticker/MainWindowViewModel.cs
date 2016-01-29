using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ReactiveUI;
using System.Reactive.Linq;
using WindowSticker.ViewModels;
using WindowSticker.Services;

namespace WindowSticker
{
    public class MainWindowViewModel : ReactiveObject
    {
        public ReactiveCommand<object> AddLayoutCmd { get; set; }

        public ReactiveCommand<object> RestoreLayoutCmd { get; set; }
        public ReactiveCommand<object> DeleteLayoutCmd { get; set; }

        public ReactiveList<WindowLayoutGroup> SavedLayouts 
        { 
            get; 
            private set; 
        }

        private WindowLayoutGroup _selectedLayout;
        public WindowLayoutGroup SelectedLayout 
        {
            get { return _selectedLayout; }
            set 
            {
                if (_selectedLayout != null)
                    _selectedLayout.IsEditingName = false;

                this.RaiseAndSetIfChanged(ref _selectedLayout, value); 
            }
        }

        private WindowManagerService _windowService = new WindowManagerService();

        public MainWindowViewModel()
        {
            SavedLayouts = new ReactiveList<WindowLayoutGroup>();

            AddLayoutCmd = ReactiveCommand.Create(SavedLayouts.CountChanged.Select(count => count < 4).StartWith(true));
            AddLayoutCmd.Subscribe(_ => SaveLayout());

            RestoreLayoutCmd = ReactiveCommand.Create(this.WhenAnyValue(vm => vm.SelectedLayout).Select(layout => layout != null));
            RestoreLayoutCmd.Subscribe(_ => RestoreLayout());

            DeleteLayoutCmd = ReactiveCommand.Create(this.WhenAnyValue(vm => vm.SelectedLayout).Select(layout => layout != null));
            DeleteLayoutCmd.Subscribe(_ => DeleteLayout());
        }

        private void SaveLayout()
        {
            SavedLayouts.Add(new WindowLayoutGroup(_windowService));
        }

        private void RestoreLayout()
        {
            SelectedLayout.RestoreAllWindows();
        }

        private void DeleteLayout()
        {
            SavedLayouts.Remove(SelectedLayout);
        }
    }
}
