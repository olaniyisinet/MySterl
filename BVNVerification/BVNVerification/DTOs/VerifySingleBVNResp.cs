using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BVNVerification.DTOs
{
    public class VerifySingleBVNResp
    {

        public string ResponseCode { get; set; }
        public string BVN { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string PhoneNumber1 { get; set; }
        public string RegistrationDate { get; set; }
        public string EnrollmentBank { get; set; }
        public string EnrollmentBranch { get; set; }
        public string WatchListed { get; set; }

    }
}