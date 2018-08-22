using MvvmCross.Forms.Platforms.Uap.Views;

namespace SandCastle.UWP
{
    sealed partial class App
    {
        public App()
        {
            InitializeComponent();
        }
    }

    public abstract class UwpApp : MvxWindowsApplication<Setup, Core.MvxApp, Core.FormsApp>
    {
    }
}
