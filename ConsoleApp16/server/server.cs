﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace server
{
    internal class server
    {
        static void Main(string[] args)
        {
            // Устанавливаем для сокета локальную конечную точку
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);
            // Создаем сокет Tcp/Ip
            Socket sListener = new Socket(ipAddr.AddressFamily,
            SocketType.Stream, ProtocolType.Tcp);
            // Назначаем сокет локальной конечной точке  ислушаем входящие сокеты
            try
            {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);
                // Начинаем слушать соединения
                while (true)
                {
                    Console.WriteLine("Ожидаем соединение через порт {0}", ipEndPoint); // Программа приостанавливается, ожидая входящее соединение
                    Socket handler = sListener.Accept();
                    string data = null;
                    // Мы дождались клиента, пытающегося с нами соединиться
                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);
                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    // Показываем данные на консоли
                    Console.Write("Полученный текст: " + data + "\n\n");
                    // Отправляем ответ клиенту\
                    Replacer(ref data); // Сначала изменяем data
                    string reply = $"Спасибо за запрос в {data.Length} символов\n\n{data}\n\n В сообщениях заменены [] на ()"; // Используем 
                    byte[] msg = Encoding.UTF8.GetBytes(reply);
                    handler.Send(msg);
                    if (data.IndexOf("<TheEnd>") > -1)
                    {
                        Console.WriteLine("Сервер завершил соединение с клиентом.");
                        break;
                    }
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }
        static void Replacer(ref string input)
        {
            input = input.Replace('[', '(').Replace(']', ')');
        }
    }
}