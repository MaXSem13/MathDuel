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
	public partial class GetStatistic : ContentPage
	{
		public GetStatistic ()
		{
			InitializeComponent ();
        }
        public void Show()
        {
            string[] s = CurrentUser.currentUser.stat.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            Answers.Text = s[0];
            CorrectAns.Text = s[1];
        }
        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }
    }
}