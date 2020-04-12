using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MathDuel
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ShowResaltPage : ContentPage
    {
        public ObservableCollection<string> Items { get; set; }
        int yourCount = 0;
        int countoponnent = 0;
        public ShowResaltPage(string[] opansw)
        {
            InitializeComponent();
            Title = "Result";
            Items = new ObservableCollection<string>
            {
                "Task №",
                "1",
                "2",
                "3",
                "4",
                "5",
            };
            string[] answ = new string[6];
            answ[0] = "Right Answers";
            for (int i = 1; i < 6; i++)
            {
                answ[i] = CurrentUser.currentUser.answers[i - 1];
            }
            MyListView2.ItemsSource =  GetArrAnsw(CurrentUser.currentUser.useranswers, CurrentUser.currentUser.userName);
            MyListView4.ItemsSource = answ;
            MyListView1.ItemsSource = Items;
            MyListView3.ItemsSource = GetArrAnsw(opansw, CurrentUser.currentUser.opponenet);
            GetWinner();

        }



        public void OK_Click(object sender, EventArgs e)
        {
            CurrentUser.currentUser.opponenet = "";
            CurrentUser.currentUser.iswait = "";
            CurrentUser.currentUser.opflag = "";
            CurrentUser.IsWaiing = false;
            CurrentUser.OponAnsw = null;
            CurrentUser.GetOponAnsw = false;
            CurrentUser.OponEx = false;
            CurrentUser.currentUser.tasks = null;
            CurrentUser.currentUser.answers = null;
            CurrentUser.currentUser.SendMessage("remGame");
            App.Current.MainPage = new MainMenu();
            //отправить результаты на сервер

        }


        private string[] GetArrAnsw(string[] answers, string name)
        {
            string[] coransw = new string[answers.Length + 1];
            coransw[0] = name;
            for (int i = 0; i < answers.Length; i++)
            {
                double d;
                if(!double.TryParse(answers[i], out d))
                {

                    coransw[i + 1] = "incorrect";
                }
                else
                {
                    coransw[i + 1] = d.ToString();
                    if(double.Parse(CurrentUser.currentUser.answers[i]) == d)
                    {
                        if(name == CurrentUser.currentUser.userName)
                        {
                            yourCount += 1;
                        }
                        else
                        {
                            countoponnent += 1;
                        }
                    }
                }
            }
            //отправить ответы и добавить в статистику
            if (name == CurrentUser.currentUser.userName)
            {
                string s = "MA";
                for (int i = 0; i < coransw.Length-1; i++)
                {
                    s += coransw[i].ToString() + "//";
                }
                s += coransw[coransw.Length - 1].ToString();
                CurrentUser.currentUser.SendMessage(s);
            }

                return coransw;
        }

        private void GetWinner()
        {
            if(yourCount == countoponnent)
            {
                WinLabel.Text = "DRAW";
            }
            else if(yourCount > countoponnent)
            {
                WinLabel.Text = "YOU WIN!";
            }
            else
            {
                WinLabel.Text = "YOUR OPPONENT WIN";
            }
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }
    }
}
