using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace SandCastle.Core.ViewModels
{
    public class MainViewModel : MvxViewModel
    {
        #region Fields

        private readonly IMvxNavigationService _NavigationService;

        #endregion Fields

        #region Constructors

        public MainViewModel(IMvxNavigationService navigationService)
        {
            _NavigationService = navigationService;
        }

        #endregion Constructors

        #region Properties

        #endregion Properties

        #region Commands

        private IMvxCommand _ClientCommand;
        public IMvxCommand ClientCommand => _ClientCommand ?? new MvxAsyncCommand(async () => await _NavigationService.Navigate<ClientViewModel>());

        private IMvxCommand _ServerCommand;
        public IMvxCommand ServerCommand => _ServerCommand ?? new MvxAsyncCommand(async () => await _NavigationService.Navigate<ServerViewModel>());

        #endregion Commands
    }
}