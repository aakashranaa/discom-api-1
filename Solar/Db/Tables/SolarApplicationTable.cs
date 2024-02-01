using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Solar.Db.Tables
{
    [Table("SolarApplications")]
    public class SolarApplicationTable
    {
        [Column("UniqueId")]
        public string UniqueId { get; set; }
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [Column("ApplicantEmail")]
        public string EmailId { get; set; }

        [Column("EmailSent")]
        public bool EmailSent { get; set; }

        [Column("DiscomName")]
        public string DiscomName { get; set; }

        [Column("ServiceConnectionNumber")]
        public string ServiceConnectionNumber { get; set; }

        [Column("ConsumerName")]
        public string ConsumerName { get; set; }

        [Column("PhaseType")]
        public string PhaseType { get; set; }

        [Column("SanctionLoad")]
        public decimal? SanctionLoad { get; set; }

        [Column("SiteAddress")]
        public string SiteAddress { get; set; }

        [Column("SiteSector")]
        public string SiteSector { get; set; }

        [Column("SitePinCode")]
        public string SitePinCode { get; set; }

        [Column("ProposedSPVPlantCapacity")]
        public decimal? ProposedSPVPlantCapacity { get; set; }

        [Column("MonthlyConsumption")]
        public string MonthlyConsumption { get; set; }

        [Column("AnnualConsumption")]
        public string AnnualConsumption { get; set; }

        [Column("EarlierInstalledSPVPlantCapacity")]
        public string EarlierInstalledSPVPlantCapacity { get; set; }

        [Column("IsApplicationUnderProcess")]
        public bool IsApplicationUnderProcess { get; set; }

        [Column("ApplicationSPVPlantCapacity")]
        public string ApplicationSPVPlantCapacity { get; set; }

        [Column("ApplicationNo")]
        public string ApplicationNo { get; set; }

        [Column("ApplicantDoorNo")]
        public string ApplicantDoorNo { get; set; }

        [Column("ApplicantSector")]
        public string ApplicantSector { get; set; }

        [Column("ApplicantPinCode")]
        public string ApplicantPinCode { get; set; }

        [Column("ApplicantPrimaryContactName")]
        public string ApplicantPrimaryContactName { get; set; }

        [Column("ApplicantMobileNumber")]
        public string ApplicantMobileNumber { get; set; }

        [Column("ApplicantPhoneNumber")]
        public string ApplicantPhoneNumber { get; set; }

        [Column("ApplicantIDProofType")]
        public string ApplicantIDProofType { get; set; }

        [Column("ApplicantIDProofNo")]
        public string ApplicantIDProofNo { get; set; }

        [Column("ApplicantHasAadhaarCard")]
        public bool ApplicantHasAadhaarCard { get; set; }

        [Column("DeclarationText")]
        public string DeclarationText { get; set; }


        [Column("PANCardNo")]
        public string PANCardNo { get; set; }


        [Column("ApplicantPhoto")]
        public byte[] ApplicantPhoto { get; set; }

        [Column("ApplicantSignature")]
        public byte[] ApplicantSignature { get; set; }

        [Column("LatestElectricityBill")]
        public byte[] LatestElectricityBill { get; set; }

        [Column("RoofPhotograph")]
        public byte[] RoofPhotograph { get; set; }

        [Column("SubmissionDate")]
        public DateTime SubmissionDate { get; set; }
        [Column("ApplicantIdProof")]
        public byte[] ApplicantIdProof { get; set; }
    }


    [Table("RescoApplications")]
    public class RescoApplication
    {
        [Column("DiscomName")]
        public string DiscomName { get; set; }

        [Column("ServiceConnectionNumber")]
        public string ServiceConnectionNumber { get; set; }

        [Column("ConsumerName")]
        public string ConsumerName { get; set; }

        [Column("PhaseType")]
        public string PhaseType { get; set; }

        [Column("SanctionLoad")]
        public decimal? SanctionLoad { get; set; }

        [Column("SiteAddress")]
        public string SiteAddress { get; set; }

        [Column("SiteSector")]
        public string SiteSector { get; set; }

        [Column("SitePinCode")]
        public string SitePinCode { get; set; }

        [Column("ProposedSPVPlantCapacity")]
        public decimal? ProposedSPVPlantCapacity { get; set; }

        [Column("MonthlyConsumption")]
        public string MonthlyConsumption { get; set; }

        [Column("AnnualConsumption")]
        public string AnnualConsumption { get; set; }

        [Column("EarlierInstalledSPVPlantCapacity")]
        public string EarlierInstalledSPVPlantCapacity { get; set; }

        [Column("IsApplicationUnderProcess")]
        public bool? IsApplicationUnderProcess { get; set; }

        [Column("ApplicationSPVPlantCapacity")]
        public string ApplicationSPVPlantCapacity { get; set; }

        [Column("ApplicationNo")]
        public string ApplicationNo { get; set; }

        [Column("ApplicantDoorNo")]
        public string ApplicantDoorNo { get; set; }

        [Column("ApplicantSector")]
        public string ApplicantSector { get; set; }

        [Column("ApplicantPinCode")]
        public string ApplicantPinCode { get; set; }

        [Column("ApplicantName")]
        public string ApplicantName { get; set; }

        [Column("ApplicantPrimaryContactName")]
        public string ApplicantPrimaryContactName { get; set; }

        [Column("ApplicantMobileNumber")]
        public string ApplicantMobileNumber { get; set; }

        [Column("ApplicantPhoneNumber")]
        public string ApplicantPhoneNumber { get; set; }

        [Column("ApplicantEmail")]
        public string ApplicantEmail { get; set; }

        [Column("ApplicantIDProofType")]
        public string ApplicantIDProofType { get; set; }

        [Column("ApplicantAadharNo")]
        public string ApplicantAadharNo { get; set; }

        [Column("ApplicantPanCardNo")]
        public string ApplicantPanCardNo { get; set; }

        [Column("DeclarationPlace")]
        public string DeclarationPlace { get; set; }

        [Column("ApplicantPhotoPath")]
        public string ApplicantPhotoPath { get; set; }

        [Column("ApplicantSignaturePath")]
        public string ApplicantSignaturePath { get; set; }

        [Column("LatestElectricityBillPath")]
        public string LatestElectricityBillPath { get; set; }

        [Column("RoofPhotographPath")]
        public string RoofPhotographPath { get; set; }

        [Column("SubmissionDate")]
        public DateTime? SubmissionDate { get; set; }

        [Key]
        [Column("Id")]
        public int Id { get; set; }

        [NotMapped]
        public string UniqueId { get { return "RESCO-" + Id.ToString().PadLeft(5, '0'); } }

        [Column("ApplicantIdProofPath ")]
        public string ApplicantIdProofPath { get; set; }
    }
}
