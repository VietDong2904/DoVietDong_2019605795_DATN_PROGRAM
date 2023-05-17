using Common.Model;
using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Common.CacheService;
using Common.Constants;
using Microsoft.Extensions.Configuration;

namespace Common.Helpers
{
    /// <summary>
    /// Dung lượng file tối đa (byte)
    /// </summary>
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;
        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                if (file.Length > _maxFileSize)
                {
                    
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"Kích thước file không được quá { _maxFileSize/1024/1024} MB!";
        }
    }
    /// <summary>
    /// Các file được phép ( { ".xlsx", ".xls",..... })
    /// </summary>
    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] _extensions;
        public AllowedExtensionsAttribute(string[] extensions)
        {
            _extensions = extensions;
        }
        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            if (file != null)
            {
                var extension = Path.GetExtension(file.FileName);
                if (!_extensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"File tải lên không đúng định dạng!";
        }
    }
    public class Helpers
    {
        private const string FORMULAR_PREFIX = "ROW_";
        private const int START_COLUMN = 3;
        private const int ROUND_RANGE = 2;
        private const int END_COLUMN = 1000;
        private const string INDEX = "_INDEX_";
        private const string CreatedBy = "CreatedBy";
        private const string Created = "Created";
        private const string LastModifiedBy = "LastModifiedBy";
        private const string LastModified = "LastModified";
        private const string SiteId = "SiteId";
        public static string GetConfig(string code)
        {

            IConfigurationRoot configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .Build();

            var value = configuration[code];
            return value;
        }
        public static string GenerateAutoCode(string prefix, long number)
        {
            return prefix + (number > 9999 ? number.ToString() : (10000 + number).ToString().Remove(0, 1)); ;
        }
        public static string PasswordGenerateHmac(string clearMessage, string secretKeyString)
        {
            var encoder = new ASCIIEncoding();
            var messageBytes = encoder.GetBytes(clearMessage);
            var secretKeyBytes = new byte[secretKeyString.Length / 2];
            for (int index = 0; index < secretKeyBytes.Length; index++)
            {
                string byteValue = secretKeyString.Substring(index * 2, 2);
                secretKeyBytes[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }
            var hmacsha512 = new HMACSHA512(secretKeyBytes);
            byte[] hashValue = hmacsha512.ComputeHash(messageBytes);
            string hmac = "";
            foreach (byte x in hashValue)
            {
                hmac += $"{x:x2}";
            }
            return hmac.ToUpper();
        }
        public static string PassowrdCreateSalt512()
        {
            var message = PassowrdRandomString(512, false);
            return BitConverter.ToString((new SHA512Managed()).ComputeHash(Encoding.ASCII.GetBytes(message))).Replace("-", "");
        }
        public static string PassowrdRandomString(int size, bool lowerCase)
        {
            var builder = new StringBuilder();
            var random = new Random();
            for (int i = 0; i < size; i++)
            {
                char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            return lowerCase ? builder.ToString().ToLower() : builder.ToString();
        }
        public static T MappingTableEntityProperties<T>(T source, T destination)
        {
            foreach (var property in destination.GetType().GetProperties())
            {
                var val = source.GetType().GetProperty(property.Name)?.GetValue(source, null);
                switch (property.Name)
                {
                    case Created:
                        property.SetValue(destination, val);
                        break;
                    case CreatedBy:
                        property.SetValue(destination, val);

                        break;
                    case LastModifiedBy:
                        property.SetValue(destination, val);
                        break;
                    case LastModified:
                        property.SetValue(destination, val);
                        break;
                    case SiteId:
                        property.SetValue(destination, val);
                        break;
                    default:
                        break;
                }
            }
            return destination;
        }
        public static string ColumnAdress(int col)
        {
            if (col <= 26)
            {
                return Convert.ToChar(col + 64).ToString();
            }
            int div = col / 26;
            int mod = col % 26;
            if (mod == 0) { mod = 26; div--; }
            return ColumnAdress(div) + ColumnAdress(mod);
        }

        public static int ColumnNumber(string colAdress)
        {
            int[] digits = new int[colAdress.Length];
            for (int i = 0; i < colAdress.Length; ++i)
            {
                digits[i] = Convert.ToInt32(colAdress[i]) - 64;
            }
            int mul = 1; int res = 0;
            for (int pos = digits.Length - 1; pos >= 0; --pos)
            {
                res += digits[pos] * mul;
                mul *= 26;
            }
            return res;
        }
        /// <summary>
        /// Convert ColumnName => column index
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        static int ExcelColumnNameToNumber(string columnName)
        {
            if (string.IsNullOrEmpty(columnName)) throw new ArgumentNullException("columnName");
            columnName = columnName.ToUpperInvariant();
            int sum = 0;
            for (int i = 0; i < columnName.Length; i++)
            {
                sum *= 26;
                sum += (columnName[i] - 'A' + 1);
            }
            return sum;
        }
        public static string ColumnIndexToColumnLetter(int colIndex)
        {
            int div = colIndex;
            string colLetter = String.Empty;
            int mod = 0;

            while (div > 0)
            {
                mod = (div - 1) % 26;
                colLetter = (char)(65 + mod) + colLetter;
                div = (int)((div - mod) / 26);
            }
            return colLetter;
        }
        public static string ChuyenDoiChuoiTimKiem(string input)
        {
            StringBuilder builder = new StringBuilder();
            if (string.IsNullOrEmpty(input)) return string.Empty;
            var listTu = input.Split();  //Lấy danh sách từ của chuỗi nhập vào
            foreach (var tu in listTu)
            {
                string tuLower = tu.ToLower().Trim();                                   //Chuyển về chữ thường
                string tuLowerKhongDau = tuLower.Normalize(NormalizationForm.FormD);    //Chuyển đổi thành không dấu
                if (!KiemTraTuCoDau(tuLower)) //Nếu không có dấu thì mới thực hiện chuyển đổi.Nếu có dấu thì giữ nguyên từ
                {
                    int index = 0;
                    foreach (var chTuLowerKhongDau in tuLowerKhongDau)
                    {
                        if (CharUnicodeInfo.GetUnicodeCategory(chTuLowerKhongDau) != UnicodeCategory.NonSpacingMark)
                        {
                            //Chỗ này Replace thêm phát nữa để xử lý trường hợp: nhập là "ê, â, ă, ư, ơ,..." thì không tìm kiếm các từ "e a, u, o".
                            //VD: nhập từ "hiên" thì không hiển thị từ "hien" mà chỉ hiển thị từ "hiên" và "hiện, hiển, hiến"
                            string kyTuConvert = ReplaceCharSet(chTuLowerKhongDau.ToString()).Replace(chTuLowerKhongDau, tuLower[index]);
                            //string kyTuConvert = ReplaceCharSet(kyTu);
                            builder.Append(kyTuConvert);
                            index++;
                        }
                    }
                    builder.Append(" ");
                }
                else
                {
                    builder.Append(tuLower + " ");
                }
            }
            return EscapeWildcards(builder.ToString().Trim());
        }

        public static readonly string nguyenAmCoDau = "áàạảãấầậẩẫắằặẳẵéèẹẻẽếềệểễóòọỏõốồộổỗớờợởỡúùụủũứừựửữíìịỉĩýỳỵỷỹ";

        public static T ConvertData<T>(T model)
        {
            var temp = typeof(T);
            foreach (var property in temp.GetProperties())
            {
                var val = temp.GetProperty(property.Name)?.GetValue(model, null);
                var type = property.PropertyType;
                if (type == typeof(double))
                {
                    if (property.CanWrite)
                    {
                        property.SetValue(model, Math.Round(Convert.ToDouble(val), ROUND_RANGE));
                    }
                }
            }
            return model;
        }

        public static bool KiemTraTuCoDau(string input)
        {
            bool result = false;
            var listChar = input.ToCharArray();
            foreach (var kyTu in listChar)
            {
                if (nguyenAmCoDau.Contains(kyTu.ToString()))
                {
                    return true;
                }
            }
            return result;
        }

        private static string ReplaceCharSet(string input)
        {
            switch (input)
            {
                case "a":
                    return "[a\x00e0ả\x00e3\x00e1ạăằẳẵắặ\x00e2ầẩẫấậ]";

                case "e":
                    return "[e\x00e8ẻẽ\x00e9ẹ\x00eaềểễếệ]";

                case "i":
                    return "[i\x00ecỉĩ\x00edị]";

                case "o":
                    return "[o\x00f2ỏ\x00f5\x00f3ọ\x00f4ồổỗốộơờởỡớợ]";

                case "u":
                    return "[u\x00f9ủũ\x00faụưừửữứự]";

                case "y":
                    return "[yỳỷỹ\x00fdỵ]";

                case "d":
                    return "[dđ]";
                    //case "đ":
                    //    return "[dđ]";
            }
            return input;
        }

        private static string EscapeWildcards(string input)
        {
            return input.Trim().Replace("%", "[%]").Replace("_", "[_]");
        }

    }
    public static class Extention
    {
        public static IDictionary<string, string> ConvertJsonToDictionary(string jsonData)
        {
            if (jsonData != null)
            {
                var data = Newtonsoft.Json.JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);
                return data;
            }
            else return null;
        }
        public static IEnumerable<T> getMoreThanOnceRepeated<T>(this IEnumerable<T> extList, Func<T, object> groupProps) where T : class
        { //Return only the second and next reptition
            return extList
                .GroupBy(groupProps)
                .SelectMany(z => z.Skip(1)); //Skip the first occur and return all the others that repeats
        }
        public static IEnumerable<T> getAllRepeated<T>(this IEnumerable<T> extList, Func<T, object> groupProps) where T : class
        {
            //Get All the lines that has repeating
            return extList
                .GroupBy(groupProps)
                .Where(z => z.Count() > 1) //Filter only the distinct one
                .SelectMany(z => z);//All in where has to be retuned
        }
        [DllImport("urlmon.dll", CharSet = CharSet.Unicode, ExactSpelling = true, SetLastError = false)]
        static extern int FindMimeFromData(IntPtr pBC,
           [MarshalAs(UnmanagedType.LPWStr)] string pwzUrl,
           [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1, SizeParamIndex = 3)] byte[] pBuffer,
           int cbSize, [MarshalAs(UnmanagedType.LPWStr)] string pwzMimeProposed,
           int dwMimeFlags, out IntPtr ppwzMimeOut, int dwReserved);

        public static string GetMimeFromFile(string file)
        {
            IntPtr mimeout;
            if (!System.IO.File.Exists(file))
                throw new FileNotFoundException(file + " not found");

            int MaxContent = (int)new FileInfo(file).Length;
            if (MaxContent > 4096) MaxContent = 4096;
            FileStream fs = File.OpenRead(file);
            byte[] buf = new byte[MaxContent];
            fs.Read(buf, 0, MaxContent);
            fs.Close();
            int result = FindMimeFromData(IntPtr.Zero, file, buf, MaxContent, null, 0, out mimeout, 0);

            if (result != 0)
                throw Marshal.GetExceptionForHR(result);
            string mime = Marshal.PtrToStringUni(mimeout);
            Marshal.FreeCoTaskMem(mimeout);
            return mime;
        }
    }
}

