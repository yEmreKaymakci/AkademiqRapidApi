namespace AkademiqRapidApi.Models
{
    public class ConvertCurrencyViewModel
    {

        public class Rootobject
        {
            public string from { get; set; }
            public To to { get; set; }
        }

        public class To
        {
            public float TRY { get; set; }
        }

    }
}
