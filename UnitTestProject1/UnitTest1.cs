using System;
using System.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using System.Globalization;
using FluentAutomation;
using FluentAutomation.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using LN.PM.FirmManager.UIAutomation.Specs.Utilities;

namespace UnitTestProject1
{
    public static class ActionSyntaxProviderExtensions
    {
        public static IWebDriver GetWebDriver(this IActionSyntaxProvider I)
        {
            return ((I.Find("html").Element as Element).WebElement as RemoteWebElement).WrappedDriver;
        }
        public static object ExecuteScript(this IActionSyntaxProvider I, string script, params object[] args)
        {
            return (I.GetWebDriver() as IJavaScriptExecutor).ExecuteScript(script, args);
        }
        public static IActionSyntaxProvider WaitForAjaxFinish(this IActionSyntaxProvider I, int timeout = 30000)
        {
            Log.Debug("Enter WaitForAjaxFinish");
            var start = DateTime.Now;
            var end = start.AddMilliseconds(timeout);

            // wait for the script being triggered.
            I.Wait(TimeSpan.FromMilliseconds(300));

            var endCount = 0;

            var count = 100;
            while (DateTime.Now < end && count > endCount)
            {
                try
                {
                    I.Wait(TimeSpan.FromMilliseconds(100));
                    count = Int32.Parse(I.ExecuteScript("return $.active").ToString());
                }
                catch (UnhandledAlertException)
                {
                    throw;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Object reference not set to an instance of an object") ||
                        ex.Message.Contains("$ is not defined"))
                    {
                        count = 0;
                    }
                }
            }

            // wait for any javascript execution after ajax executed
            I.Wait(TimeSpan.FromMilliseconds(300));

            Log.Debug("Leave WaitForAjaxFinish. Waited " + (DateTime.Now - start).TotalMilliseconds);

            return I;
        }
    }

    [TestClass]
    public class UnitTest1 : FluentTest
    {
        [TestMethod]
        public void TestMethod1()
        {
            SeleniumWebDriver.Bootstrap(SeleniumWebDriver.Browser.Chrome);
            FluentConfig.Current.MinimizeAllWindowsOnTestStart(true).WindowMaximized(true);
            var imageURL = "http://image.baidu.com";
            I.Open(imageURL);
            I.WaitUntil(() => I.Assert.Exists("input#kw"));
            I.Enter("牛").In("input#kw").Click("input.s_btn");
            I.WaitForAjaxFinish();
            I.Click("div#userInfo>a:eq(0)").WaitForAjaxFinish();
            int total = 0;
            for (int i = 0; i < 81; i++)
            {
                total = DownloadPic(total);
                I.Click("a.n:contains('下一页')").WaitForAjaxFinish();
            }

        }

        private int DownloadPic(int total)
        {
            var picNo = I.FindMultiple("li.imgitem .hover a.down").Elements.Count;

            for (int i = 0; i < picNo; i++)
            {
                var downUrl = I.Find(string.Format("li.imgitem .hover a.down:eq({0})", i)).Element.Attributes.Get("href");
                FileUtils.SimpleDownLoad(I.GetWebDriver(), downUrl, string.Format("D:\\Cows\\Cow{0}.jpg", total + i + 1));
            }

            return total + picNo;
        }
    }
}
