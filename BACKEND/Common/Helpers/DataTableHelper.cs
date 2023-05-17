using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;

namespace Common.Helpers
{
    public static class DataTableHelper
    {
        /// <summary>
        /// Convert DataTable to List T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        /// <summary>
        /// Get Item From DataRow 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dr"></param>
        /// <returns></returns>
        public static T GetItem<T>(DataRow dr)
        {
            var warningMessage = string.Empty;
            var errorMessage = string.Empty;
            var listWarningMessage = new List<string>();
            var listErrorMessage = new List<string>();

            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();
            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name.ToLower() == "isdataimported")
                    {
                        pro.SetValue(obj, Convert.ChangeType(true, typeof(bool)), null);
                        continue;
                    }
                    if (pro.Name.ToLower() == column.ColumnName.ToLower())
                    {
                        try
                        {
                            Type type = pro.PropertyType;
                            if (dr[column.ColumnName] != DBNull.Value && !string.IsNullOrEmpty(Convert.ToString(dr[column.ColumnName]).StandardizedText()))
                            {
                                if (type == typeof(DateTime) || type == typeof(DateTime?))
                                {
                                    DateTime dateOut;
                                    if (DateTime.TryParseExact(dr[column.ColumnName].ToString().StandardizedText(), "dd/MM/yyyy",
                                        CultureInfo.InvariantCulture,
                                        DateTimeStyles.None, out dateOut))
                                    {
                                        pro.SetValue(obj, dateOut);
                                    }
                                    else
                                    {
                                        var att = pro.GetCustomAttribute(typeof(ImportAttribute), true) as ImportAttribute;
                                        if (att != null)
                                        {
                                            listWarningMessage.Add($"{att.Name} không hợp lệ");
                                        }
                                        pro.SetValue(obj, dr[column.ColumnName].ToString().StandardizedText());
                                    }

                                }
                                else if (type == typeof(long)) //Chuyển sang số nguyên dương
                                {

                                    try
                                    {
                                        var numOfLongType = Convert.ChangeType(dr[column.ColumnName], type);
                                        //var numOfuongType = Convert.ToUInt64(numOfLongType);
                                        ulong num;
                                        if (ulong.TryParse(numOfLongType.ToString().StandardizedText(), out num))
                                        {
                                            pro.SetValue(obj, Convert.ChangeType(numOfLongType, type), null);
                                        }
                                        else
                                        {
                                            var att = pro.GetCustomAttribute(typeof(ImportAttribute), true) as ImportAttribute;
                                            if (att != null)
                                            {
                                                listWarningMessage.Add($"{att.Name} không hợp lệ");
                                            }
                                            pro.SetValue(obj, dr[column.ColumnName].ToString().StandardizedText());
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        var att = pro.GetCustomAttribute(typeof(ImportAttribute), true) as ImportAttribute;
                                        if (att != null)
                                        {
                                            listWarningMessage.Add($"{att.Name} không hợp lệ");
                                        }
                                        pro.SetValue(obj, dr[column.ColumnName].ToString().StandardizedText());
                                    }

                                }
                                else
                                {
                                    pro.SetValue(obj, Convert.ChangeType(dr[column.ColumnName], type), null);
                                }
                            }
                            else pro.SetValue(obj, dr[column.ColumnName].ToString().StandardizedText());
                        }
                        catch (Exception ex)
                        {
                            //Don't implimention
                        }
                    }
                    if (pro.Name.ToLower() == "warningmessage")
                    {
                        warningMessage = string.Join(',', listWarningMessage);
                        pro.SetValue(obj, warningMessage, null);
                        continue;
                    }
                    if (pro.Name.ToLower() == "errormessage")
                    {
                        pro.SetValue(obj, errorMessage, null);
                        continue;
                    }
                    else
                        continue;
                }
            }
            return obj;
        }

    }
}
