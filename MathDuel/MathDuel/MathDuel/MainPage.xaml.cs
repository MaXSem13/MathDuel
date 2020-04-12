using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Xamarin.Forms;

namespace MathDuel
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
        }

        public void SignIn_Click(object sender, EventArgs e)
        {
            SignUpInfo.Text = "";
            SignInInfo.Text = "";
            User testUser = null;
            string nickname = InputNickname.Text == null ? "" : InputNickname.Text;
            string password = InputPassword.Text == null ? "" : InputPassword.Text; ;
            if (IsCorrectNick(nickname) != "OK")
            {
                SignInInfo.Text = IsCorrectNick(nickname);
                return;
            }
            if (IsCorrectPassword(password) != "OK")
            {
                SignInInfo.Text = IsCorrectPassword(password);
                return;
            }
            try
            {
                testUser = new User();
            }
            catch (Exception ex)
            {
                string st = ex.Message;
                st += "";
            }
            testUser.SignIn(InputNickname.Text + " " + InputPassword.Text);
            while (testUser.s == "")
            {
                Thread.Sleep(20);
            }
            if (testUser.flag)
            {
                CurrentUser.currentUser = testUser;
                CurrentUser.currentUser.userName = nickname;
                SignInInfo.Text = "you enter in game";
                Navigation.PushAsync(new MainMenu());
            }
            else
            {
                SignInInfo.Text = "not correct data";
            }

        }

        public void SignUp_Click(object sender, EventArgs e)
        {
            SignUpInfo.Text = "";
            SignInInfo.Text = "";
            User testUser = null;
            string nickname = InputNickname.Text == null?"": InputNickname.Text;
            string password = InputPassword.Text == null ? "" : InputPassword.Text; ;
            if (IsCorrectNick(nickname) != "OK")
            {
                SignUpInfo.Text = IsCorrectNick(nickname);
                return;
            }
            if ( IsCorrectPassword(password) != "OK")
            {
                SignUpInfo.Text = IsCorrectPassword(password);
                return;
            }
            try
            {
                testUser = new User();
            }
            catch (Exception ex)
            {
                string st = ex.Message;
                st += "";
            }
            testUser.SignUp(InputNickname.Text + " " + InputPassword.Text);
            while (testUser.s == "")
            {
                Thread.Sleep(20);
            }
            if (testUser.flag)
            {
                CurrentUser.currentUser = testUser;
                CurrentUser.currentUser.userName = nickname;
                SignUpInfo.Text = "you SignUp and enter in game";
                Navigation.PushAsync(new MainMenu());
            }
            else
            {
                SignUpInfo.Text = "that user already exist";
            }

        }

        public string IsCorrectNick(string message)
        {
            if (message.Length > 8)
            {
                return "nickname must contein less than 8 symbols";
            }
            if (message.Contains(" ")|| message.Contains("/"))
            {
                return "nicknew can not contain space sympols and / symbol";
            }
            if (message == "")
            {
                return "Nickname can not be empty";
            }
            else
            {
                return "OK";
            }
        }

        public string IsCorrectPassword(string message)
        {
            if (message.Length > 12)
            {
                return "password must contein less than 12 symbols";
            }
            if (message.Contains(" "))
            {
                return "password can not contain space symbols";
            }
            if (message == "")
            {
                return "password can not be empty";
            }
            else
            {
                return "OK";
            }
        }
    }
}
