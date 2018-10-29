using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace SmartDeviceConsole
{

    class HttpUtils
    {
        private static readonly HttpUtils instance = new HttpUtils();

        private HttpUtils()
        {

        }

        public static HttpUtils Instance
        {
            get
            {
                return instance;
            }
        }

        public string CallGet(string url, out string statusCode)
        {
            string responseStr = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            request.ContentType = "text/html;charset=UTF-8";

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                statusCode = response.StatusCode.ToString();

                if (statusCode.Equals("OK"))
                {
                    Stream responseStream = response.GetResponseStream();
                    StreamReader responseStreamReader = new StreamReader(responseStream, Encoding.UTF8);

                    responseStr = responseStreamReader.ReadToEnd();

                    responseStreamReader.Close();
                    responseStream.Close();

                }

                return responseStr;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);

                statusCode = "FAIL";
                return responseStr;
            }
            

            

        }

        public string CallPost(string url, out string statusCode, string postData)
        {
            string responseStr = "";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json;charset=UTF-8";
            request.ContentLength = Encoding.UTF8.GetByteCount(postData);

            byte[] postBytes = Encoding.UTF8.GetBytes(postData);

            Stream requestStream = request.GetRequestStream();
            requestStream.Write(postBytes, 0, postBytes.Length);
            requestStream.Close();

            try
            {
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                statusCode = response.StatusCode.ToString();

                if (statusCode.Equals("OK"))
                {
                    Stream responseStream = response.GetResponseStream();
                    StreamReader responseStreamReader = new StreamReader(responseStream, Encoding.UTF8);

                    responseStr = responseStreamReader.ReadToEnd();

                    responseStreamReader.Close();
                    responseStream.Close();

                }

                return responseStr;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                statusCode = "FAIL";
                return responseStr;
            }
        }
    }
}
