using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.DTO
{
    using System.Text.Json.Serialization;

    public class GooglePlaceDetailsResponse
    {
        [JsonPropertyName("result")]
        public GooglePlaceDetailsResult Result { get; set; } = new();
    }

    public class GooglePlaceDetailsResult
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("formatted_address")]
        public string? FormattedAddress { get; set; }

        [JsonPropertyName("types")]
        public List<string>? Types { get; set; }

        [JsonPropertyName("rating")]
        public double? Rating { get; set; }

        [JsonPropertyName("user_ratings_total")]
        public int? UserRatingsTotal { get; set; }

        [JsonPropertyName("website")]
        public string? Website { get; set; }

        [JsonPropertyName("reviews")]
        public List<GooglePlaceReview>? Reviews { get; set; }
    }

    public class GooglePlaceReview
    {
        [JsonPropertyName("author_name")]
        public string AuthorName { get; set; } = string.Empty;

        [JsonPropertyName("rating")]
        public double Rating { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;
    }

}
