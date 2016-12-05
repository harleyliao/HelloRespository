using AnimalDownload;
using FluentAutomation.Interfaces;
using OpenQA.Selenium;
using Serilog;
using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;

namespace AnimalDownload
{
    public static class FileUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="size"></param>
        public static void CreateFileOfData(string fileName, int size)
        {
            Log.Information("Creating a data file of " + size + " bytes named " + fileName);
            var directory = Path.GetDirectoryName(fileName);
            if (!(string.IsNullOrEmpty(directory) || Directory.Exists(directory)))
            {
                Directory.CreateDirectory(directory);
            }

            var oFile = new FileStream(fileName, FileMode.Create);
            byte[] bob = Encoding.ASCII.GetBytes(RandomDataGenerator.RandomString(size));
            oFile.Write(bob, 0, bob.Length);
            oFile.Close();
        }

        public static void SimpleDownLoad(IWebDriver driver, string url, string fileName)
        {

            Log.Debug("Starting SimpleDownLoad " + fileName);
            try
            {
                var cookies = CopyCookiesFromDriver(driver);
                var req = (HttpWebRequest)WebRequest.Create(url);
                req.Timeout = 200000;
                req.CookieContainer = cookies;
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                Stream dataStream = res.GetResponseStream();

                Log.Debug("File downloaded, Dumping file now");
                var dir = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }

                var oFile = new FileStream(fileName, FileMode.Create);
                byte[] readBuffer = new byte[1024];
                int size = dataStream.Read(readBuffer, 0, readBuffer.Length);
                while (size > 0)
                {
                    oFile.Write(readBuffer, 0, size);
                    size = dataStream.Read(readBuffer, 0, readBuffer.Length);
                }
                oFile.Flush();
                oFile.Close();
                dataStream.Close();
                res.Close();
            }
            catch (Exception ex)
            {
                Log.Error("Caught an exception during file DownLoad -" + ex);
                throw;
            }
            Log.Information("Finished file download...");

        }

        public static string DownloadToString(IWebDriver driver, string url, Encoding encoding = null)
        {

            Log.Debug("Starting DownloadToString");
            string result = null;
            try
            {
                var cookies = CopyCookiesFromDriver(driver);
                var req = (HttpWebRequest)WebRequest.Create(url);
                req.CookieContainer = cookies;
                HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                Stream dataStream = res.GetResponseStream();

                var downloadedBytes = new byte[res.ContentLength];
                dataStream.Read(downloadedBytes, 0, Convert.ToInt32(res.ContentLength));

                if (encoding == null)
                {
                    encoding = Encoding.Default;
                }

                result = encoding.GetString(downloadedBytes);

                res.Close();
                dataStream.Close();
            }
            catch (Exception ex)
            {
                Log.Error("Caught an exception during file DownLoad -" + ex);
                throw;
            }
            Log.Information("Finished file download...");
            return result;
        }

        private static CookieContainer CopyCookiesFromDriver(IWebDriver driver)
        {
            CookieContainer cookie = new CookieContainer();

            // Steal all the cookies from the webdriver.
            foreach (var cook in driver.Manage().Cookies.AllCookies)
            {
                var nc = new System.Net.Cookie(cook.Name, cook.Value, "/", cook.Domain);
                cookie.Add(nc);
            }

            return cookie;
        }




    }
}
