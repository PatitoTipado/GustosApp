using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GustosApp.Application.DTO
{
    public class GooglePlacesResponse
    {
        public List<GooglePlaceResult> Results { get; set; } = new();
    }

    public class GooglePlaceResult
    {
        public string PlaceId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string? Vicinity { get; set; }
        public double Rating { get; set; }
        public int UserRatingsTotal { get; set; }
        public List<string> Types { get; set; } = new();
        public Geometry Geometry { get; set; } = new();
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
        public string PhotoReference { get; set; } = string.Empty;
    }

}
