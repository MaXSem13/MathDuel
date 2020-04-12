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
    public partial class MainMenu : MasterDetailPage
    {
        public MainMenu()
        { 
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            MasterPage.ListView.ItemSelected += ListView_ItemSelected;
        }

        private void  ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MainMenuMenuItem;
            if (item == null)
                return;
            //invite player
            if (item.Id == 0)
            {
                var page = (Page)Activator.CreateInstance(item.TargetType);
                page.Title = item.Title;
                NavigationPage np = new NavigationPage(page);
                np.BarBackgroundColor = Color.DarkBlue;
                np.BarTextColor = Color.White;
                Detail = np;
                IsPresented = false;
            }
            //get statistic
            else if(item.Id == 1)
            {
                CurrentUser.currentUser.stat = "";
                CurrentUser.currentUser.GetStatistic();
                while (CurrentUser.currentUser.stat == "")
                {
                    Thread.Sleep(50);
                }
                GetStatistic page = new GetStatistic();
                page.Show();
                page.Title = item.Title;
                NavigationPage np = new NavigationPage(page);
                np.BarBackgroundColor = Color.DarkBlue;
                np.BarTextColor = Color.White;
                Detail = np;
                IsPresented = false;
            }
            //get active users
            else if (item.Id == 2)
            {
                CurrentUser.currentUser.ActUsers = "";
                CurrentUser.currentUser.GetActiveusers();
                while (CurrentUser.currentUser.ActUsers == "")
                {
                    Thread.Sleep(50);
                }
                Page p = new GetActiveUsers();
                p.Title = item.Title;
                NavigationPage np = new NavigationPage(p);
                np.BarBackgroundColor = Color.DarkBlue;
                np.BarTextColor = Color.White;
                Detail = np;
                IsPresented = false;
            }
            //exit
            else if (item.Id == 3)
            {
                var page = (Page)Activator.CreateInstance(item.TargetType);
                page.Title = item.Title;
                NavigationPage np = new NavigationPage(page);
                np.BarBackgroundColor = Color.DarkBlue;
                np.BarTextColor = Color.White;
                Detail = np;
                IsPresented = false;
            }
            else if (item.Id == 4)
            {
                Page page = new PlayGamePage();
                page.Title = item.Title;
                NavigationPage np = new NavigationPage(page);
                np.BarBackgroundColor = Color.DarkBlue;
                np.BarTextColor = Color.White;
                Detail = np;
                IsPresented = false;
            }



            MasterPage.ListView.SelectedItem = null;
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }


    }
}