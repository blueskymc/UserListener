using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace UserListener
{
    public class XmlHelper
    {
        /// <summary>
        /// 序列化
        ///
        /// 当需要将多个对象实例序列化到同一个XML文件中的时候,xmlRootName就是所有对象共同的根节点名称,
        /// 如果不指定,.net会默认给一个名称(ArrayOf+实体类名称)
        /// </summary>
        /// <param name="srcObject">对象的实例</param>
        /// <param name="type">对象类型</param>
        /// <param name="xmlFilePath">序列化之后的xml文件的绝对路径</param>
        /// <param name="xmlRootName">xml文件中根节点名称</param>
        public static void SerializeToXml(object srcObject, Type type, string xmlFilePath, string xmlRootName)
        {
            if (srcObject != null && !string.IsNullOrEmpty(xmlFilePath))
            {
                type = type != null ? type : srcObject.GetType();

                using (StreamWriter sw = new StreamWriter(xmlFilePath, false))
                {
                    try
                    {
                        XmlSerializer xs = string.IsNullOrEmpty(xmlRootName) ?
                            new XmlSerializer(type) :
                            new XmlSerializer(type, new XmlRootAttribute(xmlRootName));
                        xs.Serialize(sw, srcObject);
                    }
                    catch (Exception ex)
                    {
                        ;
                    }
                }
            }
        }

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="xmlFilePath">需要反序列化的XML文件的绝对路径</param>
        /// <param name="type">反序列化XML为哪种对象类型</param>
        /// <returns></returns>
        public static object DeserializeFromXml(string xmlFilePath, Type type)
        {
            object result = null;
            if (File.Exists(xmlFilePath))
            {
                using (StreamReader reader = new StreamReader(xmlFilePath))
                {
                    XmlSerializer xs = new XmlSerializer(type);
                    result = xs.Deserialize(reader);
                }
            }
            return result;
        }
    }
}