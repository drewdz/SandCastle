using MvvmCross.Commands;
using PE.Plugins.Dialogs;
using PE.Plugins.Validation;
using PE.Plugins.Validation.Validators;
using System.Threading.Tasks;

namespace SandCastle.Core.ViewModels
{
    public class ValidationDemoViewModel : BaseViewModel
    {
        #region Fields

        private readonly IValidationService _ValidationService;
        private readonly IDialogService _DialogService;

        #endregion Fields

        #region Constructors

        public ValidationDemoViewModel(IValidationService validationService, IDialogService dialogService)
        {
            _ValidationService = validationService;
            _DialogService = dialogService;

            Title = "Validation Demo";
        }

        #endregion Constructors

        #region Properties

        #region Firstname

        private string _Firstname = string.Empty;
        [RequiredValidator(Message = "Firstname is required")]
        public string Firstname
        {
            get => _Firstname;
            set
            {
                SetProperty(ref _Firstname, value);
                _ValidationService.Validate(this, () => Firstname);
            }
        }

        private string _FirstnameInvalid = string.Empty;
        public string FirstnameInvalid
        {
            get => _FirstnameInvalid;
            set
            {
                FirstnameIsValid = string.IsNullOrEmpty(value);
                SetProperty(ref _FirstnameInvalid, value);
            }
        }

        private bool _FirstnameIsValid = true;
        public bool FirstnameIsValid
        {
            get => _FirstnameIsValid;
            set => SetProperty(ref _FirstnameIsValid, value);
        }

        #endregion Firstname

        #region Lastname

        private string _Lastname = string.Empty;
        [RequiredValidator(Message = "Lastname is required")]
        public string Lastname
        {
            get => _Lastname;
            set
            {
                SetProperty(ref _Lastname, value);
                _ValidationService.Validate(this, () => Lastname);
            }
        }

        private string _LastnameInvalid = string.Empty;
        public string LastnameInvalid
        {
            get => _LastnameInvalid;
            set
            {
                SetProperty(ref _LastnameInvalid, value);
                LastnameIsValid = string.IsNullOrEmpty(value);
            }
        }

        private bool _LastnameIsValid = true;
        public bool LastnameIsValid
        {
            get => _LastnameIsValid;
            set => SetProperty(ref _LastnameIsValid, value);
        }

        #endregion Lastname

        #region Username

        private string _Username = string.Empty;
        [RequiredValidator(Message = "Username is required")]
        public string Username
        {
            get => _Username;
            set
            {
                SetProperty(ref _Username, value);
                _ValidationService.Validate(this, () => Username);
            }
        }

        private string _UsernameInvalid = string.Empty;
        public string UsernameInvalid
        {
            get => _UsernameInvalid;
            set
            {
                SetProperty(ref _UsernameInvalid, value);
                UsernameIsValid = string.IsNullOrEmpty(value);
            }
        }

        private bool _UsernameIsValid = true;
        public bool UsernameIsValid
        {
            get => _UsernameIsValid;
            set => SetProperty(ref _UsernameIsValid, value);
        }

        #endregion Username

        #endregion Properties

        #region Commands

        private IMvxCommand _RegisterCommand;
        public IMvxCommand RegisterCommand
        {
            get
            {
                if (_RegisterCommand == null) _RegisterCommand = new MvxAsyncCommand(() => Register());
                return _RegisterCommand;
            }
        }

        #endregion Commands

        #region Actions

        private async Task Register()
        {
            //  make sure everything is valid
            if (!_ValidationService.Validate(this)) return;
            await _DialogService.AlertAsync("All fields are valid.", Title, Constants.DLG_ACKNOWLEDGE, null);
        }

        #endregion Actions
    }
}
