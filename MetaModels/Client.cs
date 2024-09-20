using System.Net.Sockets;
using System.Text;

namespace trading_bot_3.MetaModels
{
    public class Client
    {
        public string Command(string command)
        {
            TcpClient client = new TcpClient("127.0.0.1", 8080);
            NetworkStream stream = client.GetStream();
            byte[] data = Encoding.UTF8.GetBytes(command);
            stream.Write(data, 0, data.Length);

            byte[] responseData = new byte[1024];
            int bytes = stream.Read(responseData, 0, responseData.Length);
            stream.Close();
            return Encoding.UTF8.GetString(responseData, 0, bytes);
        }


    }
}
