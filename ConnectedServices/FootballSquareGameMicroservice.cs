using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FootballSquares.ConnectedServices.FootballSquareGameMicroservice {


    public record FootballSquare(
        [property:JsonPropertyName("football_square_game_id")] int footballSquareGameID,
        [property:JsonPropertyName("column_index")] int columnIndex,
        [property:JsonPropertyName("row_index")] int rowIndex,
        [property:JsonPropertyName("winner_quarter_number")] int winnerQuarterNumber,
        [property:JsonPropertyName("winner")] bool winner,
        [property:JsonPropertyName("user_id")] int userID,
        [property:JsonPropertyName("square_id")] int squareID,
        [property:JsonPropertyName("game_id")] int gameID,
        [property:JsonPropertyName("user_name")] string userName,
        [property:JsonPropertyName("user_alias")] string userAlias
    );

    public record GetFootballSquareGameByGameIDRequest([property: JsonPropertyName("game_id")] int gameID);

    public record GetFootballSquareGameByGameIDResponse(
        [property: JsonPropertyName("football_squares")] List<FootballSquare> footballSquares,
        [property: JsonPropertyName("error_message")] string errorMessage
    );

    public record CreateFootballSquareGameRequest(
        [property: JsonPropertyName("square_id")] int squareID,
        [property: JsonPropertyName("game_id")] int gameID,
        [property: JsonPropertyName("square_size")] int squareSize
    );

    public record CreateFootballSquareGameResponse(
        [property: JsonPropertyName("football_square_game_ids")] List<int> footballSquareGameIDs,
        [property: JsonPropertyName("error_message")] string errorMessage
    );

    public record ReserveFootballSquareRequest(
        [property: JsonPropertyName("user_id")] int userID,
        [property: JsonPropertyName("game_id")] int gameID,
        [property:JsonPropertyName("column_index")] int columnIndex,
        [property:JsonPropertyName("row_index")] int rowIndex
    );

    public record ReserveFootballSquareResponse(
        [property: JsonPropertyName("reserved")] bool reserved,
        [property: JsonPropertyName("error_message")] string errorMessage
    );

    public class FootballSquareGameMicroservice {
        private readonly HttpClient httpClient;
        private readonly string host;

        public FootballSquareGameMicroservice(IConfiguration configuration){
            this.httpClient = new HttpClient();

            this.host = configuration.GetValue<bool>("Debug") ? 
                configuration.GetValue<string>("FootballSquareConfigsDebug:FootballSquareGameMicroservice:Host") ?? "" :
                configuration.GetValue<string>("FootballSquareConfigs:FootballSquareGameMicroservice:Host") ?? "";
        }

        public async Task<GetFootballSquareGameByGameIDResponse> getFootballSquareGameByGameIDAsync(GetFootballSquareGameByGameIDRequest request){
            Console.WriteLine(JsonSerializer.Serialize(request));

            var body = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json"
            );

            Console.WriteLine(body);

            HttpResponseMessage response = await this.httpClient.PostAsync(this.host + "/GetFootballSquareGameByGameID", body);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);

            GetFootballSquareGameByGameIDResponse? getFootballSquareGameByGameIDResponse = 
                JsonSerializer.Deserialize<GetFootballSquareGameByGameIDResponse>(responseBody);

            return getFootballSquareGameByGameIDResponse ??
                new GetFootballSquareGameByGameIDResponse(new List<FootballSquare>(), "Unable to get football square game.");
        }

        public async Task<CreateFootballSquareGameResponse> createFootballSquareGame(CreateFootballSquareGameRequest request) {
            Console.WriteLine(JsonSerializer.Serialize(request));

            var body = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await this.httpClient.PostAsync(this.host + "/CreateFootballSquareGame", body);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);

            CreateFootballSquareGameResponse? createFootballSquareGameResponse = 
                JsonSerializer.Deserialize<CreateFootballSquareGameResponse>(responseBody);

            return createFootballSquareGameResponse 
                ?? new CreateFootballSquareGameResponse(new List<int>(), "Unable to create football square game");
        }

        public async Task<ReserveFootballSquareResponse> reserveFootballSquare(ReserveFootballSquareRequest request) {
            Console.WriteLine(JsonSerializer.Serialize(request));

            var body = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await this.httpClient.PostAsync(this.host + "/ReserveFootballSquare", body);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);

            ReserveFootballSquareResponse? reserveFootballSquareResponse = 
                JsonSerializer.Deserialize<ReserveFootballSquareResponse>(responseBody);

            return reserveFootballSquareResponse 
                ?? new ReserveFootballSquareResponse(false, "Unable to create football square game");
        }

    }

}