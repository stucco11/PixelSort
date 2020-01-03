using PixelSort.EventHandling;
using System.Windows.Input;
 
namespace PixelSort.ViewModel
{
    public class BasePageViewModel : BaseViewModel, IPageViewModel
    {
        private ICommand _goToSettings;

        public ICommand GoToSettings
        {
            get
            {
                return _goToSettings ?? (_goToSettings = new RelayCommand(x =>
                {
                    Mediator.Notify("GoToSettings", "");
                }));
            }
        }
    }
}
