namespace SimpleHttpServer
{
    using System;
    using System.ComponentModel.Design;
    using System.Net;
    using System.Net.Cache;
    using System.Net.Sockets;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Net.Http;
    using Newtonsoft.Json;
    using Microsoft.VisualBasic;
    using System.Runtime.CompilerServices;

    public static class Program
    {
        public static void Main(string[] args)
        {
            IHttpServer server = new HttpServer(13000);
            server.Start();
        }
    }

 

    public interface IHttpServer
    {
        void Start();
    }
    
    
    public class HttpServer : IHttpServer
    {
        private readonly TcpListener listener;

 

        public HttpServer(int port)
        {
            listener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
        }

        public enum HttpCommand
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        public HttpCommand GetHttpCommand(string incomingMessage)
        {
            var request = incomingMessage.Split(' ')[0];
            switch (request)
            {
                case "GET":
                    return HttpCommand.GET;
                case "POST":
                    return HttpCommand.POST;
                case "PUT":
                    return HttpCommand.PUT;
                case "DELETE":
                    return HttpCommand.DELETE;
                default:
                    throw new Exception("Unknown command");
            }
        }

       public string GetPath(string headerPath){
            System.Console.WriteLine("If this prits getpath works");
            string absolutePath = "html";
            var narrowPath = headerPath.Split(' ')[1];

            string fullPath = absolutePath + narrowPath;
               Console.WriteLine("This is the full local path: " + fullPath);
            return fullPath;
            }

        public string HttpGetRequest(HttpCommand request, string path){
            if (request == HttpCommand.GET){
                return CheckPath(path);
            } else {
                return "404";
            }
        }
        public string CheckPath(string path){
            // if (path == "/html/about"){
            //     return "html/about.html";
            // } else if (path == "/html/contact"){
            //     return "html/contacts.html";
            // } else if (path == "/html/services"){
            //     return "html/service.html";
            //  }if(path == "/html/"){
            //     return "html/index.html";
            // } else {
            //     return "html/404.html";
            // }
            // var lastPath = path.Split('/')[2];
            if(path == path){
                return $"{path}.html";
            } else if (path == "/"){
                return "html/index.html";
            } else {
                return "html/404.html";
                
            }
        }


        public void Start()
        {
            int i = 0;
            string incomingMessage;
            listener.Start();
            while (true)
            {
                Console.WriteLine("Waiting for client {0}", i++);
                var client = listener.AcceptTcpClient();
                Console.WriteLine("TcpClient accepted");

 

                var buffer = new byte[10240];
                var stream = client.GetStream();
                Console.WriteLine("Got Client Stream obj");

 

                var length = stream.Read(buffer, 0, buffer.Length);
                Console.WriteLine("Read stream to buffer {0} bytes", length);
                 incomingMessage = Encoding.UTF8.GetString(buffer, 0, length);
                Console.WriteLine("Buffer decoded to string:");
                
                var requestType = GetHttpCommand(incomingMessage);
                var path = GetPath(incomingMessage);
                var httpResponse = HttpGetRequest(requestType, path);
                Console.WriteLine("---------------------------------------------------------------");
                Console.WriteLine("Incoming message:");
                Console.WriteLine(incomingMessage);

                // var fullUrl = listener.ToString() + incomingMessage.Split(' ')[1];
                
                // string? httpBody;
                // switch (true)
                // {
                //     case true when fullUrl.Contains("/about"):
                //          httpBody = $"{File.ReadAllText("about.html")}";

                //         break;
                //     case true when fullUrl.Contains("/contact"):
                //         httpBody = $"{File.ReadAllText("contacts.html")}";
                //         break;
                //     case true when fullUrl.Contains("/services"):
                //         httpBody = $"{File.ReadAllText("service.html")}";
                //         break;
                //     case true when fullUrl.Contains("/"):
                //         httpBody = $"{File.ReadAllText("index.html")}";
                //         break;
                //     default:
                //         httpBody = $"{File.ReadAllText("404")}";
                //         break;

                // }
                
               var httpBody = $"{File.ReadAllText(httpResponse)}";  
               var httpResonse = "HTTP/1.0 200 OK" + Environment.NewLine
                                + "Content-Length: " + httpBody.Length + Environment.NewLine
                                + "Content-Type: " + "text/html" + Environment.NewLine
                                + Environment.NewLine
                                + httpBody
                                + Environment.NewLine + Environment.NewLine;

                
                Console.WriteLine("===============================================================");
                Console.WriteLine("Response message:");
                Console.WriteLine(httpResonse);
                Console.WriteLine("---------------------------------------------------------------");
                
                stream.Write(Encoding.UTF8.GetBytes(httpResonse));
                Console.WriteLine("Response networkstream written");
                stream.Flush();
                Console.WriteLine("Response networkstream Flushed");
                stream.Close();
                Console.WriteLine("Response networkstream Closed");
                client.Close();
                Console.WriteLine("TcpClient closed");
                Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                Thread.Sleep(100);
            }
        }
         
    }
}