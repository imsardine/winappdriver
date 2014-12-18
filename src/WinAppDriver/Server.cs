using System;
using System.IO;
using System.Net;
using Newtonsoft.Json;

namespace WinAppDriver {

    class Server {

        private RequestManager requestManager;

        public Server(RequestManager requestManager) {
            this.requestManager = requestManager;
        }

        public void Start() {
            var listener = new HttpListener();
            listener.Prefixes.Add("http://+:4444/wd/hub/");
            listener.Start();

            Console.WriteLine("Listening...");
            while (true) {
                var context = listener.GetContext();
                var request = context.Request;
                var response = context.Response;
                object result = HandleRequest(request);

                string json = JsonConvert.SerializeObject(result);
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(json);
                response.ContentLength64 = buffer.Length;
                response.ContentType = "application/json";
                var output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                output.Close();
            }
        }

        private object HandleRequest(HttpListenerRequest request) {
            string method = request.HttpMethod;
            string path = request.Url.AbsolutePath;
            string body = new StreamReader(request.InputStream, request.ContentEncoding).ReadToEnd();
            Console.WriteLine("A request received. {0} {1}\n{2}", method, path, body);

            // TODO put return object(s) into an envelop
            // TODO cache exceptions here, convert into error code
            return requestManager.Handle(method, path, body);
        }

    }

}

