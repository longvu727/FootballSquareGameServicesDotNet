using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FootballSquares.ConnectedServices {
    public record GetGameByGUIDRequest([property:JsonPropertyName("game_guid")] Guid guid);
    public record GetGameByGUIDResponse(
        [property: JsonPropertyName("game_guid")] string gameGUID,
        [property: JsonPropertyName("game_id")] int gameID,
        [property: JsonPropertyName("sport")] string sport,
        [property: JsonPropertyName("team_a")] string teamA,
        [property: JsonPropertyName("team_b")] string teamB,
        [property: JsonPropertyName("error_message")] string errorMessage
    );

    public record CreateGameRequest(
        [property: JsonPropertyName("sport")] string sport,
        [property: JsonPropertyName("team_a")] string teamA,
        [property: JsonPropertyName("team_b")] string teamB
    );

    public record CreateGameResponse(
        [property: JsonPropertyName("game_guid")] string gameGUID,
        [property: JsonPropertyName("game_id")] int gameID,
        [property: JsonPropertyName("error_message")] string errorMessage
    );


    public class GameMicroservice {
        private readonly string host;
        private readonly HttpClient httpClient;
        

        public GameMicroservice(IConfiguration configuration) {
            this.httpClient = new HttpClient();

            this.host = configuration.GetValue<bool>("Debug") ? 
                configuration.GetValue<string>("FootballSquareConfigsDebug:GameMicroservice:Host") ?? "" :
                configuration.GetValue<string>("FootballSquareConfigs:GameMicroservice:Host") ?? "";

        }

        public async Task<GetGameByGUIDResponse> getGameByGUIDAsync(GetGameByGUIDRequest request) {
            Console.WriteLine(JsonSerializer.Serialize(request));

            var body = new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json"
                );
            
            Console.WriteLine(body);

            HttpResponseMessage response = await this.httpClient.PostAsync(
                this.host + "/GetGameByGUID", 
                body
            );
            response.EnsureSuccessStatusCode();

            GetGameByGUIDResponse? getGameByGUIDResponse = JsonSerializer.Deserialize<GetGameByGUIDResponse>(await response.Content.ReadAsStringAsync());

            return getGameByGUIDResponse ??
                    new GetGameByGUIDResponse("", 0, "", "", "", "Unable to get game");
        }

        public async Task<CreateGameResponse> createGameAsync(CreateGameRequest request) {
            Console.WriteLine(JsonSerializer.Serialize(request));

            var body = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await this.httpClient.PostAsync(this.host + "/CreateGame", body);
            response.EnsureSuccessStatusCode();

            CreateGameResponse? createGameResponse = JsonSerializer.Deserialize<CreateGameResponse>(
                await response.Content.ReadAsStringAsync()
            );

            return createGameResponse ?? new CreateGameResponse("", 0, "Unable to create game");
        }
    }
}