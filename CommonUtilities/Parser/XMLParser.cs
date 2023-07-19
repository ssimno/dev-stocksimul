using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;


namespace CommonUtilities.Parser
{
    public static class XMLParser
    {
        public const string FolderName = "Parser";
        public const string XMLFileName = "XMLStrings.xml";

        public static string FolderPath = System.IO.Path.Combine(Environment.CurrentDirectory.ToString(), $"{FolderName}");
        public static string DirPath = System.IO.Path.Combine(Environment.CurrentDirectory.ToString(), $"{FolderName}\\{XMLFileName}");

        public const string TYPE_INITVIEW = "InitView";
        public const string TYPE_MENUENABLE = "MenuEnable";
        public const string TYPE_SHAREDFOLDER = "SharedFolder";

        public enum XMLType
        {
            Define,
            ViewList,
            ConnectionString,
        }

        private static XmlNodeList Read()
        {
            XmlDocument xml = new XmlDocument();
            string path = DirPath;
            xml.Load(path);
            XmlNodeList xmlList = xml.SelectNodes("/config");
            return xmlList;
        }

        public static string GetData(params string[] _datas)
        {
            string returnValue = "";
            try
            {
                XmlNodeList xmlList = Read();
                XmlNodeList savedNode = null;
                foreach (string data in _datas)
                {
                    XmlNodeList node = savedNode ?? xmlList;
                    foreach (XmlNode xnl in node)
                    {
                        savedNode = xnl.SelectNodes(data);
                    }
                }
                returnValue = savedNode[0]?.InnerText;
            }
            catch (Exception _e)
            {
                Console.WriteLine("error = " + _e.Message);
            }

            return returnValue;
        }

        public static string GetData(XMLType type, params string[] datas)
        {
            string returnValue = "";
            try
            {
                XmlNodeList xmlList = Read()[0].SelectNodes(type.ToString());
                XmlNodeList savedNode = null;
                foreach (string data in datas)
                {
                    XmlNodeList node = savedNode ?? xmlList;
                    foreach (XmlNode xnl in node)
                    {
                        savedNode = xnl.SelectNodes(data);
                    }
                }
                returnValue = savedNode[0]?.InnerText;
            }
            catch (Exception _e)
            {
                Console.WriteLine("error = " + _e.Message);
            }

            return returnValue;
        }

        public static string GetData(XMLType type, string data, string defaultData)
        {
            string returnValue = "";
            try
            {
                XmlNodeList xmlList = Read()[0].SelectNodes(type.ToString());
                XmlNodeList savedNode = null;
                XmlNodeList node = savedNode ?? xmlList;
                foreach (XmlNode xnl in node)
                {
                    savedNode = xnl.SelectNodes(data);
                }
                returnValue = savedNode[0]?.InnerText;
            }
            catch (Exception _e)
            {
                Console.WriteLine("error = " + _e.Message);
            }

            return string.IsNullOrEmpty(returnValue) ? defaultData : returnValue;
        }

        public static XmlNodeList GetDataList(params string[] _datas)
        {
            XmlNodeList returnValue = null;
            try
            {
                XmlNodeList xmlList = Read();
                XmlNodeList savedNode = null;
                foreach (string data in _datas)
                {
                    XmlNodeList node = savedNode ?? xmlList;
                    foreach (XmlNode xnl in node)
                    {
                        savedNode = xnl.SelectNodes(data);
                    }
                }
                returnValue = savedNode;
            }
            catch (Exception _e)
            {
                Console.WriteLine("error = " + _e.Message);
            }

            return returnValue;
        }
    }
}
