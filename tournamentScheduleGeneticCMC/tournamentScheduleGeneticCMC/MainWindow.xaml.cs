using Genetic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WPF_App {
    public partial class MainWindow : Window {
        public ViewData? Parameters { get; set; }
        public bool CancelFlag {  get; set; }
        public MainWindow() {
            InitializeComponent();

            Parameters = new ViewData(0, 0, 0);
            DataContext = Parameters;
        }

        private async void StartBtn(object sender, RoutedEventArgs e) {
            RunButton.IsEnabled = false;

            BindingExpression NumOfPlayersField = BindingOperations.GetBindingExpression(NumOfPlayers, TextBox.TextProperty);
            NumOfPlayersField.UpdateSource();
            BindingExpression NumOfToursField = BindingOperations.GetBindingExpression(NumOfTours, TextBox.TextProperty);
            NumOfToursField.UpdateSource();
            BindingExpression NumOfPlaygroundsField = BindingOperations.GetBindingExpression(NumOfPlaygrounds, TextBox.TextProperty);
            NumOfPlaygroundsField.UpdateSource();

            SpeciesParams speciesParams = new(
                int.Parse(NumOfPlayers.Text),
                int.Parse(NumOfTours.Text),
                int.Parse(NumOfPlaygrounds.Text)
            );

            Population population = new(speciesParams);

            int generation = 1;
            CancelFlag = false;

            while (generation <= Constants.NumOfSelectionRound && !CancelFlag) {
                var selectedPopulation = NaturalSelection.SelectionPopulation(population);
                var crossoverPopulation = NaturalSelection.CrossoverPopulation(selectedPopulation);
                var mutatedPopulation = NaturalSelection.MutationPopulation(crossoverPopulation);

                population = mutatedPopulation;

                await Task.Delay(100).ContinueWith(_ => {
                    Application.Current.Dispatcher.Invoke(() => {
                        TournamentTable.ItemsSource = ConvertBestSpeciesToDataTable(population).DefaultView;

                        AvgMinNumOfPlaygrounds.Text = population.AvgMinNumOfPlaygrounds.ToString();
                        AvgMinNumOfOpponents.Text = population.AvgMinNumOfOpponents.ToString();

                        BestMinNumOfPlaygrounds.Text = population.BestMinNumOfPlaygrounds.ToString();
                        BestMinNumOfOpponents.Text = population.BestMinNumOfOpponents.ToString();

                        Generation.Text = generation.ToString();
                    });
                });
                generation++;
            }
            RunButton.IsEnabled = true;
        }

        private void StopBtn(object sender, RoutedEventArgs e) {
            CancelFlag = true;
        }

        private DataTable ConvertBestSpeciesToDataTable(Population population) {
            DataTable dt = new DataTable();

            var bestSpecies = population.BestSpecies;
            if (bestSpecies == null || bestSpecies.Gens == null || bestSpecies.Gens.Length == 0) {
                return dt; // Возвращаем пустую таблицу, если данных нет
            }

            // Определяем максимальное количество столбцов
            int maxColumns = 0;
            foreach (var gens in bestSpecies.Gens) {
                maxColumns = Math.Max(maxColumns, gens.Count);
            }

            // Создаем столбцы
            for (int j = 0; j < maxColumns; j++)
                dt.Columns.Add($"Column{j + 1}", typeof(string));

            // Заполняем строки
            foreach (var gens in bestSpecies.Gens) {
                DataRow dr = dt.NewRow();
                for (int j = 0; j < gens.Count; j++) {
                    dr[j] = gens[j].ToString();
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }
    }

    public class ViewData {
        public int NumOfPlayers { get; set; }
        public int NumOfTours { get; set; }
        public int NumOfPlaygrounds { get; set; }

        public ViewData(int NumOfPlayers, int NumOfTours, int NumOfPlaygrounds) {
            this.NumOfPlayers = NumOfPlayers;
            this.NumOfTours = NumOfTours;
            this.NumOfPlaygrounds = NumOfPlaygrounds;
        }
    }
}
