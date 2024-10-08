using System.Text.Json.Serialization;

namespace FootballSquares.Dtos {
    public record SquareRow(
        [property:JsonPropertyName("column_index")] int columnIndex,
        [property:JsonPropertyName("row_index")] int rowIndex,
        [property:JsonPropertyName("winner_quarter_number")] int winnerQuarterNumber,
        [property:JsonPropertyName("winner")] bool winner,
        [property:JsonPropertyName("user_guid")] string userGuid,
        [property:JsonPropertyName("user_alias")] string userAlias,
        [property:JsonPropertyName("user_name")] string userName
    );
    public record GameResponse(
        [property:JsonPropertyName("game_guid")] string guid,
        [property:JsonPropertyName("sport")] string sport,
        [property:JsonPropertyName("team_a")] string teamA,
        [property:JsonPropertyName("team_b")] string teamB,
        [property:JsonPropertyName("square_size")] int squareSize,
        [property:JsonPropertyName("row_points")] string rowPoints,
        [property:JsonPropertyName("column_points")] string columnPoints,
        [property:JsonPropertyName("football_squares")] List<SquareRow> squareData,
        
        [property:JsonPropertyName("error_message")] string errorMessage
    );
    public record CreateGameResponse(Guid guid, string sport);
    public record ReserveGameResponse(
        bool reserve,
        [property:JsonPropertyName("error_message")] string errorMessage
    );

    public class CreateGameRequest{
        [JsonPropertyName("sport")]
        public string Sport { get; set; } = "";
        
        [JsonPropertyName("square_size")]
        public int SquareSize { get; set; } = 0;
        
        [JsonPropertyName("team_a")]
        public string TeamA { get; set; } = "";
        
        [JsonPropertyName("team_b")]
        public string TeamB { get; set; } = "";
    }

    public class ReserveGameRequest {
        [JsonPropertyName("user_guid")]
        public Guid UserGUID{get; set;} = Guid.Empty;
        
        [JsonPropertyName("game_guid")]
        public Guid GameGUID{get; set;} = Guid.Empty;
        
        [JsonPropertyName("row_index")]
        public int RowIndex{get; set;} = 0;
        
        [JsonPropertyName("column_index")]
        public int ColumnIndex{get; set;} = 0;
    }
}