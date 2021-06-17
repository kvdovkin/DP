using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using static Chain.Constants;

namespace Chain
{
    class Program
    {
        private static Socket _listener;
        private static Socket _sender;
        public static bool CheckingInitiation;
        private static IAsyncResult _start;

        static void Main(string[] args)
        {
            try
            {
                Params arguments = FindArgsFromParams(args);

                CreateConnection(arguments.NextPort, arguments.ListeningPort, arguments.NextHost);

                if (CheckingInitiation)
                {
                    Initiator();
                }
                else
                {
                    NormalProcess();
                }

                _sender.Shutdown(SocketShutdown.Both);
                _sender.Close();
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static Params FindArgsFromParams(string[] args)
        {
            if (args.Length < 2 || args.Length > 4)
            {
                throw new ArithmeticException(Constants.Exception);
            }

            Params parametrs = new()
            {
                ListeningPort = Int32.Parse(args[0]),
                NextHost = args[1],
                NextPort = Int32.Parse(args[2])
            };

            if (args.Length == 4 && args[3] == flagIn)
                CheckingInitiation = true;

            return parametrs;
        }

        private static void CreateConnection(int listeningPort, int nextPort, string nextHost)
        {
            IPAddress ipAddress = IPAddress.Any;
            IPEndPoint localEndPoint = new(ipAddress, Convert.ToInt32(listeningPort));
            _listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            _listener.Bind(localEndPoint);
            _listener.Listen(10);

            if (nextHost == Host)
            {
                ipAddress = IPAddress.Loopback;

            }
            else
            {
                ipAddress = IPAddress.Parse(nextHost);
            }

            IPEndPoint remoteEP = new(ipAddress, Convert.ToInt32(nextPort));

            _sender = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            CreateSocketConnection(remoteEP);
        }

        private static void CreateSocketConnection(IPEndPoint remoteEP)
        {
            _start = _sender.BeginConnect(remoteEP, null, null);

            bool success = _start.AsyncWaitHandle.WaitOne(7000, true);
            if (success)
            {
                _sender.EndConnect(_start);
            }
            else
            {
                _sender.Close();
                throw new SocketException(10060);
            }
        }

        private static void Initiator() 
        {
            string initString = Console.ReadLine();

            if (initString == null)
            {
                throw new Exception(Constants.Exception);
            }

            _sender.Send(Encoding.UTF8.GetBytes(initString));

            Socket handler = _listener.Accept();
            byte[] value = new byte[sizeof(int)];
            handler.Receive(value);

            string recievedString = Encoding.UTF8.GetString(value);

            _sender.Send(Encoding.UTF8.GetBytes(recievedString));

            Console.WriteLine(recievedString);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }

        private static void NormalProcess()
        {
            string normString = Console.ReadLine();

            if (normString == null)
            {
                throw new Exception(Constants.Exception);
            }

            Socket handler = _listener.Accept();
            byte[] value = new byte[sizeof(int)];
            handler.Receive(value);

            string recievedString = Encoding.UTF8.GetString(value);

            _sender.Send(Encoding.UTF8.GetBytes(Math.Max
                (Convert.ToInt32(normString), Convert.ToInt32(recievedString)).ToString()));

            handler.Receive(value);

            _sender.Send(value);

            Console.WriteLine(Encoding.UTF8.GetString(value));

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
    }
}