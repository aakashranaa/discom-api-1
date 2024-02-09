using System.ComponentModel.DataAnnotations.Schema;

namespace Solar.Db.Tables
{
    public class Applicant
    {
        [Column("EmailId")]
        public string EmailId { get; set; }
        public string? ReceiptConsumerName { get; set; }

        public string? Pin { get; set; }
        public int ApplicantID { get; set; }

        [Column("ConsumerNo")]
        public string? ConsumerNo { get; set; }
        public string? Address { get; set; }

        public string? HNo { get; set; }
        public string? StreetName { get; set; }
        public string? VillageName { get; set; }
        public string? MandalName { get; set; }
        public string? Mobile { get; set; }
        public string? IndividualLastName { get; set; }
        public string? IndividualFirstName { get; set; }

        public int? DistrictId { get; set; }
    }


    public class Area
    {
        public int? AreaID { get; set; }
        public string? AreaName { get; set; }
        public int? DistrictID { get; set; }
        public int? pAreaID { get; set; }
        public int? newOfficeTypeID { get; set; }
    }

    public class AreaSection
    {
        public string AreaName { get; set; }
    }

}
