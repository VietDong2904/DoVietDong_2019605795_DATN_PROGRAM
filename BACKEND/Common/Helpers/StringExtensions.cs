using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.Helpers
{
    public static class StringExtensions
    {
        public static string RemoveAccents(this string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            string[] _vietNamChar = new string[]
            {
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
            };
            var result = input.ToLower();
            for (int i = 1; i < _vietNamChar.Length; i++)
            {
                for (int j = 0; j < _vietNamChar[i].Length; j++)
                    result = result.Replace(_vietNamChar[i][j], _vietNamChar[0][i - 1]);
            }
            return result;
        }
        public static string GetTextInner(this string input, string firstText, string secondText)
        {
            if (!input.ToLower().Contains(firstText.ToLower()) || !input.ToLower().Contains(secondText.ToLower()))
            {
                return string.Empty;
            }
            string regexString = string.Format("{0}(.*?){1}", firstText, secondText);
            var regex = new Regex(regexString);
            return regex.Match(input).Groups[1].Value;
        }
        public static string GetLastFrom(this string input, string text)
        {
            List<string> array = input.Split(text.ToCharArray()).ToList();
            return array.Last();
        }
        public static string ToFilePath(this string input)
        {
            string result = input.Replace(@"\", @"/").Replace(@"//", "/");
            return result;
        }
        public static string ToVndMoneyFormat(this string input)
        {
            CultureInfo cul = CultureInfo.GetCultureInfo("vi-VN");
            string result = double.Parse(input).ToString("#,###", cul.NumberFormat);
            return result;
        }
        public static string ToDayOfTheWeek(this DateTime input)
        {
            var day = input.DayOfWeek;
            switch (day)
            {
                case DayOfWeek.Monday:
                    return "Thứ hai";
                case DayOfWeek.Tuesday:
                    return "Thứ ba";
                case DayOfWeek.Wednesday:
                    return "Thứ tư";
                case DayOfWeek.Thursday:
                    return "Thứ năm";
                case DayOfWeek.Friday:
                    return "Thứ sáu";
                case DayOfWeek.Saturday:
                    return "Thứ bảy";
                case DayOfWeek.Sunday:
                    return "Chủ nhật";
                default:
                    return String.Empty;
            }
        }
        public static string GeneratePassword()
        {
            char[] sourcesChar = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();
            char[] sourcesNumber = "0123456789".ToCharArray();
            char[] sourceSign = "!@#$%^&*()+-{}:<>~".ToCharArray();
            StringBuilder rs = new StringBuilder();
            Random rad = new Random();
            for (int i = 0; i < 4; i++)
            {
                rs.Append(sourcesChar[rad.Next(51)]);
            }
            rs.Append(sourceSign[rad.Next(17)]).Append(sourceSign[rad.Next(17)]);
            rs.Append(sourcesNumber[rad.Next(9)]).Append(sourcesNumber[rad.Next(9)]);
            return rs.ToString();
        }
        public static string ToFormatString(this string input, bool hasDauCham = false)
        {
            if (hasDauCham)
                return CustomeTrim(input.Replace("…", "").Trim());
            else
                return CustomeTrim(input.Replace(".", "").Replace("…/", "").Replace("…", "").Trim());

        }
        private static string CustomeTrim(string OriginalText)
        {
            OriginalText = OriginalText.Trim();
            OriginalText = Regex.Replace(OriginalText, @"\s+", "《");
            return Regex.Replace(OriginalText, @"《", " "); ;
        }
        /// <summary>
        /// Chuẩn hóa text
        /// </summary>
        /// <param name="OriginalText"></param>
        /// <returns></returns>
        public static string StandardizedText(this string OriginalText)
        {
            if (!String.IsNullOrEmpty(OriginalText))
            {
                OriginalText = OriginalText.Trim();
                OriginalText = Regex.Replace(OriginalText, @"\s+", "《");
                return Regex.Replace(OriginalText, @"《", " ");
            }
            else return OriginalText;
        }
        /// <summary>
        /// Chuẩn hóa text => LowerCase
        /// </summary>
        /// <param name="OriginalText"></param>
        /// <returns></returns>
        public static  string StandardizedTextLowerCase(this string OriginalText)
        {
            if (!String.IsNullOrEmpty(OriginalText))
            {
                OriginalText = OriginalText.Trim();
                OriginalText = Regex.Replace(OriginalText, @"\s+", "《");
                return Regex.Replace(OriginalText, @"《", " ").ToLower();
            }
            else return OriginalText;
        }
        public static string LoaiDau(this string str)
        {
            Regex regex = new Regex("\\p{IsCombiningDiacriticalMarks}+");
            string temp = str.Normalize(NormalizationForm.FormD);
            return regex.Replace(temp, String.Empty)
                        .Replace('đ', 'd').Replace('Đ', 'D');
        }
    }
}
