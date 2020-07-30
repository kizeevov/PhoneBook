using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PhoneBook.Views
{
    public class PhoneListView : UserControl
    {
        public PhoneListView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
