namespace AkademiqRapidApi.Models
{
    public class RecipeViewModel
    {
        public class Rootobject
        {
            public List<Result> results { get; set; }
        }

        public class Result
        {
            public int id { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public string thumbnail_url { get; set; }
            public string original_video_url { get; set; }
            public int? total_time_minutes { get; set; }
            public string yields { get; set; }
            public List<Section> sections { get; set; }
            public List<Instruction> instructions { get; set; }
        }

        public class Section
        {
            public List<Component> components { get; set; }
        }

        public class Component
        {
            public string raw_text { get; set; }
        }

        public class Instruction
        {
            public string display_text { get; set; }
            public int position { get; set; }
        }
    }
}