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
    public partial class ChooseAnswers : ContentPage
    {
        int k = 0;
        public ChooseAnswers()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            ChooseComplexyty.Toggled += ChooseComplexyty_Toggled;
        }

        private void ChooseComplexyty_Toggled(object sender, ToggledEventArgs e)
        {
            k = (k + 1) % 2;
        }

        public void Algebra_click(object sender, EventArgs e)
        {
            CurrentUser.currentUser.SendMessage($"type{1 + k}");
            //ShowTasks();
            
        }

        public void Geom_click(object sender, EventArgs e)
        {
            CurrentUser.currentUser.SendMessage($"type{3 + k}");
            ShowTasks();
        }
        public void Comb_click(object sender, EventArgs e)
        {
            CurrentUser.currentUser.SendMessage($"type{5 + k}");
            ShowTasks();
        }

        public void ShowTasks()
        {
            while (CurrentUser.currentUser.tasks == null)
            {
                Thread.Sleep(50);
            }
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }
    }
}