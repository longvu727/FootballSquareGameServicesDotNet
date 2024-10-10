using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Unicode;

namespace FootballSquares.ConnectedServices {
    public record GetUserByUserIDRequest([property:JsonPropertyName("user_id")] int userID);

    public record GetUserByUserIDResponse(
        [property: JsonPropertyName("user_id")] int userID,
        [property: JsonPropertyName("user_guid")] string userGUID,
        [property: JsonPropertyName("ip")] string ip,
        [property: JsonPropertyName("device_name")] string deviceName,
        [property: JsonPropertyName("user_name")] string userName,
        [property: JsonPropertyName("alias")] string alias,
        [property: JsonPropertyName("error_message")] string errorMessage
    );

    public record GetUserByUserGUIDRequest([property:JsonPropertyName("user_guid")] Guid userGUID);

    public record GetUserByUserGUIDResponse(
        [property: JsonPropertyName("user_id")] int userID,
        [property: JsonPropertyName("user_guid")] string userGUID,
        [property: JsonPropertyName("ip")] string ip,
        [property: JsonPropertyName("device_name")] string deviceName,
        [property: JsonPropertyName("user_name")] string userName,
        [property: JsonPropertyName("alias")] string alias,
        [property: JsonPropertyName("error_message")] string errorMessage
    );

    public class UserMicroservice {
        private readonly HttpClient httpClient;
        private readonly string host;

        public UserMicroservice(IConfiguration configuration){
            this.httpClient = new HttpClient();

            this.host = configuration.GetValue<bool>("Debug") ?
                configuration.GetValue<string>("FootballSquareConfigsDebug:UserMicroservice:Host") ?? "" :
                configuration.GetValue<string>("FootballSquareConfigs:UserMicroservice:Host") ?? "";
        }

        public async Task<GetUserByUserIDResponse> getUserByUserIDAsync(GetUserByUserIDRequest request) {
            Console.WriteLine(JsonSerializer.Serialize(request));

            var body = new StringContent(
                JsonSerializer.Serialize(request),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await this.httpClient.PostAsync(this.host + "/GetUser", body);
            response.EnsureSuccessStatusCode();

            string responseBody =await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);

            GetUserByUserIDResponse? getUserByUserIDResponse = JsonSerializer.Deserialize<GetUserByUserIDResponse>(responseBody);

            return getUserByUserIDResponse ?? new GetUserByUserIDResponse(0, "", "", "", "", "", "Unable to get user");
        }

        public async Task<GetUserByUserGUIDResponse> getUserByUserGUIDAsync(GetUserByUserGUIDRequest request) {
            Console.WriteLine(JsonSerializer.Serialize(request));

            var body = new StringContent(
                JsonSerializer.Serialize(request),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            HttpResponseMessage response = await this.httpClient.PostAsync(this.host + "/GetUserByGUID", body);
            response.EnsureSuccessStatusCode();

            string responseBody =await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);

            GetUserByUserGUIDResponse? getUserByUserGUIDResponse = JsonSerializer.Deserialize<GetUserByUserGUIDResponse>(responseBody);

            return getUserByUserGUIDResponse ?? new GetUserByUserGUIDResponse(0, "", "", "", "", "", "Unable to get user");
        }
    }
}