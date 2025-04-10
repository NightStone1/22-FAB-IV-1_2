using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace server
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Запустить сервер (1) или клиент (2)?");
            while (true)
            {
                var choice = Console.ReadLine();
                if (choice == "1")
                {
                    Server.Start();
                    break;
                }
                else if (choice == "2")
                {
                    Client.Start();
                    break;
                }
                else
                {
                    Console.WriteLine("Неверный выбор");
                }
            }           
        }
    }

    public static class Server
    {
        public static void Start()
        {
            try
            {
                IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000);
                Socket listener = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                listener.Bind(ipEndPoint);
                listener.Listen(10);
                Console.WriteLine($"Сервер запущен на {ipEndPoint}");

                while (true)
                {
                    Socket handler = listener.Accept();
                    ThreadPool.QueueUserWorkItem(HandleClient, handler);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static void HandleClient(object state)
        {
            Socket handler = (Socket)state;
            try
            {
                byte[] buffer = new byte[1024];
                int received = handler.Receive(buffer);
                string data = Encoding.UTF8.GetString(buffer, 0, received);

                Console.WriteLine($"Получено: {data}");
                Replacer(ref data);

                string reply = $"Ответ сервера ({data.Length} символов):\n{data}\n";
                handler.Send(Encoding.UTF8.GetBytes(reply));
            }
            finally
            {
                handler.Shutdown(SocketShutdown.Both);
            }
        }

        private static void Replacer(ref string input)
        {
            input = input.Replace('[', '(').Replace(']', ')');
        }
    }

    public static class Client
    {
        public static void Start()
        {
            try
            {
                while (true)
                {
                    Console.Write("Введите сообщение (или 'exit' для выхода): ");
                    string message = Console.ReadLine();

                    if (message.ToLower() == "exit")
                        break;

                    Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    client.Connect(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 11000));

                    client.Send(Encoding.UTF8.GetBytes(message));

                    byte[] buffer = new byte[1024];
                    int received = client.Receive(buffer);
                    Console.WriteLine($"Ответ сервера:\n{Encoding.UTF8.GetString(buffer, 0, received)}\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
