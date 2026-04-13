namespace AkademiqRapidApi.Models
{
    public class FootballViewModel
    {
        public class Rootobject
        {
            public List<Response> Response { get; set; } = new();
        }

        public class Response
        {
            public Fixture Fixture { get; set; } = new();
            public League League { get; set; } = new();
            public Teams Teams { get; set; } = new();
            public Goals Goals { get; set; } = new();
            public Score Score { get; set; } = new();
            public List<Events> Events { get; set; } = new();
        }

        public class Fixture
        {
            public int Id { get; set; }
            public string? Referee { get; set; }
            public string? Date { get; set; }
            public long? Timestamp { get; set; }
            public Venue? Venue { get; set; }
            public Status Status { get; set; } = new();
        }

        public class Venue
        {
            public int? Id { get; set; }
            public string? Name { get; set; }
            public string? City { get; set; }
        }

        public class Status
        {
            public string? Long { get; set; }
            public string? Short { get; set; }
            public int? Elapsed { get; set; }
        }

        public class League
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Country { get; set; }
            public string? Logo { get; set; }
            public string? Flag { get; set; }
            public int? Season { get; set; }
            public int? Round { get; set; }
        }

        public class Teams
        {
            public Team Home { get; set; } = new();
            public Team Away { get; set; } = new();
        }

        public class Team
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Logo { get; set; }
            public bool? Winner { get; set; }
        }

        public class Goals
        {
            public int? Home { get; set; }
            public int? Away { get; set; }
        }

        public class Score
        {
            public Goals? Halftime { get; set; }
            public Goals? Fulltime { get; set; }
            public Goals? Extratime { get; set; }
            public Goals? Penalty { get; set; }
        }

        public class Events
        {
            public Time? Time { get; set; }
            public Team? Team { get; set; }
            public Player? Player { get; set; }
            public Player? Assist { get; set; }
            public string? Type { get; set; }
            public string? Detail { get; set; }
        }

        public class Time
        {
            public int? Elapsed { get; set; }
        }

        public class Player
        {
            public int? Id { get; set; }
            public string? Name { get; set; }
        }
    }
}
