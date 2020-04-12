using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MathDuel
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GetActiveUsers : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }

        public GetActiveUsers()
        {
            Show();
        }
        public  void Show()
        {
            List<string> collect = new List<string>();
            InitializeComponent();
            string[] s1 = CurrentUser.currentUser.ActUsers.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            MyListView.ItemsSource = s1;
        }
        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }
    }
}
