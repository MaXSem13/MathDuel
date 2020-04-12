using System;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace ChatServer
{
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        public string userName;
        public string acception = "0";
        public int numinGame = 0;
        public string[] answers = null;
        public string[] idTasks = null;
        TcpClient client;
        ServerObject server;// объект сервера

        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
        }

       public void Process()
        {
            try
            {
                bool flag = false;
                do
                {
                    Stream = client.GetStream();
                    // получаем имя пользователя
                    string message = GetMessage();
                    if (message.Length >= 1)
                    {
                        userName = message.Substring(1);
                    }
                    else
                    {
                        return;
                    }
                    if (message[0] == '2')
                    {
                        if (server.SignUp(userName, this.Id))
                        {
                            server.SignIn(userName, this.Id);
                            message = "0";
                            flag = true;
                            Console.WriteLine($"User {userName} enter in game");
                        }
                        else
                        {
                            message = "-1";
                        }
                    }
                    else if (message[0] == '1')
                    {
                        if (server.SignIn(userName, this.Id))
                        {
                            message = "0";
                            flag = true;
                            Console.WriteLine($"User {userName} enter in game");
                        }
                        else
                        {
                            message = "-1";
                        }
                    }
                    else
                    {
                        message = "not correct data";
                    }

                    // посылаем сообщение о входе 
                    server.CallBack(message, this);
                }
                while (!flag);
                server.AddConnection(this);

                // в бесконечном цикле получаем сообщения от клиента
                while (true)
                {
                    int k = 0;
                    try
                    {
                        string message = GetMessage();
                        string msg = String.Format("{0}: {1}", userName, message);
                        if (message != "")
                        {
                            Console.WriteLine(msg);
                        }

                        if(message == "exit")
                        {
                            Close();
                        }
                       if(message == "" && k<1)
                        {
                            message = String.Format("{0}: leave game 1", userName);
                            Console.WriteLine(message);
                            k++;
                            k = 0;
                            break;
                        }
                        
                            
                        

                        //parsing messages
                        //работа с получением списка активных пользователей
                        if (message == "3")
                        {
                            string s = server.GetActiveUsers();
                            server.CallBack(s, this);
                        }
                        //получаем статистику пользователя
                        else if(message == "4")
                        {
                            string[] str = userName.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            string s = server.GetStatistic(str[0]);
                            server.CallBack(s, this);
                        }
                        //приглашаем опонентав игру
                       else if(message.Length>1 && message[0] == '5')
                        {
                            //отправляем пользователю приглашение
                            string uName = message.Substring(1);
                            bool ans = server.PossibleInvite(uName);
                            if (ans)
                            {
                                ClientObject client = server.clients.FirstOrDefault(c => c.GetName() == uName);
                                if (client==null)
                                {
                                    server.CallBack("-2", this);
                                }
                                else
                                {
                                    if (client != null)
                                    {
                                        InviteInGame(client);

                                    }
                                    else
                                        server.CallBack("-2", this);
                                }
                            }
                            else
                            {
                                server.CallBack("-2", this);
                            }
                        }
                        else if (message.Contains("type"))
                        {
                            //получаем список задач заданного типа
                            string s = message[4].ToString();
                            Game game = server.games.FirstOrDefault(c => c.player1 == this || c.player2 == this);
                            if(game!= null)
                            {
                                server.GetTasks(s, game);
                                game.SendTasks();

                            }
                            else
                            {
                                server.CallBack("er", this);
                            }
                            
                            
                        }
                        //принятие приглашения
                        else if (message.Contains("acp"))
                        {
                            AcceptInvating(message);
                        }
                        //отправляем ответы противникy
                        else if (message.Length>2 && message[0] == 'U' && message[1] == 'A')
                        {
                            Game game = server.games.FirstOrDefault(c => c.player1 == this || c.player2 == this);
                            if (game != null)
                            {
                                ClientObject player = game.player1 == this ? game.player2 : game.player1;
                               
                                server.CallBack(message, player);
                            }
                        }
                        //добавляем ответы в статистику
                        else if (message.Length > 2 && message[0] == 'M' && message[1] == 'A')
                        {
                            string[] UsAns = ParseAnsw(message);
                            server.AddStatistic(this.GetName(),idTasks, UsAns);
                        }
                        //удаляем поля InGame и Game
                        else if(message == "remGame")
                        {
                            server.RemoveInGame(GetName());
                            try
                            {
                                Game g = server.games.FirstOrDefault(c => c.player1 == this || c.player2 == this);
                                if (g != null)
                                {
                                    server.games.Remove(g);
                                    server.DeleteGame(g.idGame);
                                    g = null;
                                }
                            }
                            catch(Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            
                        }
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        string message = String.Format("{0}: leave game 2", userName);
                        Console.WriteLine(message);
                        server.CallBack("exit", this);
                        Close();
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        // чтение входящего сообщения и преобразование в строку
        private string GetMessage()
        {
            byte[] data = new byte[64]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        // закрытие подключения
        protected internal void Close()
        {
            if (userName != string.Empty && userName != null)
            {
                server.RemoveInGame(GetName());
                server.RemoveActive(userName);
            }
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
        protected internal void InviteInGame(ClientObject client)
        {
            string message = "inv" + " " + GetName();
            server.CallBack(message, client);
            while (client.acception == "0")
            {
                Thread.Sleep(20);
            }
            if (client.acception == "1")
            {
                //создаем комнату для игроков
                client.acception = "0";
                server.games.Add(new Game(this, client, server, (server.games.Count+1).ToString()));
                server.CallBack("-4", this);
                

            }
            if (client.acception == "-1")
            {
                //отправляем пригласившему отказ
                client.acception = "0";
                server.CallBack("-3", this);
            }
            //пользователь не ожидает игры
            if(client.acception == "2")
            {
                client.acception = "0";
                server.CallBack("-5", this);
            }

            
        }
        protected internal void AcceptInvating(string message)
        {
            if (message[3] == '1')
            {
                acception = "1";
            }
            else if(message[3]=='3')
            {
                acception = "2";
            }
            else
            {
                acception = "-1";
            }
        }

        protected internal string GetName()
        {
            string[] s = userName.Split(new char[] { ' ' });
            return s[0];
        }

        protected internal string[] ParseAnsw(string s)
        {
            s = s.Substring(2);
            string[] ans = s.Split(new string[] {"//"}, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < ans.Length; i++)
            {
                double d;
                if(!double.TryParse(ans[i], out d))
                {
                    ans[i] = "-1";
                }
                else
                {
                    ans[i] = d.ToString();
                }
            }
            string[] ans1 = new string[5];
            for (int i = 0; i < 5; i++)
            {
                ans1[i] = ans[i+1];
            }
            return ans1;
        }



    }
}