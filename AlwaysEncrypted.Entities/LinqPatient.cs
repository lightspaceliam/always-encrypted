using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace AlwaysEncrypted.Entities
{
    public class LinqPatient : EntityBase
    {
        [DataMember]
        [Display(Name = "Ssn")]
        [Required(ErrorMessage = "Ssn is required.")]
        [StringLength(11, ErrorMessage = "Ssn exceeds {1} characters.")]
        //[Column(TypeName = "char(11)")]
        public string Ssn { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(100, ErrorMessage = "First name exceeds {1} characters.")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(100, ErrorMessage = "Last name exceeds {1} characters.")]
        public string LastName { get; set; }

        [Display(Name = "Birth Date")]
        [Required(ErrorMessage = "Birth date is required.")]
        //[Column(TypeName = "date")]
        public DateTime BirthDate { get; set; }
    }
}
