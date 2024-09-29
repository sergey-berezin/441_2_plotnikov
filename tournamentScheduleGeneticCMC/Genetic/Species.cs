namespace Genetic;

public class Species : IComparable<Species> {
    public readonly List<int?>[] _gens;

    public Species() {
        _gens = new List<int?>[Constants.NumOfTours];
        Random random = new();
        for (var tour = 0; tour < Constants.NumOfTours; ++tour) {
            var playgroundsNumbers = Enumerable.Range(0, Constants.NumOfPlaygrounds + 1)
                .OrderBy(_ => random.Next())
                .Take(Constants.NumOfPlayers / 2)
                .ToList();
            _gens[tour] = playgroundsNumbers
                .SelectMany(num => new List<int?> { num, num })
                .Concat(Constants.NumOfPlayers % 2 != 0 ? new List<int?> { null } : Enumerable.Empty<int?>())
                .OrderBy(x => random.Next())
                .ToList();
        }
    }
    
    public Species(List<int?>[] gens) {
        _gens = gens;
    }
    
    public int MinNumOfPlaygrounds =>
        Enumerable.Range(0, Constants.NumOfPlayers)
            .Select(player => Enumerable.Range(0, Constants.NumOfTours)
                .Select(tour => _gens[tour][player])
                .Distinct()
                .Count()
            )
            .Min();

    public int MinNumOfOpponents =>
        Enumerable.Range(0, Constants.NumOfPlayers)
            .Select(player => Enumerable.Range(0, Constants.NumOfTours)
                .SelectMany(tour =>
                    Enumerable.Range(0, Constants.NumOfPlayers)
                        .Where(otherPlayer =>
                            otherPlayer != player &&
                            _gens[tour][player] == _gens[tour][otherPlayer]
                        )
                        .Distinct()
                )
                .Distinct()
                .Count())
            .Min();

    public Species Mutation() {
        Random random = new();
        return new Species(
            _gens
                .Select(row => {
                    var newRow = row.ToList();

                    for (int index = 0; index < newRow.Count; index++) {
                        if (!MyRand.GetTrueWithProbability(Constants.MutationProbability))
                            continue;
                        var randomIndex = random.Next(0, Constants.NumOfPlayers);
                        (newRow[index], newRow[randomIndex]) = (newRow[randomIndex], newRow[index]);
                    }
                    return newRow;
                })
                .ToArray()
        );
    }

    public static KeyValuePair<Species, Species> Crossover(Species species1, Species species2) {
        if (!MyRand.GetTrueWithProbability(Constants.CrossoverProbability)) {
            return new KeyValuePair<Species, Species>(species1, species2);
        }
        return new KeyValuePair<Species, Species>(
            new Species(species1._gens
                .Select((tour, index) => index < Constants.NumOfTours / 2 ? species2._gens[index] : tour)
                .ToArray()),
            new Species(species2._gens
                .Select((tour, index) => index < Constants.NumOfTours / 2 ? species1._gens[index] : tour)
                .ToArray())
        );
    }

    public int CompareTo(Species? other) => other == null ? 1 
        : MinNumOfOpponents == other.MinNumOfOpponents
            ? MinNumOfPlaygrounds.CompareTo(other.MinNumOfPlaygrounds)
            : MinNumOfOpponents.CompareTo(other.MinNumOfOpponents);
}