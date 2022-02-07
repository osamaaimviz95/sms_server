using System;
using System.Collections;
using System.Text;
using System.Xml;
using System.IO;

//classes on this file
//XMLObject - Top class
//XMLList - represent a hierarchy of lists
//XMLRecord - represent a hierarchy of records

namespace Common.XML
{
    //class XMLObject
    public class XMLObject
    {
        protected XmlNode root;

        public XMLObject(string xml, string rootPath)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            root = doc.DocumentElement.SelectSingleNode(rootPath);
        }

        public XMLObject(XmlReader reader)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(reader);
            root = doc.DocumentElement;
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

        public XmlDocument Document
        {
            get
            {
                return root.OwnerDocument;
            }
        }

        public string GetAttribute(string name)
        {
            return root.Attributes[name].Value;
        }

        public virtual XMLObject GetObject(string path)
        {
            return new XMLObject(root.SelectSingleNode(path));
        }

        public static void Save(string data, string path)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(data);
            doc.Save(path);
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
            finally
            {
                reader.Close();
            }
        }

        public static XMLObject LoadObject(string path)
        {
            XmlTextReader reader = null;
            try
            {
                reader = new XmlTextReader(new FileStream(path, FileMode.Open, FileAccess.Read));
                return new XMLObject(reader);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                reader.Close();
            }
        }

        public static XMLList LoadList(string path)
        {
            XmlTextReader reader = null;
            try
            {
                reader = new XmlTextReader(new FileStream(path, FileMode.Open, FileAccess.Read));
                return new XMLList(reader);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                reader.Close();
            }
        }

        public static XMLRecord LoadRecord(string path)
        {
            XmlTextReader reader = null;
            try
            {
                reader = new XmlTextReader(new FileStream(path, FileMode.Open, FileAccess.Read));
                return new XMLRecord(reader);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                reader.Close();
            }
        }

        public void Save(string path)
        {

            root.OwnerDocument.Save(path);
        }
    }

    //class XMLRecord
    public class XMLRecord : XMLObject, IEnumerable 
    {
        public XMLRecord(string xml)
            : base(xml)
        {
        }

        public XMLRecord(XmlReader reader)
            : base(reader)
        {
        }

        public XMLRecord(XmlNode root)
            : base(root)
        {
        }

        public XMLRecord GetRecord(string path)
        {
            try
            {
                return new XMLRecord(root.SelectSingleNode(path));
            }
            catch
            {
                return null;
            }
        }

        public string this[string path]
        {
            get
            {
                try
                {
                    return root.SelectSingleNode(path).InnerText;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                try
                {
                    root.SelectSingleNode(path).InnerText = value;
                }
                catch
                {
                }
            }
        }

        public IEnumerator GetEnumerator()
        {
            return root.GetEnumerator();
        }
    }

    //class XMLList
    public class XMLList : XMLObject, IEnumerable
    {
        public XMLList(XmlReader reader)
            : base(reader)
        {
        }

        public XMLList(XmlNode node)
            : base(node)
        {
        }

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

        public XmlNode GetNode(string idx)
        {
            return root.SelectSingleNode("item[@id='" + idx + "']");
        }

        public override XMLObject GetObject(string idx)
        {
            return base.GetObject("item[@id='" + idx + "']");
        }

        protected XmlNode AddNode(string id, string value)
        {
            XmlNode node = Document.CreateNode(XmlNodeType.Element, "item", "");
            root.InsertAfter(node, root.LastChild);
            XmlAttribute attr = node.Attributes.Append(Document.CreateAttribute("id"));
            attr.Value = id;

            return node;
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
            set
            {
                try
                {
                    XmlNode node = root.SelectSingleNode("item[@id='" + idx + "']");

                    if (node == null && value != null)
                        node = AddNode(idx, value);

                    if (value == null)
                        root.RemoveChild(node);
                    else
                        node.InnerText = value;
                }
                catch
                {
                }

            }
        }

        public IEnumerator GetEnumerator()
        {
            return root.GetEnumerator();
        }
    }
}

