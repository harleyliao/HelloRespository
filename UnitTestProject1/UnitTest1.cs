using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Serilog;
using FluentAutomation;
using FluentAutomation.Interfaces;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using AnimalDownload;

namespace UnitTestProject1
{

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
            I.Enter("美女").In("input#kw").Click("input.s_btn");
            I.WaitForAjaxFinish();
            I.Click("div#userInfo>a:eq(0)").WaitForAjaxFinish();
            int total = 0;
            for (int i = 0; i < 1; i++)
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
