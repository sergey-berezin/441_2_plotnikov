using System.Windows;

namespace WPF_App {
    public partial class TextInputDialog : Window {
        public string InputText { get; private set; }

        public TextInputDialog() {
            InitializeComponent();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e) {
            InputText = InputTextBox.Text;
            DialogResult = true;
            Close();
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e) {
            DialogResult = false;
            Close();
        }
    }
}
