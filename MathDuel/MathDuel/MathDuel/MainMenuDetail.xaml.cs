using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MathDuel
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMenuDetail : ContentPage
    {
        public MainMenuDetail()
        {
            InitializeComponent();
        }

        public  void Invite_click(object sender, EventArgs e)
        {
            string opname = InputOponnentNick.Text == null ? "" : InputOponnentNick.Text;
            if(opname == CurrentUser.currentUser.userName)
            {
                IviteLabel.Text = "you can not invite yourself";
                return;
            }
            else if(opname == "")
            {
                IviteLabel.Text = "nickname can nor be empty";
                return;
            }
            else if(opname.Contains(" ") || opname.Contains("/"))
            {
                IviteLabel.Text = "nickname can nor contain space and / symbols";
                return;
            }
            else
            {
                CurrentUser.currentUser.InviteUser(opname);
                CurrentUser.currentUser.opponenet = opname;
                CurrentUser.currentUser.opflag = "";
                while (CurrentUser.currentUser.opflag == "")
                {
                    Thread.Sleep(50);
                }
                if(CurrentUser.currentUser.opflag == "-2")
                {
                    IviteLabel.Text = "impossible invite this user in game";
                }
                else if(CurrentUser.currentUser.opflag == "-3")
                {
                    IviteLabel.Text = "opponent decline your inviting";
                }
                else if (CurrentUser.currentUser.opflag == "-5")
                {
                    IviteLabel.Text = "opponent do not wait game";
                }
                else
                {
                    Navigation.PushAsync(new ChooseAnswers());
                }


            }
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }
    }
}