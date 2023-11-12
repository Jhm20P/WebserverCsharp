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

        public string HttpGetRequest(HttpCommand request, string path, string incomingMessage){
            if (request == HttpCommand.GET){
                return CheckPath(path);
            } else if (request == HttpCommand.POST){
                var postData = GetPostData(incomingMessage);
                Console.WriteLine(postData);
                return CheckPath(path);
            } else {
                return "html/404.html";
            }
        }

        
        public string CheckPath(string path){
           var potentialFilePath = $"{path}.html";
            if(path == "html/"){
                return "html/index.html";  
            } else if (!path.Contains("/favicon.ico") && File.Exists(potentialFilePath)){
                return potentialFilePath;   
            } else {
                return "html/404.html";
            }
        }

        public Dictionary<string, string> GetPostData(string incomingMessage){
            var splitIncommingMessage = incomingMessage.Split(Environment.NewLine);
            bool harMødtTomtLinje = false;
            bool harMødtTomtLinje2 = false;
            Dictionary<string, string> dict = new Dictionary<string, string>();
            foreach (var item in splitIncommingMessage){
                if (item.Length == 0 ){
                    harMødtTomtLinje = true;
                }
                if (harMødtTomtLinje && harMødtTomtLinje2){
                    foreach (var item2 in item.Split('&')){
                        
                        var splitItem2 = item2.Split('=');
                        dict.Add(splitItem2[0], splitItem2[1]);
                    }
                }
            }
            return dict;
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
                var httpResponse  = HttpGetRequest(requestType, path, incomingMessage);
                
                Console.WriteLine("---------------------------------------------------------------");
                Console.WriteLine("Incoming message:");
                Console.WriteLine(incomingMessage);

               var httpBody = $"{File.ReadAllText(httpResponse)}";  
              var  httpResonse = $"HTTP/1.0 200 ok" + Environment.NewLine
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