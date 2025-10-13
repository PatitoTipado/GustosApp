using System.Text.Json.Serialization;
using System.Collections.Generic;

namespace GustosApp.Application.DTO.PlacesV1
{
    public class NearbyRequest
    {
        [JsonPropertyName("includedTypes")]
        public List<string>? IncludedTypes { get; set; }

        [JsonPropertyName("maxResultCount")]
        public int? MaxResultCount { get; set; }

        [JsonPropertyName("languageCode")]
        public string? LanguageCode { get; set; }

        [JsonPropertyName("locationRestriction")]
        public LocationRestriction LocationRestriction { get; set; } = new();
    }

    public class LocationRestriction
    {
        [JsonPropertyName("circle")]
        public Circle Circle { get; set; } = new();
    }

    public class Circle
    {
        [JsonPropertyName("center")]
        public LatLng Center { get; set; } = new();

        [JsonPropertyName("radius")]
        public double Radius { get; set; }
    }

    public class LatLng
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }

    public class NearbyResponse
    {
        [JsonPropertyName("places")]
        public List<PlaceLite> Places { get; set; } = new();
    }

    public class PlaceLite
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("displayName")]
        public DisplayName DisplayName { get; set; } = new();

        [JsonPropertyName("primaryType")]
        public string? PrimaryType { get; set; }

        [JsonPropertyName("types")]
        public List<string>? Types { get; set; }

        [JsonPropertyName("priceLevel")]
        public string? PriceLevel { get; set; }

        [JsonPropertyName("rating")]
        public double? Rating { get; set; }

        [JsonPropertyName("userRatingCount")]
        public int? UserRatingCount { get; set; }

        [JsonPropertyName("currentOpeningHours")]
        public OpeningHours? CurrentOpeningHours { get; set; }

        [JsonPropertyName("location")]
        public LatLng Location { get; set; } = new();

        [JsonPropertyName("photos")]
        public List<PhotoRef>? Photos { get; set; }
    }

    public class PhotoRef
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class DisplayName
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("languageCode")]
        public string? LanguageCode { get; set; }
    }

    public class OpeningHours
    {
        [JsonPropertyName("openNow")]
        public bool? OpenNow { get; set; }
    }
}
