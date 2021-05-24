using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Server
{
    class Program
    {
        public static List<string> msgCollection = new List<string>();
        private const string Eof = "<EOF>";
        public static void StartListening(int ipNumber)
        {

            // Разрешение сетевых имён
            
            // Привязываем сокет ко всем интерфейсам на текущей машинe
            IPAddress ipAddress = IPAddress.Any; 
            
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, ipNumber); 

            // CREATE
            Socket listener = new Socket(
                ipAddress.AddressFamily,
                SocketType.Stream,
                ProtocolType.Tcp);

            try
            {
                // BIND
                listener.Bind(localEndPoint);

                // LISTEN
                listener.Listen(10);

                while (true)
                {

                    // ACCEPT
                    Socket handler = listener.Accept();

                    byte[] buf = new byte[1024];
                    string data = null;
                    while (true)
                    {
                        // RECEIVE
                        int bytesRec = handler.Receive(buf);

                        data += Encoding.UTF8.GetString(buf, 0, bytesRec);
                        if (data.IndexOf(Eof) > -1)
                        {
                            break;
                        }
                    }
                    data = data.Remove(data.Length - 5, 5);
                    msgCollection.Add(data);

                    Console.WriteLine("Полученный текст: {0}", data);

                    // Отправляем текст обратно клиенту
                    string msgJson = JsonSerializer.Serialize(msgCollection);

                    byte[] msg = Encoding.UTF8.GetBytes(msgJson);

                    // SEND
                    handler.Send(msg);

                    // RELEASE
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("Invalid arguments count");
            }
            
            StartListening(Int32.Parse(args[0]));
        }
    }
}
