using Genetic;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace WPF_App {
    public partial class MainWindow : Window {
        public ViewData? Parameters { get; set; }
        public bool CancelFlag { get; set; }
        public bool ShouldInitData { get; set; }
        public SpeciesParams speciesParams { get; set; }
        public Population population { get; set; }
        public int generationNum { get; set; }
        public MainWindow() {
            InitializeComponent();

            BindingExpression NumOfPlayersField = BindingOperations.GetBindingExpression(NumOfPlayers, TextBox.TextProperty);
            NumOfPlayersField.UpdateSource();
            BindingExpression NumOfToursField = BindingOperations.GetBindingExpression(NumOfTours, TextBox.TextProperty);
            NumOfToursField.UpdateSource();
            BindingExpression NumOfPlaygroundsField = BindingOperations.GetBindingExpression(NumOfPlaygrounds, TextBox.TextProperty);
            NumOfPlaygroundsField.UpdateSource();

            Parameters = new ViewData(0, 0, 0);
            DataContext = Parameters;

            ShouldInitData = true;

            StopButton.IsEnabled = false;
            SaveButton.IsEnabled = false;
        }

        private void initData() {
            speciesParams = new SpeciesParams(
                int.Parse(NumOfPlayers.Text),
                int.Parse(NumOfTours.Text),
                int.Parse(NumOfPlaygrounds.Text)
            );

            population = new(speciesParams);

            generationNum = 1;
        }

        private void updateText() {
            TournamentTable.ItemsSource = ConvertBestSpeciesToDataTable(population).DefaultView;

            AvgMinNumOfPlaygrounds.Text = population.AvgMinNumOfPlaygrounds.ToString();
            AvgMinNumOfOpponents.Text = population.AvgMinNumOfOpponents.ToString();

            BestMinNumOfPlaygrounds.Text = population.BestMinNumOfPlaygrounds.ToString();
            BestMinNumOfOpponents.Text = population.BestMinNumOfOpponents.ToString();

            Generation.Text = generationNum.ToString();
        }

        private async void StartBtn(object sender, RoutedEventArgs e) {
            RunButton.IsEnabled = false;
            LoadButton.IsEnabled = false;
            StopButton.IsEnabled = true;
            SaveButton.IsEnabled = true;

            CancelFlag = false;

            if (ShouldInitData) {
                initData();
            }

            while (generationNum <= Constants.NumOfSelectionRound && !CancelFlag) {
                var selectedPopulation = NaturalSelection.SelectionPopulation(population);
                var crossoverPopulation = NaturalSelection.CrossoverPopulation(selectedPopulation);
                var mutatedPopulation = NaturalSelection.MutationPopulation(crossoverPopulation);

                population = mutatedPopulation;

                await Task.Delay(100).ContinueWith(_ => {
                    Application.Current.Dispatcher.Invoke(() => {
                        updateText();
                    });
                });
                generationNum++;
            }
            RunButton.IsEnabled = true;
            LoadButton.IsEnabled = true;
            StopButton.IsEnabled = false;
            SaveButton.IsEnabled = false;

            ShouldInitData = true;
        }

        private void StopBtn(object sender, RoutedEventArgs e) {
            CancelFlag = true;
        }

        private DataTable ConvertBestSpeciesToDataTable(Population population) {
            DataTable dt = new DataTable();

            var bestSpecies = population.BestSpecies;
            if (bestSpecies == null || bestSpecies.Gens == null || bestSpecies.Gens.Length == 0) {
                return dt;
            }

            int maxColumns = 0;
            foreach (var gens in bestSpecies.Gens) {
                maxColumns = Math.Max(maxColumns, gens.Count);
            }

            for (int j = 0; j < maxColumns; j++)
                dt.Columns.Add($"Column{j + 1}", typeof(string));

            foreach (var gens in bestSpecies.Gens) {
                DataRow dr = dt.NewRow();
                for (int j = 0; j < gens.Count; j++) {
                    dr[j] = gens[j].ToString();
                }
                dt.Rows.Add(dr);
            }

            return dt;
        }

        private void SaveBtn(object sender, RoutedEventArgs e) {
            string populationJson = JsonConvert.SerializeObject(population);
            var dialog = new TextInputDialog();
            if (dialog.ShowDialog() == true) {
                string populationId = dialog.InputText;
                using (var db = new PopulationContext()) {
                    var populationState = new PopulationState {
                        Id = populationId,
                        GenerationNum = generationNum,
                        NumOfPlayers = speciesParams.NumOfPlayers,
                        NumOfTours = speciesParams.NumOfTours,
                        NumOfPlaygrounds = speciesParams.NumOfPlaygrounds,
                    };

                    var populationData = new PopulationData {
                        PopulationStateId = populationId,
                        PopulationJson = populationJson,    
                    };

                    populationState.Population = populationData;
                    populationData.PopulationState = populationState;

                    db.PopulationState.Add(populationState);
                    db.SaveChanges();
                }
            }
        }

        private void LoadBtn(object sender, RoutedEventArgs e) {
            using (var db = new PopulationContext()) {
                List<string> ids = db.PopulationState.Where(ps => ps.Id != null).Select(ps => ps.Id).ToList();
                var dialog = new ItemSelectionDialog(ids);
                if (dialog.ShowDialog() == true) {
                    var populationId = dialog.SelectedItem;
                
                    var populationState = db.PopulationState
                        .Include(p => p.Population)
                        .SingleOrDefault(p => p.Id == populationId);

                    var populationJson = populationState.Population.PopulationJson; 

                    var populationFromJson = JsonConvert.DeserializeObject<Population>(populationJson);

                    population = populationFromJson;
                    generationNum = populationState.GenerationNum;

                    speciesParams = new SpeciesParams(
                        NumOfPlayers: populationState.NumOfPlayers,
                        NumOfTours: populationState.NumOfTours,
                        NumOfPlaygrounds: populationState.NumOfPlaygrounds
                    );

                    NumOfPlayers.Text = populationState.NumOfPlayers.ToString();
                    NumOfTours.Text = populationState.NumOfTours.ToString();
                    NumOfPlaygrounds.Text = populationState.NumOfPlaygrounds.ToString();

                    ShouldInitData = false;

                    updateText();
                }
            }
        }
    }

    class PopulationState {
        public string? Id { get; set; }
        public int GenerationNum { get; set; }
        public int NumOfPlayers { get; set; }
        public int NumOfTours { get; set; }
        public int NumOfPlaygrounds { get; set; }
        public PopulationData Population { get; set; }
    }

    class PopulationData {
        public int Id { get; set; }
        public string PopulationJson { get; set; }
        public string PopulationStateId { get; set; }
        public PopulationState PopulationState { get; set; }
    }

    class PopulationContext : DbContext {
        public DbSet<PopulationState> PopulationState { get; set; }
        public DbSet<PopulationData> PopulationData { get; set; }
        public PopulationContext() => Database.EnsureCreated();
        protected override void OnConfiguring(DbContextOptionsBuilder o) => o.UseSqlite("Data Source=./../../../db/populationData.db");
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<PopulationState>()
                .HasOne(ps => ps.Population)
                .WithOne(pd => pd.PopulationState)
                .HasForeignKey<PopulationData>(pd => pd.PopulationStateId);

            modelBuilder.Entity<PopulationState>()
                .HasKey(ps => ps.Id);

            modelBuilder.Entity<PopulationData>()
                .HasKey(pd => pd.Id);
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
