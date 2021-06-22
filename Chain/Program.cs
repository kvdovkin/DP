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
            System.Console.ReadLine();
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

            int initNumber = Convert.ToInt32(initString);
            byte[] numberAsBytes = BitConverter.GetBytes(initNumber);

            _sender.Send(numberAsBytes);

            Socket handler = _listener.Accept();
            byte[] value = new byte[sizeof(int)];
            handler.Receive(value);

            int recievedInt = BitConverter.ToInt32(value);

            _sender.Send(BitConverter.GetBytes(recievedInt));

            Console.WriteLine(recievedInt);

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

            int normNumber = Convert.ToInt32(normString);

            Socket handler = _listener.Accept();
            byte[] value = new byte[sizeof(int)];
            handler.Receive(value);

            int recievedInt = BitConverter.ToInt32(value);

            _sender.Send(BitConverter.GetBytes(Math.Max(recievedInt, normNumber)));

            Console.WriteLine(recievedInt);

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
    }
}