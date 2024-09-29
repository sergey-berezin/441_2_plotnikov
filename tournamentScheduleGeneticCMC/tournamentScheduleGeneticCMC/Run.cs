namespace tournamentScheduleGeneticCMC;
using Genetic;

public static class Debug {
    private static Population _startPopulation = new();
    private static Population _population = new(_startPopulation.SpeciesGroup);
    private static int _round = 0;

    public static void Init() {
        _population = new Population();
        _startPopulation = new Population(_population.SpeciesGroup);
    }
    
    public static void Train() {
        Console.CancelKeyPress += OnCancelKeyPress;

        for (_round = 0; _round < Constants.NumOfSelectionRound; _round++) {
            var selectedPopulation = NaturalSelection.SelectionPopulation(_population);
            var crossoverPopulation = NaturalSelection.CrossoverPopulation(selectedPopulation);
            var mutatedPopulation = NaturalSelection.MutationPopulation(crossoverPopulation);

            _population = mutatedPopulation;

            Console.WriteLine($"\nRound: {_round}");
            _population.BestSpecies.Print(prefix:"BestSpecies:\n", printStat:true);
        }

        Console.WriteLine("____________________________");
        _startPopulation.Print("Start population:\n");
        Console.WriteLine();
        _population.Print("Result population:\n");

        _population.BestSpecies.Print("\nBestSpecies:\n");
    }
    
    static void OnCancelKeyPress(object sender, ConsoleCancelEventArgs e) {
        e.Cancel = true;

        Console.WriteLine("____________________________");
        Console.WriteLine($"Round: {_round}");
        _startPopulation.Print("Start population:\n");
        Console.WriteLine();
        _population.Print("Result population:\n");

        _population.BestSpecies.Print("\nBestSpecies:\n");
        
        e.Cancel = false;
    }
}