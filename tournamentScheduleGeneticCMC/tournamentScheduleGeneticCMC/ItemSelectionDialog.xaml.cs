using System.Windows;

namespace WPF_App {
    public partial class ItemSelectionDialog : Window {
        public string SelectedItem { get; private set; }

        public ItemSelectionDialog(List<string> populationsId) {
            InitializeComponent();

            SavedPopulationBox.ItemsSource = populationsId;
        }

        public void SelectSavedPopulation(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (SavedPopulationBox.SelectedItem != null) {
                SelectedItem = (string)SavedPopulationBox.SelectedItem;
                DialogResult = true;
                Close();
            }
        }
    }
}
