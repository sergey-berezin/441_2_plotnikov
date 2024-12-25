const host = 'http://localhost:5000';
const timeoutDelay = 1;

let numOfPlayers;
let numOfTours;
let numOfPlaygrounds;

let population;
let bestSpecies;
let minNumOfOpponents;
let minNumOfPlaygrounds;
let avgMinNumOfOpponents;
let avgMinNumOfPlaygrounds;

let generationNum = 1;

let run = false;

function start() {
    numOfPlayers = document.getElementById('numOfPlayers').value;
    numOfTours = document.getElementById('numOfTours').value;
    numOfPlaygrounds = document.getElementById('numOfPlaygrounds').value;

    run = true;

    fetchInitialPopulation().then(_ => {
        oneGeneticItteration();
    }).catch(error => {
        console.error('Error in fetchInitialPopulation:', error);
    });
}

function oneGeneticItteration() {
    postNextGeneration().then(_ => {
        generateTable();

        document.getElementById('generationNum').innerText = generationNum;
        document.getElementById('minNumOfOpponents').innerText = minNumOfOpponents;
        document.getElementById('minNumOfPlaygrounds').innerText = minNumOfPlaygrounds;
        document.getElementById('avgMinNumOfOpponents').innerText = avgMinNumOfOpponents;
        document.getElementById('avgMinNumOfPlaygrounds').innerText = avgMinNumOfPlaygrounds;

        if (run) {
            ++generationNum;

            setTimeout(oneGeneticItteration, timeoutDelay);
        }
    }).catch(error => {
        console.error('Error in postNextGeneration:', error);
    });
}

async function fetchInitialPopulation() {
    try {
        const request = `${host}/initial?numOfPlayers=${numOfPlayers}&numOfTours=${numOfTours}&numOfPlaygrounds=${numOfPlaygrounds}`;

        console.log('postNextGeneration request: ', request);

        const response = await fetch(request);
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const data = await response.json();

        console.log('fetchInitialPopulation receive: ', data);

        population = new Population(
            data.SpeciesGroup,
            data.speciesParams,
            data.BestSpecies,
            data.BestMinNumOfOpponents,
            data.BestMinNumOfPlaygrounds,
            data.AvgMinNumOfOpponents,
            data.AvgMinNumOfPlaygrounds
        );
    } catch (error) {
        console.error('Error fetching initial population:', error);
    }
}

async function postNextGeneration() {
    try {
        const postNextInput = {
            NumOfPlayers: numOfPlayers,
            NumOfTours: numOfTours,
            NumOfPlaygrounds: numOfPlaygrounds,
            populationJson: JSON.stringify(population)
        };

        console.log('postNextGeneration send: ', postNextInput);

        const response = await fetch(`${host}/next`, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
            },
            body: JSON.stringify(postNextInput),
        });

        if (!response.ok) {
            throw new Error('Network response was not ok');
        }

        const data = await response.json();

        console.log('postNextGeneration receive: ', data);

        bestSpecies = data.bestSpecies;
        minNumOfOpponents = data.minNumOfOpponents;
        minNumOfPlaygrounds = data.minNumOfPlaygrounds;

        const newPopulation = JSON.parse(data.newPopulationJson);

        avgMinNumOfOpponents = newPopulation.AvgMinNumOfOpponents;
        avgMinNumOfPlaygrounds = newPopulation.AvgMinNumOfPlaygrounds;

        console.log('postNextGeneration receive population:', newPopulation);

        bestSpecies = JSON.parse(data.bestSpeciesJson);

        console.log('postNextGeneration receive bestSpecies:', bestSpecies);

        population = new Population(
            newPopulation.SpeciesGroup,
            newPopulation.speciesParams,
            newPopulation.BestSpecies,
            newPopulation.BestMinNumOfOpponents,
            newPopulation.BestMinNumOfPlaygrounds,
            newPopulation.AvgMinNumOfOpponents,
            newPopulation.AvgMinNumOfPlaygrounds
        );
    } catch (error) {
        console.error('Error posting next generation:', error);
    }
}

function stop() {
    run = false;
}

function generateTable() {
    let table = document.getElementById('outputTable');
    let html = '';

    for (let i = 0; i < numOfTours; i++) {
        html += '<tr>';
        for (let j = 0; j < numOfPlayers; j++) {
            html += `<td>${bestSpecies.Gens[i][j]}</td>`;
        }
        html += '</tr>';
    }

    table.innerHTML = html;
}

class SpeciesParams {
    constructor(numOfPlayers, numOfTours, numOfPlaygrounds) {
        this.numOfPlayers = numOfPlayers;
        this.numOfTours = numOfTours;
        this.numOfPlaygrounds = numOfPlaygrounds;
    }
}

class Species {
    constructor(gens, speciesParams, minNumOfPlaygrounds, minNumOfOpponents) {
        this.gens = gens;
        this.speciesParams = new SpeciesParams(
            speciesParams.NumOfPlayers,
            speciesParams.NumOfTours,
            speciesParams.NumOfPlaygrounds
        );
        this.minNumOfPlaygrounds = minNumOfPlaygrounds;
        this.minNumOfOpponents = minNumOfOpponents;
    }
}

class Population {
    constructor(speciesGroup, speciesParams, bestSpecies, bestMinNumOfOpponents, bestMinNumOfPlaygrounds, avgMinNumOfOpponents, avgMinNumOfPlaygrounds) {
        this.speciesGroup = speciesGroup.map(species => new Species(
            species.Gens,
            species.speciesParams,
            species.MinNumOfPlaygrounds,
            species.MinNumOfOpponents
        ));
        this.speciesParams = new SpeciesParams(
            speciesParams.NumOfPlayers,
            speciesParams.NumOfTours,
            speciesParams.NumOfPlaygrounds
        );
        this.bestSpecies = new Species(
            bestSpecies.Gens,
            bestSpecies.speciesParams,
            bestSpecies.MinNumOfPlaygrounds,
            bestSpecies.MinNumOfOpponents
        );
        this.bestMinNumOfOpponents = bestMinNumOfOpponents;
        this.bestMinNumOfPlaygrounds = bestMinNumOfPlaygrounds;
        this.avgMinNumOfOpponents = avgMinNumOfOpponents;
        this.avgMinNumOfPlaygrounds = avgMinNumOfPlaygrounds;
    }
}
