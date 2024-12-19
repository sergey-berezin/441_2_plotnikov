namespace Genetic {
    public class SpeciesParams {
        public int NumOfPlayers { get; set; }
        public int NumOfTours { get; set; }
        public int NumOfPlaygrounds { get; set; }

        public SpeciesParams(int NumOfPlayers, int NumOfTours, int NumOfPlaygrounds) {
            this.NumOfPlayers = NumOfPlayers;
            this.NumOfTours = NumOfTours;
            this.NumOfPlaygrounds = NumOfPlaygrounds;
        }
    }
}
