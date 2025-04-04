using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ConsoleApp12
{
    internal class Program
    {
        public static string SpaceMethod(string input) 
        {
            StringBuilder sb = new StringBuilder();
            bool lastWasSpace = false;

            foreach(char c in input) 
            {
                if (char.IsWhiteSpace(c))
                {
                    if (!lastWasSpace)
                    {
                        sb.Append(c);
                        lastWasSpace = true;
                    }
                }
                else 
                {
                    sb.Append(c);
                    lastWasSpace = false;
                }
            }
            return sb.ToString();
        }
        static void Main(string[] args)
        {
            IPHostEntry ipHost = Dns.GetHostEntry("localhost");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 7200);

            Socket sListener = new Socket(ipAddr.AddressFamily, 
                SocketType.Stream, ProtocolType.Tcp);

            try
            {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);

                while (true)
                {
                    Console.WriteLine("Ожидание соеденения через порт {0}", ipEndPoint);

                    Socket handler = sListener.Accept();
                    string data = null;

                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);

                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);

                    Console.Write("Полученный текст;" + data + "\n\n");

                    string spacereply = data;
                    string normalspacereply = SpaceMethod(spacereply);
               

                    string reply = "Спсибо за запрос в " + data.Length.ToString() + " символво \n\nИзменненое сообщение: " + normalspacereply;
                    byte[] msg = Encoding.UTF8.GetBytes(reply);
                    handler.Send(msg);

                    if (data.IndexOf("<The End>") > -1)
                    {
                        Console.WriteLine("Сервер завершил соеденение с клиентом");
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
    }
}
