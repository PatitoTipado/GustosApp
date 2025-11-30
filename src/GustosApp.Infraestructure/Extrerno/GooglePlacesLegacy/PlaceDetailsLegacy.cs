using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GustosApp.Infraestructure.Extrerno.GooglePlacesLegacy
{
    public class GoogleLegacyDetailsResponse
    {
        [JsonPropertyName("result")]
        public GoogleLegacyResult? Result { get; set; }
    }
    
    public class GoogleLegacyResult
    {
        [JsonPropertyName("reviews")]
        public List<GoogleLegacyReview>? Reviews { get; set; }
    }

    public class GoogleLegacyReview
    {
        [JsonPropertyName("author_name")]
        public string AuthorName { get; set; } = "";

        [JsonPropertyName("profile_photo_url")]
        public string? ProfilePhotoUrl { get; set; }

        [JsonPropertyName("rating")]
        public double Rating { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; } = "";

        [JsonPropertyName("relative_time_description")]
        public string? RelativeTimeDescription { get; set; }
    }

}
