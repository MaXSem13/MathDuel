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
    public partial class PlayGamePage : ContentPage
    {
        public PlayGamePage()
        {
            InitializeComponent();
        }
        public void WaitBut_Click(object sender, EventArgs e)
        {
            CurrentUser.IsWaiing = true;
            int k = 0;
            while(CurrentUser.currentUser.iswait == "" && k!=1000)
            {
                Thread.Sleep(20);
                k++;
            }
            if (k == 1000 && CurrentUser.currentUser.iswait=="")
            {
                //выводим информациюна экран
                CurrentUser.IsWaiing = false;
                PlayGameInfo.Text = "you was not invaited";
            }
            else
            {
                string[] s = CurrentUser.currentUser.iswait.Split(new char[] { ' ' });
                Navigation.PushAsync(new AcceptPage(s[1]));
            }
        }
        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }
    }
}