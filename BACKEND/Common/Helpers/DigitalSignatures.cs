using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Common.Helpers
{
    public static class Encrypt
        {
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }
        /// <summary>
        /// Mã hóa các dữ liệu cần kiểm tra sự thay đổi
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="FieldToEncode"></param>
        /// <returns></returns>
        public static string GetDataToEndcode<T>(T data) where T : IList
        {
            StringBuilder sb = new StringBuilder();
            foreach (var listItem in data)
            {
                IList<PropertyInfo> lstProp = new List<PropertyInfo>(listItem.GetType().GetProperties());
                foreach (PropertyInfo propInfo in lstProp)
                {
                    PropertyInfo info = listItem.GetType().GetProperty(propInfo.Name);
                    var att = info.GetCustomAttribute(typeof(IsCheckChangeAttribute), true) as IsCheckChangeAttribute;
                    if (att != null && att.isCheckChange)
                    {
                        sb.Append($"{info.GetValue(listItem, null)?.ToString().StandardizedText()}");
                    }
                }
            }
            return Base64Encode(sb.ToString());
        }
    }
    /// <summary>
    /// Attribute để đánh dấu trường này có đưa vào để kiểm tra sự thay đổi giá trị hay không
    /// </summary>
    public class IsCheckChangeAttribute : System.Attribute
    {
        public bool isCheckChange;
        public IsCheckChangeAttribute(bool IsCheckChange)
        {
            this.isCheckChange = IsCheckChange;
        }
    }
    public class ImportAttribute : System.Attribute
    {
        public string Name;
        public ImportAttribute(string name)
        {
            this.Name = name;
        }
    }
}
