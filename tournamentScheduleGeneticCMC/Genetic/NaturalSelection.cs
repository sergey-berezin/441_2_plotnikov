namespace Genetic;

public static class NaturalSelection {
    public static Population SelectionPopulation(Population population) {
        // Tournament selection
        Random random = new();
        return new Population(
            population.SpeciesGroup
                .Select(_ => population.SpeciesGroup
                    .OrderBy(_ => random.Next())
                    .Take(Constants.TournamentSelectionSize)
                    .Max()
                )
                .ToList()!
        );
    }

    public static Population CrossoverPopulation(Population population) {
        Population resultPopulation = new();
        for (int i = 0; i < population.SpeciesGroup.Count - 1; i += 2) {
            var crossoverResult = 
                Species.Crossover(population.SpeciesGroup[i], population.SpeciesGroup[i + 1]);
                
            resultPopulation.SpeciesGroup[i] = crossoverResult.Key;
            resultPopulation.SpeciesGroup[i + 1] = crossoverResult.Value;
        }
        return resultPopulation;
    }
    
    public static Population MutationPopulation(Population population) {
        Population resultPopulation = new();
        
        for (var i = 0; i < Constants.PopulationSize; ++i) {
            resultPopulation.SpeciesGroup[i] = population.SpeciesGroup[i].Mutation();
        }
        return resultPopulation;
    }
}
