using Genetic;

namespace tournamentScheduleGeneticCMC;

public static class DebugExtensions {
    public static void Print(this Species species, string prefix = "", bool printStat = false) {
        Console.Write(prefix);
        for (var tour = 0; tour < Constants.NumOfTours; ++tour) {
            for (var player = 0; player < Constants.NumOfPlayers; ++player) {
                Console.Write(species.Gens[tour][player] == null ? "- " : $"{species.Gens[tour][player]} ");
            }
            Console.WriteLine();
        }
        if (!printStat) return;
        Console.WriteLine($"MinNumOfOpponents: {species.MinNumOfOpponents}");
        Console.WriteLine($"MinNumOfPlaygrounds: {species.MinNumOfPlaygrounds}");
    }
    
    public static void Print(this Population population, string prefix = "", bool printSpecies = false, bool printStat = true, bool printSpeciesStat = false) {
        Console.Write(prefix);
        if (printSpecies) {
            foreach (var species in population.SpeciesGroup) {
                species.Print(printStat:printSpeciesStat);
                Console.WriteLine();
            }
        }
        if (!printStat) return;
        Console.WriteLine($"BestMinNumOfOpponents: {population.BestMinNumOfOpponents}");
        Console.WriteLine($"BestMinNumOfPlaygrounds: {population.BestMinNumOfPlaygrounds}");
        Console.WriteLine($"AvgMinNumOfPlaygrounds: {population.AvgMinNumOfPlaygrounds}");
        Console.WriteLine($"AvgMinNumOfOpponents: {population.AvgMinNumOfOpponents}");
    }
}