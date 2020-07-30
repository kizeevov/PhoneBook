using System;
using System.Timers;
using System.Threading.Tasks;
using System.Collections.Generic;
using PhoneBook.Models;
using PhoneBook.Common;
using System.Linq;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace PhoneBook.Network
{
    public class PhoneBookService : IPhoneBookService
    {
        private readonly GetFileService _fileService;
        private readonly IXmlPhoneBookParser _xmlPhoneBookParser;
        private readonly IConfiguration _configuration;
        private readonly Timer _timer;
        private List<Contact> _contacts;

        private readonly string _xmlFilePath;

        public event EventHandler<UpdateStatus> UpdateStatusChanged;

        public IReadOnlyCollection<Contact> Contacts => _contacts;

        public PhoneBookService(GetFileService fileService, IXmlPhoneBookParser xmlPhoneBookParser, IConfiguration configuration)
        {
            _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
            _xmlPhoneBookParser = xmlPhoneBookParser ?? throw new ArgumentNullException(nameof(xmlPhoneBookParser));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            _contacts = new List<Contact>();
            _xmlFilePath = Path.Combine(Directory.GetCurrentDirectory(), "phonebook.xml");

            _timer = new Timer(10000);
            _timer.Elapsed += async (sender, e) => await LoadXmlFromHttpAsync();

            LoadPhoneBook();
        }

        private async void LoadPhoneBook()
        {
            string xml = await TryReadXmlFromFileAsync();

            if (string.IsNullOrEmpty(xml))
            {
                await LoadXmlFromHttpAsync();
                return;
            }

            _contacts = _xmlPhoneBookParser.Parse(xml).ToList();
            UpdateStatusChanged?.Invoke(this, UpdateStatus.Success);

            _timer.Start();
        }

        private async Task LoadXmlFromHttpAsync()
        {
            _timer.Stop();

            UpdateStatusChanged?.Invoke(this, UpdateStatus.InProgress);

            string url = _configuration["host"];

            //string url = "https://raw.githubusercontent.com/kizeevov/PhoneBook/master/PhoneBook/Assets/phonebook.xml";
            string xml = await _fileService.TryGetXmlFileAsync(url).ConfigureAwait(false);
            string xmlFile = await TryReadXmlFromFileAsync();

            if (!string.IsNullOrEmpty(xml))
            {
                if (xml == xmlFile)
                {
                    UpdateStatusChanged?.Invoke(this, UpdateStatus.Actual);
                    return;
                }

                _contacts = _xmlPhoneBookParser.Parse(xml).ToList();
                UpdateStatusChanged?.Invoke(this, UpdateStatus.Success);
                await TryWriteXmlToFileAsync(xml);
            }
            else
            {
                UpdateStatusChanged?.Invoke(this, UpdateStatus.Error);
            }

            _timer.Start();
        }

        private async Task<string> TryReadXmlFromFileAsync()
        {
            try
            {
                return await File.ReadAllTextAsync(_xmlFilePath);
            }
            catch { }

            return string.Empty;
        }

        private async Task TryWriteXmlToFileAsync(string xml)
        {
            try
            {
                await File.WriteAllTextAsync(_xmlFilePath, xml);
            }
            catch { }
        }
    }
}
