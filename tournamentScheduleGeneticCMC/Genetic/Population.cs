namespace Genetic;

public class Population {
    public readonly List<Species> SpeciesGroup;

    public Population() {
        SpeciesGroup = Enumerable.Range(0, Constants.PopulationSize)
            .Select(_ => new Species()).ToList();
    }
    public Population(List<Species> speciesGroup) {
        SpeciesGroup = speciesGroup;
    }

    public Species BestSpecies => SpeciesGroup.Max();
    public int BestMinNumOfOpponents => SpeciesGroup.Select(species => species.MinNumOfOpponents).Max();
    public int BestMinNumOfPlaygrounds => SpeciesGroup.Select(species => species.MinNumOfPlaygrounds).Max();
    public double AvgMinNumOfOpponents => SpeciesGroup.Select(species => species.MinNumOfOpponents).Average();
    public double AvgMinNumOfPlaygrounds => SpeciesGroup.Select(species => species.MinNumOfPlaygrounds).Average();
}