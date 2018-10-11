using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Model;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using API;

namespace EchoServer
{
    class MainClass
    {
        private const int _port = 5000;
        private const string _host = "127.0.0.1";
        private static IPAddress _address = IPAddress.Parse(_host);
        private static TcpListener _server = new TcpListener(_address, _port);

        public static string fullURLRegex = @"^\/(api\d*?)\/([\w\-\+]+)\/?(\d+)?$";
        public static string forceFullURLRegex = @"^\/(api\d*?)\/([\w\-\+]+)\/?(\d+)$";
        public static string noIdURLRegex = @"^\/(api\d*?)\/([\w\-\+]+)\/?$";

        public static object JObject { get; private set; }

        public static void Main(string[] args)
        {
            CategoriesController.InitDatabase();

            _server.Start();
            Console.WriteLine($"Server started at port {_port}");

            while (true)
            {
                TcpClient client = _server.AcceptTcpClient();
                Thread clientThread = new Thread(new ParameterizedThreadStart(RunClientThread));
                clientThread.Start(client);
            }
        }

        private static void RunClientThread(object obj)
        {
            TcpClient client = (TcpClient)obj;
            NetworkStream stream = client.GetStream();

            // for any error
            try
            {
                // check if we can read the stream or not
                if (stream.CanRead)
                {

                    //convert stream into byte array
                    Byte[] buffer = new byte[client.ReceiveBufferSize];

                    int readCount = stream.Read(buffer, 0, buffer.Length);
                    string payload = Encoding.UTF8.GetString(buffer, 0, readCount);
                    Console.WriteLine("Payload: " + payload);

                    List<Status> statuses;
                    try
                    {
                        JObject _request = (Newtonsoft.Json.Linq.JObject)JObject.ToString();
                        statuses = IsRequestValid(_request);
                    }
                    catch (Exception)
                    {
                        SendResponse(new Response(new Status(4)), stream);
                        return;
                    }


                    if (statuses.Count > 0)
                    {
                        //Console.WriteLine("Errors in packet: " + errors);
                        SendResponse(new Response(statuses), stream);
                        return;
                    }

                    Request request = JsonConvert.DeserializeObject<Request>(payload);

                    //Response response = CategoriesController.ProcessRequest(request);
                    //SendResponse(response, stream);

                }
                else
                {
                    SendResponse(new Response("no request"), stream);
                    Console.WriteLine("Error: no request");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e);
                return;
            }
        }

        private static void SendResponse(Response response, NetworkStream stream)
        {
            //var response = new Response(message);
            var serializedResponse = JsonConvert.SerializeObject(response);
            var responseByteArray = Encoding.UTF8.GetBytes(serializedResponse);
            stream.Write(responseByteArray, 0, responseByteArray.Length);
        }

        private static List<Status> IsRequestValid(JObject request)
        {
            List<Status> statuses = new List<Status>();

            bool methodNull = request["method"] == null;
            bool pathNull = request["path"] == null;
            bool dateNull = request["date"] == null;
            bool bodyNull = request["body"] == null;


            string apiRegex = fullURLRegex;


            if (dateNull)
            {
                statuses.Add(new Status(4, "missing date"));
            }
            else
            {
                long _datetime;
                bool isEpoch = long.TryParse(request["date"].ToString(), out _datetime);
                if (!isEpoch)
                {
                    statuses.Add(new Status(4, "illegal date"));
                }
            }


            if (methodNull)
            {
                statuses.Add(new Status(4, "missing method"));
            }
            else
            {
                bool validMethod = (request["method"].ToString() == "create" || request["method"].ToString() == "update" || request["method"].ToString() == "delete" || request["method"].ToString() == "read" || request["method"].ToString() == "echo");
                if (!validMethod)
                {
                    statuses.Add(new Status(4, "illegal method"));
                }
                else
                {
                    if (request["method"].ToString() != "echo" && !bodyNull)
                    {
                        try
                        {
                            var body = JsonConvert.DeserializeObject(request["body"].ToString());
                        }
                        catch (Exception e)
                        {
                            statuses.Add(new Status(4, "illegal body"));
                        }
                    }
                }
            }


            if (bodyNull)
            {
                if (!methodNull)
                {
                    if (request["method"].ToString() == "create" || request["method"].ToString() == "update" || request["method"].ToString() == "echo")
                    {
                        statuses.Add(new Status(4, "missing body"));
                    }
                }
            }
            else
            {
                // if body is not null
            }

            if (pathNull)
            {
                if (!methodNull)
                {
                    if (request["method"].ToString() == "read" || request["method"].ToString() == "create" || request["method"].ToString() == "update" || request["method"].ToString() == "delete")
                    {
                        statuses.Add(new Status(4, "missing resource"));
                    }
                }
            }
            else
            {
                string _tempRegex = apiRegex;
                if (!methodNull)
                {
                    if (request["method"].ToString() == "create")
                    {
                        _tempRegex = noIdURLRegex;
                    }

                }
                if (!Regex.IsMatch(request["path"].ToString(), _tempRegex))
                {
                    statuses.Add(new Status(4));
                }
            }

            return statuses;
        }
    }
}
