using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PhoneBook.Views
{
    public class PhoneView : UserControl
    {
        public PhoneView()
        {
            this.InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
