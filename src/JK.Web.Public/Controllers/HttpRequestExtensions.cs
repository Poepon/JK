using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace JK.Web.Public.Controllers
{
    public static class HttpRequestExtensions
    {
        public static string GetBodyContent(this HttpRequest httpRequest)
        {
            using (var buffer = new MemoryStream())
            {
                httpRequest.Body.CopyTo(buffer);
                buffer.Position = 0;
                var bytes = buffer.ToArray();
                return Encoding.UTF8.GetString(bytes);
            }
        }

        public static string ToXmlString(this Dictionary<string, string> m_values)
        {
            //数据为空时不能转化为xml格式
            if (0 == m_values.Count)
            {
                throw new Exception("WxPayData数据为空!");
            }

            string xml = "<xml>";
            foreach (KeyValuePair<string, string> pair in m_values)
            {
                //字段值不能为null，会影响后续流程
                if (pair.Value == null)
                {
                    throw new Exception("WxPayData内部含有值为null的字段!");
                }

                if (pair.Value.GetType() == typeof(int))
                {
                    xml += "<" + pair.Key + ">" + pair.Value + "</" + pair.Key + ">";
                }
                else if (pair.Value.GetType() == typeof(string))
                {
                    xml += "<" + pair.Key + ">" + "<![CDATA[" + pair.Value + "]]></" + pair.Key + ">";
                }
                else//除了string和int类型不能含有其他数据类型
                {
                    throw new Exception("WxPayData字段数据类型错误!");
                }
            }
            xml += "</xml>";
            return xml;
        }

    }
}
