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
    
    public class CreateGameResponse{
        [JsonPropertyName("game_guid")] public string gameGUID{get; set;} = "";
        [JsonPropertyName("sport")] public string sport{get; set;} = "";
        [JsonPropertyName("error_message")] public string errorMessage{get; set;} = "";
    };

    public class ReserveGameResponse{
        public bool reserved{get; set;} = false;
        [JsonPropertyName("error_message")] public string errorMessage = "";
    };

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

    public class GameResponse{
        [JsonPropertyName("game_guid")]public string gameGUID{get; set;} = "";
        [JsonPropertyName("sport")] public string sport{get; set;}="";
        [JsonPropertyName("team_a")] public string teamA{get; set;}="";
        [JsonPropertyName("team_b")] public string teamB{get; set;}="";
        [JsonPropertyName("square_size")] public int squareSize{get; set;}=0;
        [JsonPropertyName("row_points")] public string rowPoints{get; set;}="";
        [JsonPropertyName("column_points")] public string columnPoints{get; set;}="";
        [JsonPropertyName("football_squares")] public List<SquareRow> squareData{get; set;}= new List<SquareRow>();
        [JsonPropertyName("error_message")] public string errorMessage{get; set;} ="";
    };
}