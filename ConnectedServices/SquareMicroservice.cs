using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace FootballSquares.ConnectedServices.SquareMicroservice {
    public record GetSquareBySquareIDRequest([property:JsonPropertyName("square_id")] int squareID);

    public record GetSquareBySquareIDResponse(
        [property: JsonPropertyName("square_id")] int squareID,
        [property: JsonPropertyName("square_guid")] Guid squareGUID,
        [property: JsonPropertyName("square_size")] int squareSize,
        [property: JsonPropertyName("row_points")] string rowPoints,
        [property: JsonPropertyName("column_points")] string columnPoints,
        [property: JsonPropertyName("error_message")] string errorMessage
    );
    
    public record CreateSquareRequest(
        [property: JsonPropertyName("square_size")] int squareSize
    );
    public record CreateSquareResponse(
        [property: JsonPropertyName("square_id")] int squareID,
        [property: JsonPropertyName("square_guid")] Guid squareGUID,
        [property: JsonPropertyName("error_message")] string errorMessage
    );
    

    public class SquareMicroservice {
        private readonly HttpClient httpClient;
        private readonly string host;

        public SquareMicroservice(IConfiguration configuration) {
            this.httpClient = new HttpClient();

            this.host = configuration.GetValue<bool>("Debug") ? 
                configuration.GetValue<string>("FootballSquareConfigsDebug:SquareMicroservice:Host") ?? "" :
                configuration.GetValue<string>("FootballSquareConfigs:SquareMicroservice:Host") ?? "";
        }

        public async Task<GetSquareBySquareIDResponse> getSquareBySquareIDAsync(GetSquareBySquareIDRequest request) {
            Console.WriteLine(JsonSerializer.Serialize(request));
            
            var body = new StringContent(
                JsonSerializer.Serialize(request),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await this.httpClient.PostAsync( this.host + "/GetSquare", body);
            response.EnsureSuccessStatusCode();

            var getSquareBySquareID = JsonSerializer.Deserialize<GetSquareBySquareIDResponse>(
                await response.Content.ReadAsStringAsync()
            );

            return getSquareBySquareID ?? new GetSquareBySquareIDResponse(0, Guid.Empty, 0, "", "", "Unable to get Square");
        }

        public async Task<CreateSquareResponse> createSquareAsync(CreateSquareRequest request){
            Console.WriteLine(JsonSerializer.Serialize(request));

            var body = new StringContent(
                JsonSerializer.Serialize(request),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await this.httpClient.PostAsync(this.host + "/CreateSquare", body);
            response.EnsureSuccessStatusCode();

            CreateSquareResponse? createSquareResponse = JsonSerializer.Deserialize<CreateSquareResponse>(
                await response.Content.ReadAsStringAsync()
            );

            return createSquareResponse ?? new CreateSquareResponse(0, Guid.Empty, "Unable to create square");
        }
    }
}