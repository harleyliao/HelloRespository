using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnimalDownload
{
    public static class RandomDataGenerator
    {
        private static Random randomVar = new Random(DateTime.Now.Millisecond);

        public static int RandomInt(int maxValue)
        {
            return randomVar.Next(maxValue);
        }

        public static int RandomInt(int minValue, int maxValue)
        {
            return randomVar.Next(minValue, maxValue);
        }

        public static bool RandomBool(int biasToReturningTrue = 50)
        {
            return RandomInt(100) < biasToReturningTrue;
        }

        public static string RandomString(int length)
        {
            if (length == 0)
            {
                return string.Empty;
            }

            string baseString = Guid.NewGuid().ToString().ToLower();
            var tempString = baseString.Replace("-", "");
            while (tempString.Length < length)
            {
                tempString += baseString;
            }

            tempString = tempString.Substring(0, length);
            return tempString;
        }

        public static string RandomString(int minLength, int maxLength)
        {
            if (minLength < 0 || maxLength < 0 || minLength > maxLength)
            {
                throw new Exception("Invalid input.");
            }

            int length = RandomInt(minLength, maxLength);

            string baseString = Guid.NewGuid().ToString().ToLower();
            var tempString = baseString.Replace("-", "");
            while (tempString.Length < length)
            {
                tempString += baseString;
            }

            tempString = tempString.Substring(0, length);
            return tempString;
        }

        public static string RandomName(string name = "Name")
        {
            string randomName = RandomString(5);

            randomName = string.Format("{0}{1}", name, randomName.ToString());
            return randomName;
        }

        public static string RandomEmail(string email = "fmTest@gmail.com")
        {
            string randomEmail = RandomString(10);
            String[] substrings = email.Split('@');
            randomEmail = string.Format("{0}{1}@{2}", substrings[0], randomEmail.ToString(), substrings[1]);
            return randomEmail;
        }

        public static string RandomWebsite(string website = "http://gofmtest.com")
        {
            string randomWebsite = RandomString(5);
            String[] substrings = website.Split('.');
            randomWebsite = string.Format("{0}{1}.{2}", substrings[0], randomWebsite.ToString(), substrings[1]);
            return randomWebsite;
        }

        public static string RandomPriority()
        {
            List<string> priorities = new List<string>()
            {
            "High",
            "Medium",
            "Critical",
            "Low"
            };
            return priorities.OrderBy(s => Guid.NewGuid()).First().ToString();
        }

        public static long RandomLong(int length = 19)
        {
            byte[] number = Guid.NewGuid().ToByteArray();
            long temp = Convert.ToInt64(BitConverter.ToInt64(number, 0).ToString().Substring(0, length));
            return temp;
        }

        public static double RandomDouble(int minValue = 0, int maxValue = 1000)
        {
            return (maxValue - minValue) * randomVar.NextDouble() + minValue;
        }

        public static DateTime RandomTime(bool history = false)
        {
            int days = RandomInt(1, 365);
            int hours = RandomInt(1, 24);
            int mins = RandomInt(1, 60);
            int seconds = RandomInt(1, 60);

            var timespan = new TimeSpan(days, hours, mins, seconds);
            return history ?
                DateTime.Now - timespan :
                DateTime.Now + timespan;
        }
    }
}
