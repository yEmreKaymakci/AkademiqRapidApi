namespace AkademiqRapidApi.Models
{
    public class MusicViewModel
    {
        public class Rootobject
        {
            public List<Track>? data { get; set; }
            public int? total { get; set; }
        }

        public class Track
        {
            public long id { get; set; }
            public string? title { get; set; }
            public string? title_short { get; set; }
            public string? link { get; set; }
            public int? duration { get; set; }
            public string? preview { get; set; }
            public Artist? artist { get; set; }
            public Album? album { get; set; }
        }

        public class Artist
        {
            public long id { get; set; }
            public string? name { get; set; }
            public string? link { get; set; }
            public string? picture_medium { get; set; }
            public string? picture_xl { get; set; }
        }

        public class Album
        {
            public long id { get; set; }
            public string? title { get; set; }
            public string? cover_medium { get; set; }
            public string? cover_xl { get; set; }
        }
    }
}
