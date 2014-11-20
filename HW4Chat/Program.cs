using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HW4Chat
{
    class Program
    {
        static void Main(string[] args)
        {
            Program receiver = new Program();
            Program sender = new Program();

            Thread ReceiverThread = new Thread(receiver.Server);
            Thread SenderThread = new Thread(sender.Client);

            ReceiverThread.Start();
            SenderThread.Start();
        }

        public void Server()
        {
            using (NamedPipeServerStream pipe = new NamedPipeServerStream("PipeTo" + Process.GetCurrentProcess().Id.ToString()))
            {
                Console.WriteLine("Server Pipe Created! PID is {0}", Process.GetCurrentProcess().Id.ToString());

                pipe.WaitForConnection();
                Console.WriteLine("Server Pipe connection established!");

                using (StreamReader streamReader = new StreamReader(pipe))
                {
                    string message;
                    while ((message = streamReader.ReadLine()) != null)
                    {
                        Console.WriteLine("Incoming message: " + message);
                        Console.Write("Message to send: ");
                    }
                }
            }
            Console.WriteLine("**Connection has been lost**");
        }//Server Method

        public void Client(object obj)
        {
            ManualResetEvent SyncClientServer = (ManualResetEvent)obj;
            Console.WriteLine("Please enter the PID for pipe connection");
            string PIDToCall = Console.ReadLine();

            using (NamedPipeClientStream pipe = new NamedPipeClientStream("PipeTo" + PIDToCall))
            {
                pipe.Connect();

                Console.WriteLine("Client Pipe Connection established!");
                using (StreamWriter streamWriter = new StreamWriter(pipe))
                {
                    streamWriter.AutoFlush = true;
                    string message;
                    Console.WriteLine("Enter a message or type 'END' to exit");
                    while ((message = Console.ReadLine()) != null)
                    {
                        if (message == "END")
                            break;
                        streamWriter.WriteLine(message);
                    }
                }
            }
            
        }//Client Method
    }//class Program
}//namespace
