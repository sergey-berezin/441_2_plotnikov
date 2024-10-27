namespace Genetic;

public class Population {
    public readonly List<Species> SpeciesGroup;
    public SpeciesParams speciesParams { get; set; }

    public Population(SpeciesParams speciesParams) {
        this.speciesParams = speciesParams;
        SpeciesGroup = Enumerable.Range(0, Constants.PopulationSize)
            .Select(_ => new Species(speciesParams)).ToList();
    }
    public Population(List<Species> speciesGroup, SpeciesParams speciesParams) {
        this.speciesParams = speciesParams;
        SpeciesGroup = speciesGroup;
    }

    public Species BestSpecies => SpeciesGroup.Max();
    public int BestMinNumOfOpponents => SpeciesGroup.Select(species => species.MinNumOfOpponents).Max();
    public int BestMinNumOfPlaygrounds => SpeciesGroup.Select(species => species.MinNumOfPlaygrounds).Max();
    public double AvgMinNumOfOpponents => SpeciesGroup.Select(species => species.MinNumOfOpponents).Average();
    public double AvgMinNumOfPlaygrounds => SpeciesGroup.Select(species => species.MinNumOfPlaygrounds).Average();
}
