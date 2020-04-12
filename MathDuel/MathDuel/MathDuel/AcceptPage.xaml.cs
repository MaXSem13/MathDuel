using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MathDuel
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AcceptPage : ContentPage
	{
		public AcceptPage (string usName)
		{
			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
            InviteLabel.Text = $"user {usName} invite you";
            CurrentUser.currentUser.opponenet = usName;
		}

        public void Yes_Click(object sender, EventArgs e)
        {
            WaitLab.Text = "wait while your opponent chose type of tasks";
            CurrentUser.currentUser.SendMessage("acp1");
            if (WaitMethod())
            {
                App.Current.MainPage = new ShowErPage();
                return;
            }
            //App.Current.MainPage = new TasksPage(1, CurrentUser.currentUser.tasks[0]);
        }
        public void No_Click(object sender, EventArgs e)
        {
            CurrentUser.currentUser.opponenet = "";
            CurrentUser.currentUser.iswait = "";
            CurrentUser.currentUser.opflag = "";
            CurrentUser.currentUser.tasks = null;
            CurrentUser.currentUser.answers = null;
            CurrentUser.IsWaiing = false;
            CurrentUser.OponAnsw = null;
            CurrentUser.GetOponAnsw = false;
            CurrentUser.OponEx = false;
            CurrentUser.currentUser.SendMessage("acp0");
            Navigation.PushAsync(new MainMenu());
        }
        private bool WaitMethod()
        {
            while (CurrentUser.currentUser.tasks == null)
            {
                if (CurrentUser.OponEx)
                {
                    CurrentUser.OponEx = false;
                    return true;
                }
                Thread.Sleep(20);
            }
            return false;
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }

    }
}