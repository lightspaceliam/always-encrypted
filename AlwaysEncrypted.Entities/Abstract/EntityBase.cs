using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace AlwaysEncrypted.Entities
{
    [DataContract]
    public abstract class EntityBase : IEntity
    {
        [DataMember]
        [Display(Name = "Id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DataMember]
        [Display(Name = "Created")]
        public DateTime Created { get; set; }

        [DataMember]
        [Display(Name = "Last Modified")]
        public DateTime LastModified { get; set; }
    }
}
