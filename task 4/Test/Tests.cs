using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Genetic;

namespace app {
    public class Tests : IClassFixture<WebApplicationFactory<Program>> {
        private readonly WebApplicationFactory<Program> factory;
        public Tests(WebApplicationFactory<Program> factory) {
            this.factory = factory;
        }

        readonly string host = "http://localhost:5000";
        [Fact]
        public async Task Init200() {
            var client = factory.CreateClient();
            var response = await client.GetAsync($"{host}/initial?numOfPlayers=2&numOfTours=3&numOfPlaygrounds=4");
            Assert.Equal(200, (int)response.StatusCode);
        }
        [Fact]
        public async Task Init400() {
            var client = factory.CreateClient();
            var response = await client.GetAsync($"{host}/initial?sadsadsad");
            Assert.Equal(400, (int)response.StatusCode);
        }
        [Fact]
        public async Task InitCorrectData() {
            var client = factory.CreateClient();
            var numOfPLayers = 2;
            var numOFTours = 3;
            var numOfPlaygrounds = 4;
            var response = await client.GetAsync($"{host}/initial?numOfPlayers={numOfPLayers}&numOfTours={numOFTours}&numOfPlaygrounds={numOfPlaygrounds}");
            var data = await response.Content.ReadAsStringAsync();

            var json = JsonConvert.DeserializeObject<Population>(data)!;
            Assert.Equal(200, (int)response.StatusCode);
            Assert.Equal(json.speciesParams.NumOfPlayers, numOfPLayers);
            Assert.Equal(json.speciesParams.NumOfTours, numOFTours);
            Assert.Equal(json.speciesParams.NumOfPlaygrounds, numOfPlaygrounds);
        }
        [Fact]
        public async Task Next200() {
            var client = factory.CreateClient();

            var initResponse = await client.GetAsync($"{host}/initial?numOfPlayers=2&numOfTours=3&numOfPlaygrounds=4");
            var sendData = await initResponse.Content.ReadAsStringAsync();

            var response = await client.PostAsync($"{host}/next", new StringContent(sendData, System.Text.Encoding.UTF8, "application/json"));

            Assert.Equal(200, (int)response.StatusCode);
        }
        [Fact]
        public async Task Next400() {
            var client = factory.CreateClient();
            var response = await client.PostAsync($"{host}/next", new StringContent("Wrong", System.Text.Encoding.UTF8, "application/json"));
            Assert.Equal(400, (int)response.StatusCode);
        }
    }
}
