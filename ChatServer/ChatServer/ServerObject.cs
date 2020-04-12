using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading;

namespace ChatServer
{
    public class ServerObject
    {
        static TcpListener tcpListener; // сервер для прослушивания
        public List<ClientObject> clients = new List<ClientObject>(); // все подключения
        const string ip = "172.20.10.10";
        const string connectionString = @"Data Source=LAPTOP-OORPDL13\SQLEXPRESS;Initial Catalog=MathQuiz;Integrated Security=True";
        static Random rnd = new Random();
        public List<Game> games = new List<Game>();

        protected internal void AddConnection(ClientObject clientObject)
        {
            clients.Add(clientObject);
        }
        protected internal void RemoveConnection(string id)
        {
            // получаем по id закрытое подключение
            ClientObject client = clients.FirstOrDefault(c => c.Id == id);
            if (client != null)
            {
                Game g = games.FirstOrDefault(c => c.player1 == client || c.player2 == client);
                if(g != null)
                {
                    ClientObject opon = g.player1 == client ? g.player2 : g.player1;
                    if(opon!= null)
                    {
                        CallBack("opex", opon);
                        RemoveInGame(opon.GetName());
                        games.Remove(g);
                        DeleteGame(g.idGame);
                        g = null;
                    }
                }
            }
            // и удаляем его из списка подключений
            if (client != null)
                clients.Remove(client);
        }
        // прослушивание входящих подключений
        protected internal void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Parse(ip), 8888);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    ClientObject clientObject = new ClientObject(tcpClient, this);
                    Thread clientThread = new Thread(new ThreadStart(clientObject.Process));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }


        // отключение всех клиентов
        protected internal void Disconnect()
        {
            tcpListener.Stop(); //остановка сервера

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].Close(); //отключение клиента
            }
            Environment.Exit(0); //завершение процесса
        }
        protected internal bool SignIn(string msg, string id)
        {
            string[] s = msg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand("SELECT * FROM [Users] WHERE [Nickname] = @Nickname", sqlConnection);
            command.Parameters.AddWithValue("Nickname", s[0]);

            try
            {
                sqlReader = command.ExecuteReader();
                if (sqlReader.Read()&& sqlReader["Password"].ToString() == s[1]&& sqlReader["IsActive"].ToString() == "False")
                {
                    sqlReader.Close();
                    SqlCommand update = new SqlCommand("UPDATE [Users] SET [IsActive] = @IsActive WHERE [Nickname] = @Nickname", sqlConnection);
                    update.Parameters.AddWithValue("Nickname", s[0]);
                    update.Parameters.AddWithValue("IsActive", true);
                    update.ExecuteNonQuery();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                if (sqlReader != null)
                {
                    sqlReader.Close();
                }
                if (sqlConnection != null)
                {
                    sqlConnection.Close();
                }
            }



        }
        //
        protected internal bool SignUp(string msg, string id)
        {
            string[] s = msg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand("SELECT * FROM [Users] WHERE [Nickname] = @Nickname", sqlConnection);
            command.Parameters.AddWithValue("Nickname", s[0]);
            sqlReader = command.ExecuteReader();
            int count = 1;

            try
            {
                if (!sqlReader.Read())
                {
                    if (sqlReader != null)
                    {
                        sqlReader.Close();
                    }

                    command = new SqlCommand("SELECT * FROM [Users]", sqlConnection);
                    sqlReader = command.ExecuteReader();
                    while (sqlReader.Read())
                    {
                        count += 1;
                    }

                    if (sqlReader != null)
                    {
                        sqlReader.Close();
                    }

                    SqlCommand insertUser = new SqlCommand("INSERT [Users] (Id_User, Nickname, Password, InGame, IsActive)VALUES(@Id_User,@Nickname, @Password, @InGame, @IsActive)", sqlConnection);
                    insertUser.Parameters.AddWithValue("Nickname", s[0]);
                    insertUser.Parameters.AddWithValue("Password", s[1]);
                    insertUser.Parameters.AddWithValue("InGame", false);
                    insertUser.Parameters.AddWithValue("IsActive", true);
                    insertUser.Parameters.AddWithValue("Id_User", count);
                    insertUser.ExecuteNonQuery();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                if (sqlReader != null)
                {
                    sqlReader.Close();
                }
            }

        }
        //отправляет клиенту ответ от сервера
        protected internal void CallBack(string message, ClientObject client)
        {
            try
            {
                byte[] data = Encoding.Unicode.GetBytes(message);
                if (client != null)
                {
                    client.Stream.Write(data, 0, data.Length);
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
        //снимает активность пользователя в базе данных
        protected internal void RemoveActive(string msg)
        {
            string[] s = msg.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            try
            {
                SqlCommand update = new SqlCommand("UPDATE [Users] SET [IsActive] = @IsActive WHERE [Nickname] = @Nickname", sqlConnection);
                update.Parameters.AddWithValue("Nickname", s[0]);
                update.Parameters.AddWithValue("IsActive", false);
                update.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected internal string GetActiveUsers()
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlDataReader sqlReader = null;
            SqlDataReader sqlReaderOpon = null;
            SqlCommand command = new SqlCommand("SELECT * FROM [Users] WHERE [IsActive] = @IsActive", sqlConnection);
            command.Parameters.AddWithValue("IsActive", true);
            sqlReader = command.ExecuteReader();
            string s = "AU";
            try
            {
                while (sqlReader.Read())
                {
                   s += sqlReader["Nickname"].ToString() + " ";
                }
                return s;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return "-10";
            }
            finally
            {
                if (sqlReader != null)
                {
                    sqlReader.Close();
                }
                if (sqlReaderOpon != null)
                {
                    sqlReaderOpon.Close();
                }
                if (sqlConnection != null)
                {
                    
                    sqlConnection.Close();
                }
            }
        }
        protected internal string GetStatistic(string nickname)
        {
            int counttasks = 0;
            int countcortasks = 0;
            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                SqlDataReader sqlReader = null;
                SqlCommand command = new SqlCommand("SELECT s. * FROM [Statistic] as s  JOIN [Tasks] AS t ON s.Id_Task = t.Id_Task WHERE s.Id_User = @Id_User", sqlConnection);
                command.Parameters.AddWithValue("@Id_User", GetIdUser(nickname));
                using (sqlReader = command.ExecuteReader())
                {
                    while (sqlReader.Read())
                    {
                        counttasks++;
                    }
                }
            }
            if(counttasks == 0)
            {
                return $"ST{counttasks} {countcortasks}";
            }

            using (SqlConnection sqlConnection = new SqlConnection(connectionString))
            {
                sqlConnection.Open();
                SqlDataReader sqlReader = null;
                SqlCommand command = new SqlCommand("SELECT s. * FROM [Statistic] as s  JOIN[Tasks] AS t ON s.User_Answer = t.Answer AND s.Id_Task = t.Id_Task WHERE s.Id_User = @Id_User", sqlConnection);
                command.Parameters.AddWithValue("@Id_User", GetIdUser(nickname));
                using (sqlReader = command.ExecuteReader())
                {
                    while (sqlReader.Read())
                    {
                        countcortasks++;
                    }
                }
            }

            return $"ST{counttasks} {countcortasks}";
        }

        protected internal bool PossibleInvite(string usName)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand("SELECT * FROM [Users] WHERE [Nickname] = @Nickname", sqlConnection);
            command.Parameters.AddWithValue("Nickname", usName);
            sqlReader = command.ExecuteReader();
            try
            {
                while (sqlReader.Read())
                {
                    if (sqlReader["IsActive"].ToString() == "True" && sqlReader["InGame"].ToString() == "False")
                    {
                        return true;
                    }
                    else
                        return false;
                }
                return false;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            finally
            {
                if (sqlReader != null)
                {
                    sqlReader.Close();
                }
                if (sqlConnection != null)
                {
                    sqlConnection.Close();
                }
            }


        }

        protected internal void GetTasks(string s, Game game)
        {
            List<string> tasks = new List<string>();
            List<string> answers = new List<string>();
            List<string> IdTasks = new List<string>();
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand("SELECT * FROM [Tasks] WHERE [Id_Type] = @Id_Type", sqlConnection);
            command.Parameters.AddWithValue("Id_Type", s);
            try
            {
                sqlReader = command.ExecuteReader();
                while (sqlReader.Read())
                {
                    tasks.Add(sqlReader["Task"].ToString());
                    answers.Add(sqlReader["Answer"].ToString());
                    IdTasks.Add(sqlReader["Id_Task"].ToString());
                }
                int[] numTasks = new int[5];
                numTasks = GetNumbers("", tasks.Count);
                string[] tasks1 = new string[5];
                string[] answ1 = new string[5];
                string[] IdTask1 = new string[5];
                for (int i = 0; i < 5; i++)
                {
                    int num = numTasks[i];
                    tasks1[i] = tasks[num];
                    answ1[i] = answers[num];
                    IdTask1[i] = IdTasks[num];
                }
                game.tasks = tasks1;
                game.answ = answ1;
                game.idTasks = IdTask1;
                game.player1.idTasks = IdTask1;
                game.player2.idTasks = IdTask1;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (sqlReader != null)
                {
                    sqlReader.Close();
                }
                if (sqlConnection != null)
                {
                    sqlConnection.Close();
                }
            }


        }

        public void SetInGame(string nikcname)
        {
            List<string> tasks = new List<string>();
            List<string> answers = new List<string>();
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            try
            {
                SqlCommand update = new SqlCommand("UPDATE [Users] SET [InGame] = @InGame WHERE [Nickname] = @Nickname", sqlConnection);
                update.Parameters.AddWithValue("Nickname", nikcname);
                update.Parameters.AddWithValue("InGame", true);
                update.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (sqlConnection != null)
                {
                    sqlConnection.Close();
                }
            }
        }

        public void RemoveInGame(string nikcname)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            List<string> tasks = new List<string>();
            List<string> answers = new List<string>();
            SqlCommand update = new SqlCommand("UPDATE [Users] SET [InGame] = @InGame WHERE [Nickname] = @Nickname", sqlConnection);
            update.Parameters.AddWithValue("Nickname", nikcname);
            update.Parameters.AddWithValue("InGame", false);
            update.ExecuteNonQuery();
            if (sqlConnection != null)
            {
                sqlConnection.Close();
            }
        }

        public void AddStatistic(string nick, string[] idT, string[] answ)
        {
            string id = GetIdUser(nick);
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            for (int i = 0; i < answ.Length; i++)
            {
                try
                {
                    SqlCommand insertUser = new SqlCommand("INSERT [Statistic] (Id_User, Id_Task, User_Answer)VALUES(@Id_User,@Id_Task, @User_Answer)", sqlConnection);
                    insertUser.Parameters.AddWithValue("Id_User", id);
                    insertUser.Parameters.AddWithValue("Id_Task", idT[i]);
                    insertUser.Parameters.AddWithValue("User_Answer", answ[i]);
                    insertUser.ExecuteNonQuery();
                }
                catch
                {
                    SqlCommand update = new SqlCommand("UPDATE [Statistic] SET [User_Answer] = @User_Answer WHERE [Id_User] = @Id_User AND [Id_Task] = @Id_Task", sqlConnection);
                    update.Parameters.AddWithValue("Id_User", id);
                    update.Parameters.AddWithValue("Id_Task", idT[i]);
                    update.Parameters.AddWithValue("User_Answer", answ[i]);
                    update.ExecuteNonQuery();
                }

            }
            if (sqlConnection != null)
            {
                sqlConnection.Close();
            }

        }

        public string GetIdUser(string nickname)
        {
            string id = "";
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlDataReader sqlReader = null;
            SqlCommand command = new SqlCommand("SELECT * FROM [Users] WHERE [Nickname] = @Nickname", sqlConnection);
            command.Parameters.AddWithValue("Nickname", nickname);
            sqlReader = command.ExecuteReader();
            while (sqlReader.Read())
            {
                id = sqlReader["Id_User"].ToString();
            }
            if (sqlReader != null)
            {
                sqlReader.Close();
            }
            if (sqlConnection != null)
            {
                sqlConnection.Close();
            }
            return id;

        }

        protected internal void AddGame(string nickname1, string nickname2, string idGame)
        {
            string id1 = GetIdUser(nickname1);
            string id2 = GetIdUser(nickname2);
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            SqlCommand insertUser = new SqlCommand("INSERT [Games] (Id_Game, Id_Player1, Id_Player2)VALUES(@Id_Game,@Id_Player1, @Id_Player2)", sqlConnection);
            insertUser.Parameters.AddWithValue("Id_Player1", id1);
            insertUser.Parameters.AddWithValue("Id_Player2", id2);
            insertUser.Parameters.AddWithValue("Id_Game", idGame);
            insertUser.ExecuteNonQuery();
            if (sqlConnection != null)
            {
                sqlConnection.Close();
            }

        }

        protected internal void DeleteGame(string idGame)
        {
            SqlConnection sqlConnection = new SqlConnection(connectionString);
            sqlConnection.Open();
            try
            {
                SqlCommand insertUser = new SqlCommand("DELETE FROM [Games] WHERE Id_Game = @Id_Game", sqlConnection);
                insertUser.Parameters.AddWithValue("Id_Game", idGame);
                insertUser.ExecuteNonQuery();

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (sqlConnection != null)
                {
                    sqlConnection.Close();
                }
            }

        }

        private int[] GetNumbers(string s, int count)
        {
            int k = 0;
            int[] numTasks = new int[5];
            while (k != 5)
            {
                int randNum = rnd.Next(0,count);
                if (!s.Contains(randNum.ToString()))
                {
                    numTasks[k] = randNum;
                    k++;
                    s += randNum.ToString() + " ";
                }
            }
            return numTasks;

        }


    }
}
