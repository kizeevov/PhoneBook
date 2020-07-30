using Avalonia;
using Avalonia.Collections;
using PhoneBook.Models;
using PhoneBook.Network;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;

namespace PhoneBook.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly IPhoneBookService _phoneBookService;

        private string _updateStateText;
        private string _searchText;
        private List<Contact> _sourceContacts;

        public AvaloniaList<Contact> Contacts { get; private set; }

        public string UpdateStateText
        {
            get => _updateStateText;
            set => this.RaiseAndSetIfChanged(ref _updateStateText, value);
        }

        public string SearchText
        {
            get => _searchText;
            set => this.RaiseAndSetIfChanged(ref _searchText, value);
        }

        // For UI dezigner
        //public MainWindowViewModel() { }

        public MainWindowViewModel(IPhoneBookService phoneBookService)
        {
            _phoneBookService = phoneBookService ?? throw new ArgumentNullException(nameof(phoneBookService));

            _phoneBookService.UpdateStatusChanged += HandleUpdateStatusChanged;

            _sourceContacts = new List<Contact>();
            Contacts = new AvaloniaList<Contact>();

            this.WhenAnyValue(x => x.SearchText).Subscribe(x => ChangeText(x));
        }

        public async void CopyValue(string parameter)
        {
            await Application.Current.Clipboard.SetTextAsync(parameter);

            string text = UpdateStateText;
            UpdateStateText = $"Скопировано: {parameter}";
            await Task.Delay(5000);
            UpdateStateText = text;
        }

        public void ClearSearchText()
        {
            SearchText = string.Empty;
        }

        public async void PasteSearchText()
        {
            SearchText = await Application.Current.Clipboard.GetTextAsync();
        }

        private void ChangeText(string text)
        {
            Filter();
        }

        private void Filter()
        {
            var list = _sourceContacts.FindAll(x => string.IsNullOrWhiteSpace(SearchText) || ContainsString(x.FirstName, SearchText) ||
                ContainsString(string.Join("", x.Phones.Select(x => x.PhoneNumber)), SearchText));
            Contacts.Clear();
            Contacts.AddRange(list);
        }

        private bool ContainsString(string s1, string s2)
        {
            string prepareString1 = s1.Trim().Replace(" ", "");
            string prepareString2 = s2.Trim().Replace(" ", "");
            bool result = prepareString1.Contains(prepareString2, StringComparison.OrdinalIgnoreCase);

            return result;
        }

        private void HandleUpdateStatusChanged(object sender, Common.UpdateStatus e)
        {
            switch (e) {
                case Common.UpdateStatus.Actual:
                    UpdateStateText = $"Список актуален: {DateTime.Now}";
                    break;

                case Common.UpdateStatus.InProgress:
                    UpdateStateText = $"Обновление списка телефонов";  
                    break;
                
                case Common.UpdateStatus.Error:
                    UpdateStateText = $"При обновлении произошла ошибка";
                    break;

                case Common.UpdateStatus.Success:
                    UpdateStateText = $"Список обновлен: {DateTime.Now}";

                    Avalonia.Threading.Dispatcher.UIThread.Post(() => {

                        _sourceContacts.Clear();
                        _sourceContacts.AddRange(_phoneBookService.Contacts);
                        Filter();
                    });

                    break;
            }
        }
    }
}
