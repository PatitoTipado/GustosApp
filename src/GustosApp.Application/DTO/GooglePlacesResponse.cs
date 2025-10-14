using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;


namespace GustosApp.Application.DTO
{
    public class GooglePlacesResponse
    {
       [JsonPropertyName("results")]
        public List<GooglePlaceResult> Results { get; set; } = new();
    }

    public class GooglePlaceResult
    {
        [JsonPropertyName("place_id")]
        public string PlaceId { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("vicinity")]
        public string? Vicinity { get; set; }

        [JsonPropertyName("rating")]
        public double? Rating { get; set; }

        [JsonPropertyName("user_ratings_total")]
        public int? UserRatingsTotal { get; set; }

        [JsonPropertyName("types")]
        public List<string> Types { get; set; } = new();

        [JsonPropertyName("geometry")]
        public Geometry Geometry { get; set; } = new();

        [JsonPropertyName("photos")]
        public List<Photo>? Photos { get; set; }
    }

    public class Geometry
    {
        public Location Location { get; set; } = new();
    }

    public class Location
    {
        public double Lat { get; set; }
        public double Lng { get; set; }
    }

    public class Photo
    {
       [JsonPropertyName("photo_reference")]
        public string PhotoReference { get; set; } = string.Empty;
    }
}
