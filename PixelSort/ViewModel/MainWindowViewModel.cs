using PixelSort.EventHandling;
using System.Collections.Generic;
using System.Linq;

// Code for this Window Switcher found here - https://www.technical-recipes.com/2018/navigating-between-views-in-wpf-mvvm/
namespace PixelSort.ViewModel
{
    public class MainWindowViewModel : BaseViewModel
    {
        private IPageViewModel _currentPageViewModel;
        private List<IPageViewModel> _pageViewModels;

        public List<IPageViewModel> PageViewModels
        {
            get
            {
                if (_pageViewModels == null)
                    _pageViewModels = new List<IPageViewModel>();

                return _pageViewModels;
            }
        }

        public IPageViewModel CurrentPageViewModel
        {
            get
            {
                return _currentPageViewModel;
            }
            set
            {
                _currentPageViewModel = value;
                OnPropertyChanged("CurrentPageViewModel");
            }
        }

        private void ChangeViewModel(IPageViewModel viewModel)
        {
            if (!PageViewModels.Contains(viewModel))
                PageViewModels.Add(viewModel);

            CurrentPageViewModel = PageViewModels
                .FirstOrDefault(vm => vm == viewModel);
        }

        private void GoToMain(object obj)
        {
            ChangeViewModel(PageViewModels[0]);
        }

        private void GoToSettings(object obj)
        {
            ChangeViewModel(PageViewModels[1]);
        }

        public MainWindowViewModel()
        {
            // Add available pages and set page
            PageViewModels.Add(new BasePageViewModel());
            PageViewModels.Add(new SettingsViewModel());

            CurrentPageViewModel = PageViewModels[0];

            Mediator.Subscribe("GoToMain", GoToMain);
            Mediator.Subscribe("GoToSettings", GoToSettings);
        }
    }
}
