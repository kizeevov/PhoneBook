using System;
using System.Collections.Generic;
using PhoneBook.Common;
using PhoneBook.Models;

namespace PhoneBook.Network
{
    public interface IPhoneBookService
    {
        event EventHandler<UpdateStatus> UpdateStatusChanged;

        IReadOnlyCollection<Contact> Contacts { get; }
    }
}
