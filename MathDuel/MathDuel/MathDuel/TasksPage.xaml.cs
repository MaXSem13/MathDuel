using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Timers;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MathDuel
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TasksPage : ContentPage
    {
        public TasksPage(int number, string task)
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
            NumberTask.Text = number.ToString();
            TaskLabel.Text = task;

        }
        public void But_Click(object sender, EventArgs e)
        {
            Button but = (Button)sender;
            if(but.Text == "del")
            {
                AnswerLab.Text = DelLastSymbol(AnswerLab.Text);
                
                
            }
            else if(but.Text == "Enter")
            {
                if (CurrentUser.OponEx)
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
                    App.Current.MainPage = new ShowErPage();
                }
                if (int.Parse(NumberTask.Text) == 5)
                {
                    if (AnswerLab.Text == "")
                    {
                        CurrentUser.currentUser.useranswers[int.Parse(NumberTask.Text) - 1] = "-";
                        TaskLabel.Text = CurrentUser.currentUser.tasks[int.Parse(NumberTask.Text) - 1];
                    }
                    else
                    {
                        CurrentUser.currentUser.useranswers[int.Parse(NumberTask.Text) - 1] = AnswerLab.Text;
                        CurrentUser.currentUser.SendMessage(AnswToStr());
                    }
                    //сравниваем результаты и выбрасыывем другую старницу
                    while (!CurrentUser.GetOponAnsw)
                    {
                        Thread.Sleep(20);
                        if (CurrentUser.OponEx)
                        {
                            CurrentUser.OponEx = false;
                            App.Current.MainPage = new ShowErPage();
                            return;
                        }
                    }
                   App.Current.MainPage =  new ShowResaltPage(CurrentUser.OponAnsw);
                    return;
                }
                else
                {
                    if(AnswerLab.Text == "")
                    {
                        CurrentUser.currentUser.useranswers[int.Parse(NumberTask.Text) - 1] = "-";
                        NumberTask.Text = (int.Parse(NumberTask.Text) + 1).ToString();
                        TaskLabel.Text = CurrentUser.currentUser.tasks[int.Parse(NumberTask.Text) - 1];
                    }
                    else
                    {
                        CurrentUser.currentUser.useranswers[int.Parse(NumberTask.Text) - 1] = AnswerLab.Text;
                        NumberTask.Text = (int.Parse(NumberTask.Text) + 1).ToString();
                        TaskLabel.Text = CurrentUser.currentUser.tasks[int.Parse(NumberTask.Text) - 1];

                    }
                    AnswerLab.Text = "";

                }


            }
            else if (AnswerLab.Text.Length<6)
            {
                AnswerLab.Text += but.Text;
            }
            
            
        }

        private string DelLastSymbol(string s)
        {
            string str = "";
            for (int i = 0; i < s.Length-1; i++)
            {
                str += s[i];
            }
            return str;
        }
        private string AnswToStr()
        {
            string s = "UA";
            for (int i = 0; i < CurrentUser.currentUser.useranswers.Length-1; i++)
            {
                s += CurrentUser.currentUser.useranswers[i] + "//";
            }
            s += CurrentUser.currentUser.useranswers[CurrentUser.currentUser.useranswers.Length - 1];
            return s;
        }

        protected override bool OnBackButtonPressed()
        {
            base.OnBackButtonPressed();
            return true;
        }
    }
}