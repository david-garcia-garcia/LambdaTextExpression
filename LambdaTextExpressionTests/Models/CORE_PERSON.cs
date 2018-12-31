using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LambdaTextExpressionTests.Models
{
    public class CORE_PERSON
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(@"id", TypeName = "uniqueidentifier")]
        [Required]
        [Key]
        [Display(Name = "id")]
        public System.Guid id { get; set; } // id (Primary key)

        [Column(@"identification", TypeName = "nvarchar")]
        [Required]
        [MaxLength(128)]
        [StringLength(128)]
        [Display(Name = "identification")]
        public string identification { get; set; } // identification (length: 128)

        [Column(@"firstname", TypeName = "nvarchar")]
        [Required]
        [MaxLength(128)]
        [StringLength(128)]
        [Display(Name = "firstname")]
        public string firstname { get; set; } // firstname (length: 128)

        [Column(@"lastname", TypeName = "nvarchar")]
        [Required]
        [MaxLength(128)]
        [StringLength(128)]
        [Display(Name = "lastname")]
        public string lastname { get; set; } // lastname (length: 128)

        [Column(@"secondLastname", TypeName = "nvarchar")]
        [MaxLength(128)]
        [StringLength(128)]
        [Display(Name = "secondLastname")]
        public string secondLastname { get; set; } // secondLastname (length: 128)

        [Column(@"fk_core_user", Order = 5, TypeName = "uniqueidentifier")]
        [Display(Name = "fk_core_user")]
        public System.Guid? fk_core_user { get; set; } // fk_core_user

        [Column(@"mail", TypeName = "nvarchar")]
        [MaxLength(128)]
        [StringLength(128)]
        [Display(Name = "mail")]
        public string mail { get; set; } // mail (length: 128)

        [Column(@"changedAt", TypeName = "bigint")]
        [Display(Name = "changedAt")]
        public long? changedAt { get; set; } // changedAt

        [Column(@"deletedAt", TypeName = "bigint")]
        [Display(Name = "deletedAt")]
        public long? deletedAt { get; set; } // deletedAt

        [Column(@"fk_sabentis_identificationtype", Order = 6, TypeName = "uniqueidentifier")]
        [Display(Name = "fk_sabentis_identificationtype")]
        public System.Guid? fk_sabentis_identificationtype { get; set; } // fk_sabentis_identificationtype

        [Column(@"phonenumber", Order = 7, TypeName = "nvarchar")]
        [MaxLength(128)]
        [StringLength(128)]
        [Display(Name = "phonenumber")]
        public string phonenumber { get; set; } // phonenumber (length: 128)

        [Column(@"communications", Order = 8, TypeName = "bit")]
        [Required]
        [Display(Name = "communications")]
        public bool communications { get; set; } // communications

        [Column(@"language", Order = 9, TypeName = "nvarchar")]
        [MaxLength(5)]
        [StringLength(5)]
        [Display(Name = "language")]
        public string language { get; set; } // language (length: 5)

        [Column(@"birthdate", Order = 10, TypeName = "bigint")]
        [Display(Name = "birthdate")]
        public long? birthdate { get; set; } // birthdate

        [Column(@"gender", Order = 11, TypeName = "smallint")]
        [Display(Name = "gender")]
        public short? gender { get; set; } // gender

        [Column(@"fax", Order = 13, TypeName = "nvarchar")]
        [MaxLength(128)]
        [StringLength(128)]
        [Display(Name = "fax")]
        public string fax { get; set; } // fax (length: 128)

        [Column(@"fk_sabentis_maritalstatus", Order = 14, TypeName = "uniqueidentifier")]
        [Display(Name = "fk_sabentis_maritalstatus")]
        public System.Guid? fk_sabentis_maritalstatus { get; set; } // fk_sabentis_maritalstatus

        [Column(@"fk_sabentis_address", Order = 15, TypeName = "uniqueidentifier")]
        [Display(Name = "fk_sabentis_address")]
        public System.Guid? fk_sabentis_address { get; set; } // fk_sabentis_address

        [Column(@"fk_sabentis_company", Order = 16, TypeName = "uniqueidentifier")]
        [Display(Name = "fk_sabentis_company")]
        public System.Guid? fk_sabentis_company { get; set; } // fk_sabentis_address

        [Column(@"idExternal", Order = 17, TypeName = "nvarchar")]
        [MaxLength(30)]
        [StringLength(30)]
        [Display(Name = "idExternal")]
        public string idExternal { get; set; } // idExternal (length: 20)
    }
}
