namespace Genetic;

public static class Constants {
    public const int NumOfSelectionRound = 1000;
    
    public const int PopulationSize = 1000;
    
    public const int TournamentSelectionSize = 4;
    public const int CrossoverProbability = 80;
    public const int MutationProbability = 1;

    public const int NumOfTours = 7; // >= 1 | ROWS
    public const int NumOfPlayers = 8; // > NumOfTours | COLUMNS
    public const int NumOfPlaygrounds = 10; // >= NumOfPlayers
}
