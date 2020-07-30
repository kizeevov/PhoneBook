using System.Collections.Generic;
using System.Linq;

namespace PhoneBook.Models
{
    public class Contact
    {
        public string Id { get; set; }

        public string FirstName { get; set; }

        public List<Phone> Phones { get; }

        public string PhonesString => string.Join(", ", Phones.Select(x => x.PhoneNumber));

        public Contact()
        {
            Phones = new List<Phone>();
        }
    }
}
