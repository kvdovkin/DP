using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace Client
{
    class Program
    {
        private const string defaultHost = "localhost";
        private const string Eof = "<EOF>";
        public static void StartClient(string server, int ipNumber, string message)
        {
            try
            {
                // Разрешение сетевых имён
                IPAddress ipAddress;
                //IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
                //IPAddress ipAddress = ipHostInfo.AddressList[0];
                if (server == defaultHost)
                {
                    ipAddress = IPAddress.Loopback;
                }
                else
                {
                    ipAddress = IPAddress.Parse(server);
                }

                IPEndPoint remoteEP = new IPEndPoint(ipAddress, ipNumber);

                // CREATE
                Socket sender = new Socket(
                    ipAddress.AddressFamily,
                    SocketType.Stream, 
                    ProtocolType.Tcp);

                try
                {
                    // CONNECT
                    sender.Connect(remoteEP);

                    // Подготовка данных к отправке
                    byte[] msg = Encoding.UTF8.GetBytes(message + Eof);
                    // SEND
                    int bytesSent = sender.Send(msg);

                    // RECEIVE
                    byte[] buf = new byte[1024];

                    int byteCounter = 0;
                    string strCollection = null;

                    do
                    {
                        byteCounter = sender.Receive(buf);
                        strCollection += Encoding.UTF8.GetString(buf, 0, byteCounter);
                    }
                    while (byteCounter > 0);

                    var sbCollection = JsonSerializer.Deserialize<List<string>>(strCollection);

                    foreach (var element in sbCollection)
                    {
                        Console.WriteLine(element);
                    }

                    // RELEASE
                    sender.Shutdown(SocketShutdown.Both);
                    sender.Close();

                }
                catch (ArgumentNullException ane)
                {
                    Console.WriteLine("ArgumentNullException : {0}", ane.ToString());
                }
                catch (SocketException se)
                {
                    Console.WriteLine("SocketException : {0}", se.ToString());
                }
                catch (Exception e)
                {
                    Console.WriteLine("Unexpected exception : {0}", e.ToString());
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }

        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                Console.WriteLine("Incorrect args");
            }
            else
            {
                StartClient(args[0], Int32.Parse(args[1]), args[2]);
            }
        }
    }
}
