namespace Genetic;

public class Species : IComparable<Species> {
    public readonly List<int>[] Gens;
    public SpeciesParams speciesParams { get; set; }

    public Species(SpeciesParams speciesParams) {
        this.speciesParams = speciesParams;
        Gens = new List<int>[speciesParams.NumOfTours];
        for (var tour = 0; tour < speciesParams.NumOfTours; ++tour) {
            var playgroundsNumbers = Enumerable.Range(0, speciesParams.NumOfPlaygrounds + 1)
                .OrderBy(_ => MyRand.Rnd.Next())
                .Take(speciesParams.NumOfPlayers / 2)
                .ToList();
            Gens[tour] = playgroundsNumbers
                .SelectMany(num => new List<int> { num, num })
                .Concat(speciesParams.NumOfPlayers % 2 != 0 ? new List<int> { -1 } : Enumerable.Empty<int>())
                .OrderBy(_ => MyRand.Rnd.Next())
                .ToList();
        }
    }

    private Species(List<int>[] gens, SpeciesParams speciesParams) {
        this.speciesParams = speciesParams;
        Gens = gens;
    }

    public int MinNumOfPlaygrounds =>
        Enumerable.Range(0, speciesParams.NumOfPlayers)
            .Select(player => Enumerable.Range(0, speciesParams.NumOfTours)
                .Select(tour => Gens[tour][player])
                .Distinct()
                .Count()
            )
            .DefaultIfEmpty(-1)
            .Min();

    public int MinNumOfOpponents =>
        Enumerable.Range(0, speciesParams.NumOfPlayers)
            .Select(player => Enumerable.Range(0, speciesParams.NumOfTours)
                .SelectMany(tour =>
                    Enumerable.Range(0, speciesParams.NumOfPlayers)
                        .Where(otherPlayer =>
                            otherPlayer != player &&
                            Gens[tour][player] == Gens[tour][otherPlayer]
                        )
                        .Distinct()
                )
                .Distinct()
                .Count())
            .DefaultIfEmpty(-1)
            .Min();

    public Species Mutation() {
        return new Species(
            Gens
                .Select(row => {
                    var newRow = row.ToList();

                    for (var index = 0; index < newRow.Count; index++) {
                        if (!MyRand.GetTrueWithProbability(Constants.MutationProbability))
                            continue;
                        var randomIndex = MyRand.Rnd.Next(0, speciesParams.NumOfPlayers);
                        (newRow[index], newRow[randomIndex]) = (newRow[randomIndex], newRow[index]);
                    }
                    return newRow;
                })
                .ToArray(),
            speciesParams
        );
    }

    public static KeyValuePair<Species, Species> Crossover(Species species1, Species species2, SpeciesParams speciesParams) {
        if (!MyRand.GetTrueWithProbability(Constants.CrossoverProbability)) {
            return new KeyValuePair<Species, Species>(species1, species2);
        }
        return new KeyValuePair<Species, Species>(
            new Species(species1.Gens
                .Select((tour, index) => index < speciesParams.NumOfTours / 2 ? species2.Gens[index] : tour)
                .ToArray(),
                speciesParams
            ),
            new Species(species2.Gens
                .Select((tour, index) => index < speciesParams.NumOfTours / 2 ? species1.Gens[index] : tour)
                .ToArray(),
                speciesParams
            )
        );
    }

    public int CompareTo(Species? other) => other == null ? 1 
        : MinNumOfOpponents == other.MinNumOfOpponents
            ? MinNumOfPlaygrounds.CompareTo(other.MinNumOfPlaygrounds)
            : MinNumOfOpponents.CompareTo(other.MinNumOfOpponents);
}
