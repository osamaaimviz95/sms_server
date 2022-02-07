using System;
using System.Collections;
using System.Text;
using System.Xml;
using System.IO;


namespace CommonComponent.XML
{
    public class XMLObject
    {
        protected XmlNode root;

        public XMLObject(string xml, string rootPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            root = doc.DocumentElement.SelectSingleNode(rootPath);
        }

        public XMLObject(XmlNode root)
        {
            this.root = root;
        }

        public XMLObject(FileStream stream, string rootPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(stream);
            root = doc.DocumentElement.SelectSingleNode(rootPath);
        }

        public XMLObject(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            root = doc.DocumentElement;
        }

        public XMLObject(FileStream stream)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(stream);
            root = doc.DocumentElement;
        }

        public string GetAttribute(string name)
        {
            return root.Attributes[name].Value;
        }

        public virtual XMLObject GetObject(string path)
        {
            return new XMLObject(root.SelectSingleNode(path));
        }


        public static string Load(string path)
        {
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read));

                StringBuilder line = new StringBuilder("");
                while (!reader.EndOfStream)
                    line.Append(reader.ReadLine());

                return line.ToString();
            }
            catch (Exception e)
            {
                reader.Close();
                throw e;
            }
        }

        public void Save(string path)
        {

            root.OwnerDocument.Save(path);
        }
    }

    public class XMLList : XMLObject, IEnumerable
    {
        public XMLList(string xml)
            : base(xml)
        {
        }

        public XMLList(string xml, string rootPath)
            : base(xml, rootPath)
        {
        }

        public string GetXML(string idx)
        {
            return root.SelectSingleNode("item[@id='" + idx + "']").InnerXml;
        }

        public override XMLObject GetObject(string idx)
        {
            return base.GetObject("item[@id='" + idx + "']");
        }

        public string this[string idx]
        {
            get
            {
                try
                {
                    return root.SelectSingleNode("item[@id='" + idx + "']").InnerText;
                }
                catch
                {
                    return "";
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            return root.GetEnumerator();
        }
    }
}

