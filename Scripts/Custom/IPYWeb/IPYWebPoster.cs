using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;
using Server.Custom.Townsystem;
using System.Threading;

namespace Server.Custom.IPYWeb
{
    public class IPYWebPoster
    {
        public static void Initialize()
        {
			// DIABLED 
			//Timer timer = Timer.DelayCall(TimeSpan.Zero, TimeSpan.FromMinutes(5), new TimerCallback(StartPostingAsync));
        }

        private static readonly string URI = "http://stats.inporylem.com/update/towns";
        private static byte[] toPost;

        public static void StartPostingSync()
        {
            var p = new Server.IPYWeb.IPYServerStatisticsPacket();
            toPost = p.UnderlyingStream.ToArray();
            p.Release();

            var startDelegate = new ThreadStart(PostUpdateSync);
            var thread = new Thread(startDelegate);
            thread.Start();
        }
        public static void StartPostingAsync()
        {
            var p = new Server.IPYWeb.IPYServerStatisticsPacket();
            toPost = p.UnderlyingStream.ToArray();
            p.Release();

            PostUpdateAsync();

        }
        public static void PostUpdateSync()
        {
            var packetBytes = toPost;

            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URI);
                request.Method = "POST";
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.ContentLength = packetBytes.Length;
                Stream reqStream = request.GetRequestStream();
                reqStream.Write(packetBytes, 0, packetBytes.Length);
                reqStream.Close();
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                //Console.WriteLine(new StreamReader(response.GetResponseStream()).ReadToEnd());
            }
            catch (WebException ex)
            {
                var resp = (HttpWebResponse)ex.Response;
                string toWrite = new StreamReader(resp.GetResponseStream()).ReadToEnd();
                using (StreamWriter sw = new StreamWriter("ipyWebPosterError.html"))
                {
                    sw.WriteLine(toWrite);
                }
            }
            catch
            {

            }
            /*
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URI);
                request.KeepAlive = false;
                request.Method = "POST";
                //request.ContentType = "application/x-www-form-urlencoded";

                var result = request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), request);
            }
            catch
            {

            }*/
        }
        public static void PostUpdateAsync()
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URI);
                request.Method = "POST";
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version10;
                request.ContentLength = toPost.Length;
                var result = request.BeginGetRequestStream(new AsyncCallback(GetRequestStreamCallback), request);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private static void GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                var packetBytes = toPost;
                HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

                using (Stream postStream = request.EndGetRequestStream(asynchronousResult))
                {
                    postStream.Write(packetBytes, 0, packetBytes.Length);
                }

                // Start the asynchronous operation to get the response
                request.BeginGetResponse(new AsyncCallback(GetResponseCallback), request);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        private static void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;

                // End the operation
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);

                string responseString;
                using (StreamReader streamRead = new StreamReader(response.GetResponseStream()))
                {
                    responseString = streamRead.ReadToEnd();
                }
                //Console.WriteLine(responseString);

                // Release the HttpWebResponse
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
