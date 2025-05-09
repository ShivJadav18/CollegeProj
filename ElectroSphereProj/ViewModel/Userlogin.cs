
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ElectroSphereProj.ViewModel
{
    public class Userlogin
    {
        [RegularExpression("[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}", ErrorMessage = "Please Enter Proper Email.")]
        [Required(ErrorMessage = "Please Enter Valid Email.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Please Enter Valid Password.")]
        public string Password { get; set; }
        public bool rememberme { get; set; }
    }

    public class registerView
    {
        [RegularExpression("[a-z0-9._%+-]+@[a-z0-9.-]+\\.[a-z]{2,4}", ErrorMessage = "Please Enter Proper Email.")]
        [Required(ErrorMessage = "Please Enter Valid Email.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter Valid Password.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$",
        ErrorMessage = "Password must be at least 8 characters long and include uppercase, lowercase, number, and special character.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please Enter FirstName.")]
        public string Firstname { get; set; }

        [Required(ErrorMessage = "Please Enter LastName.")]
        public string Lastname { get; set; }

        [Required(ErrorMessage = "The Contactnumber can not be empty.")]
        [RegularExpression(@"^[0-9]{10}", ErrorMessage = "Contactnumber can only contain numbers and must be have length of 10.")]
        public string Contactnumber { get; set; }
    }
}