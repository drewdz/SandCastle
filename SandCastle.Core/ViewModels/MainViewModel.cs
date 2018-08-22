using MvvmCross.Navigation;
using MvvmCross.ViewModels;

using SandCastle.Core.Models;

using System.Collections.Generic;

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

        private List<MenuItem> _MenuItems = null;
        public List<MenuItem> MenuItems
        {
            get => _MenuItems;
            set => SetProperty(ref _MenuItems, value);
        }

        private MenuItem _MenuItem;
        public MenuItem MenuItem
        {
            get => _MenuItem;
            set
            {
                SetProperty(ref _MenuItem, value);
                if (value != null) value.Action?.Invoke();
                //  TODO: clear the selected item so that we can click it again
            }
        }

        #endregion Properties

        #region Lifecycle

        public override void ViewAppeared()
        {
            MvxNotifyTask.Create(async () =>
            {
                //  create a list of menu items
                var menu = new List<MenuItem>
                {
                    new MenuItem
                    {
                        Title = "Dialogs",
                        Description = "Demonstrate the use of the dialogs plugins.",
                        Action = async () => await _NavigationService.Navigate<DialogsDemoViewModel>()
                    },
                    new MenuItem
                    {
                        Title = "Validation",
                        Description = "See how the validation plugin works.",
                        Action = async () => await _NavigationService.Navigate<ValidationDemoViewModel>()
                    }
                };
                MenuItems = menu;
            });
        }

        #endregion Lifecycle
    }
}