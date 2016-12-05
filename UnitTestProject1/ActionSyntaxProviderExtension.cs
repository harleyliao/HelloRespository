using FluentAutomation;
using FluentAutomation.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestProject1
{
    public static class ActionSyntaxProviderExtension
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
}
