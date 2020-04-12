using System;
using System.Threading;
using System.Net.Sockets;
using System.Text;
using Xamarin.Forms;

namespace MathDuel
{
    public class User
    {
        public string userName;
        private const string host = "172.20.10.10";
        private const int port = 8888;
        public string opponenet = "";
        public string iswait = "";
        public string opflag = "";
        public string ActUsers = "";
        public string stat = "";
        public string[] tasks = null;
        public string[] answers = null;
        public string[] useranswers = new string[5];
        public bool flag = false;
       public string s = "";
        static TcpClient client;
        static NetworkStream stream;
        public User()
        {
            client = new TcpClient();
            client.Connect(host, port); //подключение клиента
            stream = client.GetStream(); // получаем поток
            Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
            receiveThread.Start();//старт потока
            
        }
        ~User()
        {
            Disconnect();
        }


        // отправка сообщений
        public void SendMessage(string message)
        {
            try
            {
                if (message != "")
                {
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    stream.Write(data, 0, data.Length);
                }
            }
            catch
            {

            }



        }
        // получение сообщений
        public void ReceiveMessage()
        {
            while (true)
            {
                try
                {
                    byte[] data = new byte[64]; // буфер для получаемых данных
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0;
                    do
                    {
                        bytes = stream.Read(data, 0, data.Length);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (stream.DataAvailable);

                    string message = builder.ToString();
                    if (message == "-1")
                    {
                        flag = false;
                        s = "NO";
                    }
                    else if (message == "0")
                    {
                        flag = true;
                        s = "OK";

                    }
                    if (message == "-10")
                    {
                        s = "error";
                    }
                    else if(message[0] == 'A' && message.Length>=2 && message[1] == 'U')//принимает активных пользоваелей
                    {
                        s = "OKAU";
                        ActUsers = message.Substring(2);
                    }
                    else if(message[0] == 'S' && message.Length >= 2 && message[1] == 'T')//принимает статистику
                    {
                        s = "OKST";
                        stat = message.Substring(2);
                        
                    }
                    //работа с приглашением пользователей
                    else    if(message == "-2")
                    {
                        
                        CurrentUser.currentUser.opflag = message;
                        
                    }
                    else if (message == "-3")
                    {
                        CurrentUser.currentUser.opflag = message;

                    }
                    else if(message == "-5")
                    {
                        CurrentUser.currentUser.opflag = message;
                    }
                    else if (message == "-4")
                    {
                        CurrentUser.currentUser.opflag = message;
                    }
                    else if(message == "er")
                    {
                        App.Current.MainPage = new MainPage();
                    }
                    else if (message.Contains("inv"))
                    {
                        
                        if (CurrentUser.IsWaiing == true && CurrentUser.currentUser.iswait == "")
                        {
                            iswait = message;
                        }
                        else
                        {
                            CurrentUser.currentUser.SendMessage("acp3");
                        }



                    }
                    else if(message == "exit")
                    {
                        Disconnect();
                    }
                    else if (message.Contains("t/asks"))
                    {
                        TransformTasks(message);
                        App.Current.MainPage = new TasksPage(1, tasks[0]);
                        //показать окно с вопросами
                    }
                    else if (message.Contains("opex"))
                    {
                       opponenet = "";
                       iswait = "";
                       opflag = "";
                        tasks = null;
                       answers = null;
                        CurrentUser.OponEx = true;
                       App.Current.MainPage = new ShowErPage();

                    }
                    else if (message.Contains("UA"))
                    {
                         ParseAnsw(message.Substring(2));
                        CurrentUser.GetOponAnsw = true;
                    }

                }
                catch
                {
                    Disconnect();
                }
            }
        }

        public void Disconnect()
        {
            SendMessage("exit");
            if (stream != null)
                stream.Close();//отключение потока
            if (client != null) 
                client.Close();//отключение клиента
            System.Diagnostics.Process.GetCurrentProcess().CloseMainWindow(); //завершение процесса
        }

        public void SignIn(string message)
        {
            message = "1" + message;
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        public void SignUp(string message)
        {
            message = "2" + message;
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);

        }

        public void GetActiveusers()
        {
            string message = "3";
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        public void GetStatistic()
        {
            string message = "4";
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);

        }

        public void InviteUser(string s)
        {
            string message = "5"+s;
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);

        }
        public void AcceptionInvating(string s)
        {
            string message = "acp" + s;
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        public void TransformTasks(string message)
        {
            message = message.Substring(6);
            string[] sep = {"__"};
            string[] s = message.Split(sep,StringSplitOptions.RemoveEmptyEntries);
            sep[0] = "//";
            answers = s[1].Split(sep, StringSplitOptions.RemoveEmptyEntries);
            tasks = s[0].Split(sep, StringSplitOptions.RemoveEmptyEntries);

        }

        public string[] ParseAnsw(string answ)
        {
            string[] s = answ.Split(new string[] { "//" }, StringSplitOptions.RemoveEmptyEntries);
            CurrentUser.OponAnsw = s;
            return s;
        }




    }
}
