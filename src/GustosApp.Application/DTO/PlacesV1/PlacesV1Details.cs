using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace GustosApp.Application.DTO.PlacesV1
{
    public class PlaceDetails
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("priceLevel")]
        public string? PriceLevel { get; set; }

        [JsonPropertyName("rating")]
        public double? Rating { get; set; }

        [JsonPropertyName("userRatingCount")]
        public int? UserRatingCount { get; set; }

        [JsonPropertyName("currentOpeningHours")]
        public OpeningHours? CurrentOpeningHours { get; set; }

        [JsonPropertyName("primaryType")]
        public string? PrimaryType { get; set; }

        [JsonPropertyName("types")]
        public List<string>? Types { get; set; }

        // Serves flags
        [JsonPropertyName("delivery")] public bool? Delivery { get; set; }
        [JsonPropertyName("takeout")] public bool? Takeout { get; set; }
        [JsonPropertyName("dineIn")] public bool? DineIn { get; set; }
        [JsonPropertyName("curbsidePickup")] public bool? CurbsidePickup { get; set; }
        [JsonPropertyName("reservable")] public bool? Reservable { get; set; }

        [JsonPropertyName("servesBreakfast")] public bool? ServesBreakfast { get; set; }
        [JsonPropertyName("servesBrunch")] public bool? ServesBrunch { get; set; }
        [JsonPropertyName("servesLunch")] public bool? ServesLunch { get; set; }
        [JsonPropertyName("servesDinner")] public bool? ServesDinner { get; set; }
        [JsonPropertyName("servesBeer")] public bool? ServesBeer { get; set; }
        [JsonPropertyName("servesWine")] public bool? ServesWine { get; set; }
        [JsonPropertyName("servesCocktails")] public bool? ServesCocktails { get; set; }
        [JsonPropertyName("servesCoffee")] public bool? ServesCoffee { get; set; }
        [JsonPropertyName("servesVegetarianFood")] public bool? ServesVegetarianFood { get; set; }
    }
}
