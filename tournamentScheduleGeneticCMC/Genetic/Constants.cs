namespace Genetic;

public static class Constants {
    public const int NumOfSelectionRound = 10;
    
    public const int PopulationSize = 1000;
    
    public const int TournamentSelectionSize = 4;
    public const int CrossoverProbability = 80;
    public const int MutationProbability = 1;

    public const int NumOfTours = 2; // >= 1 | ROWS
    public const int NumOfPlayers = 4; // > NumOfTours | COLUMNS
    public const int NumOfPlaygrounds = 4; // >= NumOfPlayers
}
