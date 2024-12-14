namespace Genetic;

public static class NaturalSelection {
    public static Population SelectionPopulation(Population population) {
        // Tournament selection

        return new Population(
            population.SpeciesGroup
                .AsParallel()
                .Select(_ => population.SpeciesGroup
                    .OrderBy(_ => MyRand.Rnd.Next())
                    .Take(Constants.TournamentSelectionSize)
                    .Max()
                )
                .ToList(),
            population.speciesParams
        );
    }

    public static Population CrossoverPopulation(Population population) {
        Population resultPopulation = new(population.speciesParams);
        Parallel.For(0, (population.SpeciesGroup.Count - 1) / 2, index => {
            int i = index * 2;
            var crossoverResult =
                Species.Crossover(population.SpeciesGroup[i], population.SpeciesGroup[i + 1], population.speciesParams);

            resultPopulation.SpeciesGroup[i] = crossoverResult.Key;
            resultPopulation.SpeciesGroup[i + 1] = crossoverResult.Value;
        });
        return resultPopulation;
    }
    
    public static Population MutationPopulation(Population population) {
        Population resultPopulation = new(population.speciesParams);

        Parallel.For(0, Constants.PopulationSize, i => {
            resultPopulation.SpeciesGroup[i] = population.SpeciesGroup[i].Mutation();
        });
        return resultPopulation;
    }
}
