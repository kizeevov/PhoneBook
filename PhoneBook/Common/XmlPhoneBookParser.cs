using PhoneBook.Models;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace PhoneBook.Common
{
    public class XmlPhoneBookParser : IXmlPhoneBookParser
    {
        private const string CONTACT_TAG = "Contact";
        private const string PHONE_TAG = "Phone";
        private const string ID_TAG = "id";
        private const string FIRSTNAME_TAG = "FirstName";
        private const string PHONENUMBER_TAG = "phonenumber";

        public IEnumerable<Contact> Parse(string xml)
        {
            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreWhitespace = true;

            List<Contact> contacts = new List<Contact>();

            using (TextReader sr = new StringReader(xml))
            using (XmlReader reader = XmlReader.Create(sr, settings))
            {
                while (reader.Read())
                {
                    switch (reader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (reader.Name == CONTACT_TAG)
                            {
                                Contact contact = ParseContact(reader);
                                contacts.Add(contact);
                            }
                            break;
                        case XmlNodeType.Text:
                            break;
                        case XmlNodeType.EndElement:
                            break;
                        default:
                            break;
                    }
                }
            }

            return contacts;
        }

        private Contact ParseContact(XmlReader reader)
        {
            Contact contact = new Contact();

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == ID_TAG)
                    {
                        contact.Id = ReadTagValue(reader);
                    }
                    else if (reader.Name == FIRSTNAME_TAG)
                    {
                        contact.FirstName = ReadTagValue(reader);
                    }
                    else if (reader.Name == PHONE_TAG)
                    {
                        Phone phone = ParsePhone(reader);
                        contact.Phones.Add(phone);
                    }
                }

                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.Name == CONTACT_TAG)
                    {
                        return contact;
                    }
                }
            }

            return null;
        }

        private Phone ParsePhone(XmlReader reader)
        {
            Phone phone = new Phone();

            string type = reader.GetAttribute("type");
            switch (type)
            {
                case "Home":
                    phone.PhoneType = PhoneType.Home;
                    break;
                case "Work":
                    phone.PhoneType = PhoneType.Work;
                    break;
                case "Mobile":
                    phone.PhoneType = PhoneType.Mobile;
                    break;
            }

            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element)
                {
                    if (reader.Name == PHONENUMBER_TAG)
                    {
                        phone.PhoneNumber = ReadTagValue(reader);
                    }
                }
                
                if (reader.NodeType == XmlNodeType.EndElement)
                {
                    if (reader.Name == PHONE_TAG)
                    {
                        return phone;
                    }
                }
            }

            return null;
        }

        private string ReadTagValue(XmlReader reader)
        {
            if (!reader.Read() && reader.NodeType != XmlNodeType.Text)
            {
                return "";
            }

            return reader.Value;
        }
    }
}
