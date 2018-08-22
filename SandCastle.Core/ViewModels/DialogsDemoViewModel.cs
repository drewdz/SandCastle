using MvvmCross.Commands;

using PE.Plugins.Dialogs;

using System;
using System.Threading.Tasks;

namespace SandCastle.Core.ViewModels
{
    public class DialogsDemoViewModel : BaseViewModel
    {
        #region Fields

        //  Create a readonly reference to a service you would like to inject
        private readonly IDialogService _DialogService;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Create a constructor with parameters for each service you would like to inject
        /// </summary>
        /// <param name="dialogService"></param>
        public DialogsDemoViewModel(IDialogService dialogService)
        {
            //  assign injected services to readonly fields. Not that service references are ALWAYS injected as interfaces. See CoreApp intialize method
            _DialogService = dialogService;

            Title = "Dialogs Demo";
        }

        #endregion Constructors

        #region Properties

        /// <summary>
        /// For bound properties use this pattern. Note the setter is important - this ensures the INotifyPropertyChange.PropertyChanged event is triggered
        /// </summary>
        private string _SomeText = string.Empty;
        public string SomeText
        {
            get => _SomeText;
            set => SetProperty(ref _SomeText, value);
        }

        #endregion Properties

        #region Commands

        /// <summary>
        /// Commands are typically bound to buttons. Sometimes it makes sense to execute simple actions directly within the command as an anonymous method. For more complex actions, defer to a concrete action.
        /// </summary>
        private IMvxCommand _SomeCommand = null;
        public IMvxCommand SomeCommand => _SomeCommand ?? new MvxAsyncCommand(() => SomeAction());

        #endregion Commands

        #region Actions

        private async Task SomeAction()
        {
            try
            {
                //  show a dialog
                if (string.IsNullOrEmpty(SomeText))
                {
                    await _DialogService.AlertAsync("Please enter some text. For examples of the validation plugin go to the Validation item in the menu.", Title, Constants.DLG_ACKNOWLEDGE, null);
                }
                else
                {
                    //  when referencing a bound property - whether for read or write - always reference the public version and not the private one
                    //  this is a very important habit to get into - eg: if you modify the private version (eg: _SomeText) it will not raise the PropertyChanged event
                    //  and you bound control will not update.
                    await _DialogService.AlertAsync(string.Format("The text you have entered is...\r\n\r\n{0}", SomeText), Title, Constants.DLG_ACKNOWLEDGE, null);
                }
            }
            catch (Exception ex)
            {
                //  Unless you have something specific to achieve, allow exception handling to bubble up to the top layer. 
                //  Remember try/catch is expensive from a performance point of view
                await _DialogService.AlertAsync("An error has prevented us from doing something. Try again or fix it first.", Title, Constants.DLG_ACKNOWLEDGE, null);
            }
        }

        #endregion Actions
    }
}
