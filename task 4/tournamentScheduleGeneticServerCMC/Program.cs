using Genetic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddDefaultPolicy(builder => {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.MapGet("/initial", (int numOfPlayers, int numOfTours, int numOfPlaygrounds) => {
    var speciesParams = new SpeciesParams(numOfPlayers, numOfTours, numOfPlaygrounds);
    var population = new Population(speciesParams);
    string populationJson = JsonConvert.SerializeObject(population);
    return populationJson;
})
.WithName("Init population")
.WithOpenApi();

app.MapPost("/next", ([FromBody] PostNextInput input) => {
    Population population = JsonConvert.DeserializeObject<Population>(input.populationJson);

    Population selectedPopulation = NaturalSelection.SelectionPopulation(population);
    Population crossoverPopulation = NaturalSelection.CrossoverPopulation(selectedPopulation);
    Population mutatedPopulation = NaturalSelection.MutationPopulation(crossoverPopulation);

    string newPopulationJson = JsonConvert.SerializeObject(mutatedPopulation);

    string bestSpeciesJson = JsonConvert.SerializeObject(mutatedPopulation.BestSpecies);

    int minNumOfOpponents = mutatedPopulation.BestSpecies.MinNumOfOpponents;
    int minNumOfPlaygrounds = mutatedPopulation.BestSpecies.MinNumOfPlaygrounds;

    return new PostNextOutput(
       NewPopulationJson: newPopulationJson,
       BestSpeciesJson: bestSpeciesJson,
       MinNumOfOpponents: minNumOfOpponents,
       MinNumOfPlaygrounds: minNumOfPlaygrounds
    );
})
.WithName("NextGeneration")
.WithOpenApi();

app.Run();

record PostNextInput(
    int NumOfPlayers,
    int NumOfTours,
    int NumOfPlaygrounds,
    string populationJson
);

record PostNextOutput(
    string NewPopulationJson,
    string BestSpeciesJson,
    int MinNumOfOpponents,
    int MinNumOfPlaygrounds
);
