using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SkietbaanBE.Models;

namespace SkietbaanBE.Controllers
{
    [Produces("application/json")]
    [Route("api/VersionNumber")]
    public class VersionNumberController : Controller
    {
        private ModelsContext context;
        public VersionNumberController(ModelsContext context) {
            this.context = context;
        }

        [HttpGet]
        public IActionResult GetVersionNumber() {
            if (!ModelState.IsValid) return new BadRequestObjectResult("Model state is not valid");
            try {
                //TODO: create a relevent table to use for versionNumber
                var versionNumber = context.OTPs.FirstOrDefault(x => x.OneTimePassword == x.OneTimePassword);
                if (versionNumber == null) return new NotFoundObjectResult("No record in the database");
                return new OkObjectResult(versionNumber.OneTimePassword);
            } catch (Exception) {
                return new NotFoundObjectResult("Error getting version number");
            }
        }

        [HttpPost("{versionNumber}")]
        public IActionResult PostVersion(int versionNumber) {
            if (!ModelState.IsValid) return new BadRequestObjectResult("Model state is not valid");
            try {
                OTP dbOTP = context.OTPs.FirstOrDefault(x => x.OneTimePassword == x.OneTimePassword);
                if (dbOTP != null && dbOTP.OneTimePassword != 0) {
                    if (dbOTP.OneTimePassword == versionNumber) return new OkObjectResult("Version number is up-to-date");
                    dbOTP.OneTimePassword = versionNumber;
                    context.OTPs.Update(dbOTP);
                } else {
                    OTP otp = new OTP {
                        OneTimePassword = versionNumber,
                        UserId = 1
                    };
                    context.OTPs.Add(otp);
                }
                context.SaveChangesAsync();
                return new OkObjectResult("Version changed");
            } catch (Exception) {
                return new BadRequestObjectResult("Could not change version number");
            }
        }
    }
}