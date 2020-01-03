using PixelSort.EventHandling;
using System.Windows.Input;
 
namespace PixelSort.ViewModel
{
    public class SettingsViewModel : BaseViewModel, IPageViewModel
    {
        private ICommand _goToMain;

        public ICommand GoToMain
        {
            get
            {
                return _goToMain ?? (_goToMain = new RelayCommand(x =>
                {
                    Mediator.Notify("GoToMain", "");
                }));
            }
        }
    }
}