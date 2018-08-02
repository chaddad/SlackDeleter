using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Configuration;
using System.Collections.Specialized;

namespace SlackDeleter
{

    class Program
    {
        static void Main(string[] args)
        {
            const string title = @"
(`|   _|   |\ _ | _ |- _   
_)|(|(_|<  |/(/_|(/_|_(/_|`
                           
";
            Console.WriteLine(title);

            // get list of files to delete
            var files = GetFiles();

            Console.WriteLine(string.Format("Found {0} files to delete, any key to continue ... ", files.Count));
            Console.ReadLine();

            // loop through each file to make a POST request to delete the file
            foreach (var item in files)
            {
                Console.WriteLine(string.Format("Deleting file: {0}", item));

                //DeleteFile(item);

                // Slack rate limits the Delete API endpoint, this should prevent us from going over those rate limits
                System.Threading.Thread.Sleep(new TimeSpan(0, 0, 0, 2));
            }

            Console.WriteLine("*** job complete, press any key to continue ***");
            Console.ReadLine();
        }

        /// <summary>
        /// Uses your Slack authentication token to retrieve a list of files for your Slack that are over 5 days old
        /// </summary>
        /// <returns>List of file IDs from your Slack that are over 5 days old</returns>
        static private List<string> GetFiles()
        {
            string authToken = ConfigurationManager.AppSettings.Get("authToken");

            var ret = new List<string>();
            string fileListUrl = @"https://slack.com/api/files.list?token={0}&count=1000&ts_to={1}&pretty=1";
            long pastTicks = new DateTimeOffset(DateTime.Now.AddDays(-5)).ToUnixTimeSeconds();

            string fileContents = string.Empty;

            // request files
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(string.Format(fileListUrl, authToken, pastTicks.ToString()));
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                fileContents = streamReader.ReadToEnd();
            }

            // deserialize JSON
            dynamic obj = JsonConvert.DeserializeObject(fileContents);

            // build return list
            foreach (var item in obj.files)
            {
                //if (item.created.Value < pastTicks)
                //{
                    ret.Add(item.id.Value);
                //}
            }

            return ret;
        }

        /// <summary>
        /// Uses your Slack authentication token to make a Delete POST request for a specific file
        /// </summary>
        /// <param name="File">string of the Slack File ID to delete</param>
        static private void DeleteFile(string File)
        {
            string authToken = ConfigurationManager.AppSettings.Get("authToken");
            string deleteUri = "https://slack.com/api/files.delete";

            // each file has to be deleted individually, this is a common HTTP POST request with appropriate headers
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(deleteUri);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";
            httpWebRequest.Headers.Add("Authorization", string.Format("Bearer {0}", authToken));

            // load current file to delete into model
            var p = new Models.DeletePayload { FileId = File };

            // make POST request to delete file
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                string json = JsonConvert.SerializeObject(p);

                streamWriter.Write(json);
                streamWriter.Flush();
                streamWriter.Close();
            }

            // read response and provide output to user
            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var result = streamReader.ReadToEnd();
                Console.WriteLine(result);
            }
        }
    }
}
