using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace TTTServer
{
    public class ClientObject
    {
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        TcpClient client;
        ServerObject server; // объект сервера
        ClientObject opponent;
        public string code;
        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            serverObject.AddConnection(this);
        }

        public void Process()
        {
            try
            {
                //1 name 2 room 3 side
                Stream = client.GetStream();
                string message = GetMessage();
                MatchCollection matches = new Regex(@"Name:(.*)\|Room:(-?.+)\|Side:(\d)").Matches(message);
                // посылаем сообщение о входе в чат всем подключенным пользователям
               // server.BroadcastMessage(message, this.Id);
                if(matches[0].Groups[2].Value == "-1")
                {
                    code = Guid.NewGuid().ToString().Split('-')[1].ToUpper() + Id[0].ToString().ToUpper();
                    SendToPlayer("CODE:" + code);
                    Console.WriteLine(code);
                }
                else
                {
                    opponent = server.GetPlayerByCode(matches[0].Groups[2].Value);
                    if (opponent == null)
                    {
                        SendToPlayer("ERR");
                        return;
                    }
                    opponent.opponent = this;
                    SendToOpponent("OK");
                    SendToPlayer("OK");
                    Console.WriteLine($"User poluchil opponenta");
                }
                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        SendToOpponent(message);
                    }
                    catch
                    {
                        Console.WriteLine("Connection lost");
                        SendToOpponent(message);
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
        // трансляция сообщения подключенным клиентам
        protected internal void SendToOpponent(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            opponent.Stream.Write(data, 0, data.Length); //передача данных
        }
        protected internal void SendToPlayer(string message)
        {
            byte[] data = Encoding.Unicode.GetBytes(message);
            Stream.Write(data, 0, data.Length);
        }
        // закрытие подключения
        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }
}