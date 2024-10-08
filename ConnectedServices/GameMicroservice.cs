using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FootballSquares.ConnectedServices {
    public record GetGameByGUIDRequest([property:JsonPropertyName("game_guid")] Guid guid);
    public record GetGameByGUIDResponse(
        [property: JsonPropertyName("game_guid")] string gameGUID,
        [property: JsonPropertyName("game_id")] int gameId,
        [property: JsonPropertyName("sport")] string sport,
        [property: JsonPropertyName("team_a")] string teamA,
        [property: JsonPropertyName("team_b")] string teamB,
        [property: JsonPropertyName("error_message")] string errorMessage
    );

    public class GameMicroservice {
        private readonly string host;
        private readonly HttpClient httpClient = new HttpClient();
        

        public GameMicroservice(IConfiguration configuration) {
            this.host = configuration.GetValue<string>("FootballSquareConfigs:GameMicroservice:Host") ?? "";

        }

        public async Task<GetGameByGUIDResponse> getGameByGUIDAsync(GetGameByGUIDRequest request) {
            Console.WriteLine(JsonSerializer.Serialize(request));
            Console.WriteLine(this.host + "/GetGameByGUID");
            var body = new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json"
                );
            
            Console.WriteLine(body);

            using HttpResponseMessage response = await this.httpClient.PostAsync(
                this.host + "/GetGameByGUID", 
                body
            );
            response.EnsureSuccessStatusCode();

            GetGameByGUIDResponse? getGameByGUIDResponse = JsonSerializer.Deserialize<GetGameByGUIDResponse>(await response.Content.ReadAsStringAsync());

            return getGameByGUIDResponse ??
                    new GetGameByGUIDResponse("", 0, "", "", "", "Unable to get game");
        }
    }
}