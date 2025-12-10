using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

public class Client
{
    private static Client mainClient;
    private static string myHost;
    private static int myPort;
    static TcpClient client;
    static NetworkStream stream;
    private Client(string host,int port)
    {
        myHost = host;
        myPort = port;
    }
    public static Client GetClient()
    {
        if(mainClient == null)
            return new Client(SettingClass.host, SettingClass.port);
        else return mainClient;
    }
    public string StartOnline(string name,string room = "-1")
    {
        
        client = new TcpClient();
        try
        {
            client.Connect(myHost, myPort); //подключение клиента
            stream = client.GetStream(); // получаем поток

            string message = $"Name:{name}|Room:{room}|Side:0";
            byte[] data = Encoding.Unicode.GetBytes(message);
            stream.Write(data, 0, data.Length);
            return ReceiveResponce();
        }
        catch
        {
            Disconnect();
            return "Server OFF";
        }
    }
    // отправка сообщений
    public void SendMessage(string message)
    {
        byte[] data = Encoding.Unicode.GetBytes(message);
        stream.Write(data, 0, data.Length);
    }
    // получение сообщений
    public string ReceiveMessage()
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
                return message;
            }
            catch
            {
                Disconnect();
                return "ERROR";
            }
        }
    }
    private static string ReceiveResponce()
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
                return message;
            }
            catch
            {
                Disconnect();
                return "ERROR";
            }
        }
    }
    static void Disconnect()
    {
        if (stream != null)
            stream.Close();//отключение потока
        if (client != null)
            client.Close();//отключение клиента
    }
    public async Task<string> ReceiveMessageAsync()
    {
        return await Task.Run(() => ReceiveMessage());
    }
}
