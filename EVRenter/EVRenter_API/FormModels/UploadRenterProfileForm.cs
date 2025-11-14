using Microsoft.AspNetCore.Mvc;

public class UploadRenterProfileForm
{
    [FromForm(Name = "renterId")]
    public int RenterId { get; set; }

    [FromForm(Name = "idNumber")]
    public string IDNumber { get; set; }

    [FromForm(Name = "driverLicenseNo")]
    public string DriverLicenseNo { get; set; }

    [FromForm(Name = "idFront")]
    public IFormFile IDFront { get; set; }

    [FromForm(Name = "idBack")]
    public IFormFile IDBack { get; set; }

    [FromForm(Name = "dlFront")]
    public IFormFile DLFront { get; set; }

    [FromForm(Name = "dlBack")]
    public IFormFile DLBack { get; set; }
}
