using PhoneBook.Models;
using System.Collections.Generic;

namespace PhoneBook.Common
{
    public interface IXmlPhoneBookParser
    {
        IEnumerable<Contact> Parse(string xml);
    }
}
