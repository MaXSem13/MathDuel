using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatServer
{
    public class Game
    {
        public ClientObject player1 = null;
        public ServerObject server = null;
        public ClientObject player2 = null;
        public string[] tasks = new string[5];
        public string[] answ = new string[5];
        public string[] idTasks = new string[5];
        public string idGame = "";
        public Game(ClientObject client1,ClientObject client2, ServerObject server, string idGame)
        {
            player1 = client1;
            player1.numinGame = 1;
            player2 = client2;
            player2.numinGame = 2;
            this.server = server;
            this.idGame = idGame;
            //добавить в БД информацию что пользователи в игре
            server.SetInGame(player1.GetName());
            server.SetInGame(player2.GetName());
            server.AddGame(player1.GetName(), player2.GetName(), idGame);

        }

        public void SendTasks()
        {
            string tasksStr = "t/asks";
            for (int i = 0; i < tasks.Length-1; i++)
            {
                tasksStr += tasks[i] + "//";
            }
            tasksStr += tasks[tasks.Length - 1];

            string answersStr = "";
            for (int i = 0; i < answ.Length - 1; i++)
            {
                answersStr += answ[i] + "//";
            }
            answersStr += answ[answ.Length - 1];

            string comMsg = tasksStr + "__" + answersStr;
            server.CallBack(comMsg, player1);
            server.CallBack(comMsg, player2);
            


        }
    }
}
