using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Collections;
using System.Reflection;
using System.Data;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using fastJSON;

namespace fastJSON.IO
{
    /// <summary>
    /// 序列化器
    /// </summary>
    public static class Serializer
    {
        /// <summary>
        /// 以二进制的方式保存
        /// </summary>
        /// <typeparam Name="T"></typeparam>
        /// <param Name="data"></param>
        /// <param Name="path"></param>
        /// <returns></returns>
        public static bool SaveWithBinary<T>(T data, string path)
        {
            bool f = false;
            try
            {
                FileStream fs = new FileStream(path, FileMode.Create);
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, data);
                f = true;
            }
            catch (Exception)
            {
            }
            return f;
        }

        /// <summary>
        /// 从二进文件中加载
        /// </summary>
        /// <typeparam Name="T"></typeparam>
        /// <param Name="path"></param>
        /// <returns></returns>
        public static T LoadFromBinary<T>(string path)
        {
            T data = default(T);
            try
            {
                FileStream fs = null;
                if (!File.Exists(path)) return default(T);
                fs = new FileStream(path, FileMode.Open);
                BinaryFormatter bf = new BinaryFormatter();
                data = (T)bf.Deserialize(fs);
            }
            catch (Exception)
            {
            }
            return data;
        }


        /// <summary>
        /// 以Xml的方式保存
        /// </summary>
        /// <typeparam Name="T"></typeparam>
        /// <param Name="data"></param>
        /// <param Name="path"></param>
        /// <returns></returns>
        public static bool SaveWithXml<T>(T data, string path)
        {
            bool f = false;

            XmlWriter xmlWriter = null;
            try
            {
                FileStream fs = new FileStream(path, FileMode.Create);

                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                xmlWriter = new XmlTextWriter(fs, Encoding.UTF8);
                //序列化
                xmlSerializer.Serialize(xmlWriter, data);
            }
            catch (Exception)
            {
                f = false;
            }
            finally
            {
                if (xmlWriter != null)
                {
                    xmlWriter.Close();
                }
            }
            return f;
        }


        /// <summary>
        /// 从Xml文件中加载
        /// </summary>
        /// <typeparam Name="T"></typeparam>
        /// <param Name="path"></param>
        /// <returns></returns>
        public static T LoadFromXml<T>(string path)
        {
            T data = default(T);

            try
            {
                if (!File.Exists(path)) return default(T);
                FileStream fs = new FileStream(path, FileMode.Open);
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
                //序列化
                data = (T)xmlSerializer.Deserialize(fs);
            }
            catch (Exception)
            {
            }
            return data;
        }

        /// <summary>
        /// 将集合转换为字节数给
        /// </summary>
        /// <typeparam Name="T"></typeparam>
        /// <param Name="data"></param>
        /// <param Name="memberType"></param>
        /// <returns></returns>
        public static byte[] ToArray<T>(T data, Type memberType) where T : IEnumerable
        {
            byte[] buffer = null;
            try
            {
                MemoryStream ms = new MemoryStream();
                DataContractSerializer dcs = new DataContractSerializer(typeof(IList<>).MakeGenericType(memberType));
                dcs.WriteObject(ms, data);
                ms.Position = 0;
                buffer = ms.ToArray();
            }
            catch (Exception)
            {
            }
            return buffer;
        }

        /// <summary>
        /// 将实体转换为字节数给
        /// </summary>
        /// <typeparam Name="T"></typeparam>
        /// <param Name="data"></param>
        /// <param Name="memberType"></param>
        /// <returns></returns>
        public static byte[] ToArray<T>(T data) where T : class
        {
            byte[] buffer = null;
            try
            {
                MemoryStream ms = new MemoryStream();
                DataContractSerializer dcs = new DataContractSerializer(typeof(T));

                dcs.WriteObject(ms, data);
                ms.Position = 0;
                buffer = ms.ToArray();
            }
            catch (Exception)
            {
            }
            return buffer;
        }

        /// <summary>
        /// 将字节数组转换为指定类型集合
        /// </summary>
        /// <typeparam Name="T"></typeparam>
        /// <param Name="data"></param>
        /// <returns></returns>
        public static IList<T> ToEntitys<T>(byte[] data)
        {
            IList<T> result = null;
            try
            {
                MemoryStream ms = new MemoryStream(data);
                DataContractSerializer dcs = new DataContractSerializer(typeof(IList<>).MakeGenericType(typeof(T)));
                result = dcs.ReadObject(ms) as IList<T>;
            }
            catch (Exception)
            {
            }
            return result;
        }

        /// <summary>
        /// 将字节数据组转换为指定类型实体
        /// </summary>
        /// <typeparam Name="T"></typeparam>
        /// <param Name="data"></param>
        /// <returns></returns>
        public static T ToEntity<T>(byte[] data) where T : class
        {
            T result = default(T);

            try
            {
                MemoryStream ms = new MemoryStream(data);
                DataContractSerializer dcs = new DataContractSerializer(typeof(T));
                result = dcs.ReadObject(ms) as T;
            }
            catch (Exception)
            {
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param Name="table"></param>
        /// <returns></returns>
        public static string ToJsonByStringBuilder(this DataTable table)
        {
            var jsonString = new StringBuilder();
            if (table.Rows.Count > 0)
            {
                jsonString.Append("[");
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    jsonString.Append("{");
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < table.Columns.Count - 1)
                        {
                            jsonString.Append("\"" + table.Columns[j].ColumnName.ToString()
                         + "\":" + "\""
                         + table.Rows[i][j].ToString() + "\",");
                        }
                        else if (j == table.Columns.Count - 1)
                        {
                            jsonString.Append("\"" + table.Columns[j].ColumnName.ToString()
                         + "\":" + "\""
                         + table.Rows[i][j].ToString() + "\"");
                        }
                    }
                    if (i == table.Rows.Count - 1)
                    {
                        jsonString.Append("}");
                    }
                    else
                    {
                        jsonString.Append("},");
                    }
                }
                jsonString.Append("]");
            }
            return jsonString.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param Name="table"></param>
        /// <returns></returns>
        public static string ToJsonByJavaScriptSerializer(this DataTable table)
        {
            JavaScriptSerializer jsSerializer = new JavaScriptSerializer();
            List<Dictionary<string, object>> parentRow = new List<Dictionary<string, object>>();
            Dictionary<string, object> childRow;
            foreach (DataRow row in table.Rows)
            {
                childRow = new Dictionary<string, object>();
                foreach (DataColumn col in table.Columns)
                {
                    childRow.Add(col.ColumnName, row[col]);
                }
                parentRow.Add(childRow);
            }
            return jsSerializer.Serialize(parentRow);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param Name="obj"></param>
        /// <returns></returns>
        public static string ToJson(this Object obj)
        {
            if (obj == null) return "{}";
            if (obj is DataRowView)
            {
                DataRowView d = obj as DataRowView;
                int index = 0;
                Dictionary<string, string> _d = new Dictionary<string, string>();
                foreach (var item in d.Row.Table.Columns)
                {
                    _d[item.ToString()] = d[index].ToString();
                    index++;
                }
                obj = _d;
            }
            if (obj.GetType().FullName.Contains("AnonymousType") || obj is DataTable)
                return JsonConvert.SerializeObject(obj);
            else
                return JSON.ToJSON(obj);

        }


        static XmlDocument doc = new XmlDocument();
        public static string XmlToJson(this string xmlStr)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.Reset();
            settings.IgnoreComments = true;

            //byte[] array = Encoding.UTF8.GetBytes(xmlStr);
            //MemoryStream stream = new MemoryStream(array); 
            //stream.Position = 0; 
            TextReader tr = new StringReader(xmlStr);
            XmlReader reader = XmlReader.Create(tr, settings);
            doc.Load(reader);
            reader.Close();

            return JsonConvert.SerializeXmlNode(doc);
        }
    }
}
