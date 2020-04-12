using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MathDuel
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class ShowErPage : ContentPage
	{
		public ShowErPage ()
		{

			InitializeComponent ();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        public void Ok_click(object sender, EventArgs e)
        {
            App.Current.MainPage = new MainMenu();
        }
        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }
    }

}