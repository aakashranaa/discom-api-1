namespace Solar.Db.Tables
{
    public class DiscomApplicationForm
    {
        public int DiscomApplicationFormId { get; set; }
        // public string ConsumerDistrictCode { get; set; }

        public int ApplicantID { get; set; }

        public decimal? ConnectedLoad { get; set; }
        public string? CircleCode { get; set; }
        public string? DivisionCode { get; set; }
        public string? SubDivisionCode { get; set; }
        public string? PhaseType { get; set; }
        public decimal? EarlierInstalledCapacity { get; set; }
    
    }
}
