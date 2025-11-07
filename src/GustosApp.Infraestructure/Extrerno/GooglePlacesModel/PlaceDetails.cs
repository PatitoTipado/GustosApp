using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GustosApp.Infraestructure.Extrerno.GooglePlacesModel
{
    public class PlaceDetails
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("displayName")]
        public DisplayName? DisplayName { get; set; }

        [JsonPropertyName("formattedAddress")]
        public string? FormattedAddress { get; set; }

        [JsonPropertyName("location")]
        public LatLng? Location { get; set; }

        [JsonPropertyName("photos")]
        public List<PlacePhoto>? Photos { get; set; }

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

        //  Flags de servicio disponibles
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

    
        [JsonPropertyName("reviews")]
        public List<PlaceReview>? Reviews { get; set; }
    }

   

    public class DisplayName
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }

    public class LatLng
    {
        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }

    public class PlacePhoto
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }
    }

    public class OpeningHours
    {
        [JsonPropertyName("openNow")]
        public bool? OpenNow { get; set; }

        [JsonPropertyName("periods")]
        public List<OpeningPeriod>? Periods { get; set; }
    }

    public class OpeningPeriod
    {
        [JsonPropertyName("open")]
        public OpeningEvent? Open { get; set; }

        [JsonPropertyName("close")]
        public OpeningEvent? Close { get; set; }
    }

    public class OpeningEvent
    {
        [JsonPropertyName("day")]
        public int? Day { get; set; }

        [JsonPropertyName("hour")]
        public int? Hour { get; set; }

        [JsonPropertyName("minute")]
        public int? Minute { get; set; }
    }

    //  Modelo para reseñas (reviews)
    public class PlaceReview
    {
        [JsonPropertyName("rating")]
        public double Rating { get; set; }

        [JsonPropertyName("relativePublishTimeDescription")]
        public string? RelativePublishTimeDescription { get; set; }

        [JsonPropertyName("text")]
        public PlaceText? Text { get; set; }

        [JsonPropertyName("authorAttribution")]
        public AuthorAttribution? AuthorAttribution { get; set; }
    }

    public class PlaceText
    {
        [JsonPropertyName("text")]
        public string? Text { get; set; }
    }

    public class AuthorAttribution
    {
        [JsonPropertyName("displayName")]
        public string? DisplayName { get; set; }

        [JsonPropertyName("photoUri")]
        public string? PhotoUri { get; set; }
    }
}
