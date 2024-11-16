using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hotel.Data
{
    public class ApplicationUser : IdentityUser
    {

        [StringLength(100)]
        public string Name { get; set; }

        //[Column(TypeName ="date")]
        //public DateTime DOB {  get; set; }
    }
}
