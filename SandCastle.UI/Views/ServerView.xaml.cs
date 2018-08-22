using MvvmCross.Forms.Presenters.Attributes;
using MvvmCross.Forms.Views;
using SandCastle.Core.ViewModels;

namespace SandCastle.UI.Views
{
    public partial class ServerView : MvxContentPage<ServerViewModel>
	{
		public ServerView()
		{
			InitializeComponent ();
		}
	}
}