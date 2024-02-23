namespace Solar.Models
{
    public class Consumer200Response
    {
        public string status_code { get; set; }
        public string? consumer_address { get; set; }
        public string? consumer_lg_district_code { get; set; }
        public string? consumer_pin_code { get; set; }

        public string? connection_load { get; set; }
        public string? circle_name { get; set; }
        public string? circle_code { get; set; }
        public string? division_name { get; set; }
        public string? division_code { get; set; }
        public string? sub_division_name { get; set; }
        public string? sub_division_code { get; set; }
        public string? connection_type { get; set; }
        public string? consumer_mobile { get; set; }
        public string? consumer_email { get; set; }
        public string? consumer_name { get; set; }
        public string? existing_installed_capacity { get; set; }
    }

    public class ConsumerOtherResponse
    {
        public string status_code { get; set; }
        public string message { get; set; }
        public string consumerNo { get; set; }
    }
}
