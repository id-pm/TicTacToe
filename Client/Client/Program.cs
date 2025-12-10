using System.Net.Sockets;
using System.Text;

namespace ChatClient
{
    class Program
    {
        static string userName;
        private const string host = "192.168.56.1";
        private const int port = 3210;
        static TcpClient client;
        static NetworkStream stream;
        static string room;
        static string[] cellTable = new[] { "cell:0.0", "cell:0.1", "cell:0.2", "cell:1.0", "cell:1.1", "cell:1.2", "cell:2.0", "cell:2.1", "cell:2.2" };
        static void Main(string[] args)
        {
            Console.Write("Введите свое имя: ");
            userName = Console.ReadLine();
            Console.Write("номер комнаты: ");
            room = Console.ReadLine();
            client = new TcpClient();
            try
            {
                client.Connect(host, port); //подключение клиента
                stream = client.GetStream(); // получаем поток

                string message = $"Name:{userName}|Room:{room}|Side:0";
                byte[] data = Encoding.Unicode.GetBytes(message);
                stream.Write(data, 0, data.Length);

                // запускаем новый поток для получения данных
                Thread receiveThread = new Thread(new ThreadStart(ReceiveMessage));
                receiveThread.Start(); //старт потока
                Console.WriteLine("Добро пожаловать, {0}", userName);
                SendMessage();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Disconnect();
            }
        }
        // отправка сообщений
        static void SendMessage()
        {

            while (true)
            {
                for(int i = 0; i < cellTable.Length; i++)
                {
                    Console.WriteLine($"{i}. {cellTable[i]}");
                }
                Console.WriteLine("Ячейка по номером: ");
                string cell;
                string message;
                do
                {
                    message = Console.ReadLine();
                    cell = cellTable[int.Parse(message)];
                } while (cell == "-");
                cellTable[int.Parse(message)] = "-";
                byte[] data = Encoding.Unicode.GetBytes(cell);
                stream.Write(data, 0, data.Length);
            }
        }
        // получение сообщений
        static void ReceiveMessage()
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
                    if(message == "ERR")
                    {
                        Disconnect();
                    }
                    if (message.Contains("cell"))
                    {
                        Console.Clear();
                        cellTable[cellTable.ToList().IndexOf(message)] = "-";
                        for (int i = 0; i < cellTable.Length; i++)
                        {
                            Console.WriteLine($"{i}. {cellTable[i]}");
                        }
                    }
                    Console.WriteLine(message);//вывод сообщения
                }
                catch
                {
                    Console.WriteLine("Подключение прервано!"); //соединение было прервано
                    Console.ReadLine();
                    Disconnect();
                }
            }
        }

        static void Disconnect()
        {
            if (stream != null)
                stream.Close();//отключение потока
            if (client != null)
                client.Close();//отключение клиента
            Environment.Exit(0); //завершение процесса
        }
    }
}