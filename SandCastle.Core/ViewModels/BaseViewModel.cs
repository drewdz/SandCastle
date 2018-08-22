using MvvmCross.ViewModels;

namespace SandCastle.Core.ViewModels
{
    public abstract class BaseViewModel : MvxViewModel
    {
        #region Properties

        private string _Title = string.Empty;
        public string Title
        {
            get => _Title;
            set => SetProperty(ref _Title, value);
        }

        #endregion Properties
    }
}
