using Core.Entities.Data;
namespace AspNetGroupBasedPermissions.ViewModels
{
    public class BranchMasterViewModel
    {
       public string BranchCode {get; set;}
       public string BranchName { get; set; }
       public string AddressLine1 { get; set; }
       public string AddressLine2 { get; set; }
       public string AddressLine3 { get; set; }
       public string City { get; set; }
       public string PostCode { get; set; }
       public int StateCode { get; set; }
       public string Manager { get; set; }
       public string AdminOfficer { get; set; }
       public string ManagerMobileNumber { get; set; }
       public string PhoneNumber { get; set; }
       public string FaxNumber { get; set; }
       public string EmailAddress { get; set; }
       public string GPSLocation { get; set; }
       public string GroupCode { get; set; }
       public int BranchType { get; set; }
       public int RegionalCode { get; set; }
       public string NearestBranch { get; set; }
       public string NearestCPC { get; set; }
       public bool IsActive { get; set; }
    }
}