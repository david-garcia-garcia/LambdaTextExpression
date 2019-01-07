using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LambdaTextExpressionTests.Models
{
    // CORE_USER
    [Table("CORE_USER", Schema = "dbo")]
    [System.CodeDom.Compiler.GeneratedCode("EF.Reverse.POCO.Generator", "2.31.0.0")]
    public class CORE_USER
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column(@"id", Order = 1, TypeName = "uniqueidentifier")]
        [Required]
        [Key]
        [Display(Name = "Id")]
        public System.Guid id { get; set; } // id (Primary key)

        [Column(@"name", Order = 2, TypeName = "nvarchar")]
        [MaxLength(125)]
        [StringLength(125)]
        [Display(Name = "Name")]
        public string name { get; set; } // name (length: 125)

        // This status is here since the POC, but it isnt used anywhere
        // TODO: Find what the user.status is supposed to be
        [Column(@"status", Order = 3, TypeName = "smallint")]
        [Display(Name = "Status")]
        public short? status { get; set; } // status

        [Column(@"mail", Order = 4, TypeName = "nvarchar")]
        [MaxLength(255)]
        [StringLength(255)]
        [Display(Name = "Mail")]
        public string mail { get; set; } // mail (length: 255)

        [Column(@"lastauth", Order = 5, TypeName = "bigint")]
        [Display(Name = "Lastauth")]
        public long? lastauth { get; set; } // lastauth

        [Column(@"authcount", Order = 6, TypeName = "bigint")]
        [Display(Name = "Authcount")]
        [Required]
        public long authcount { get; set; } // authcount

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        [Column(@"authtoken", Order = 7, TypeName = "nvarchar")]
        [MaxLength(36)]
        [StringLength(36)]
        [Display(Name = "Authtoken")]
        public string authtoken { get; set; } // authtoken (length: 36)

        [Column(@"changedAt", Order = 8, TypeName = "bigint")]
        [Display(Name = "changedAt")]
        public long? changedAt { get; set; } // lastupdatedate

        [Column(@"deletedAt", Order = 9, TypeName = "bigint")]
        [Display(Name = "deletedAt")]
        public long? deletedAt { get; set; } // deletedAt

        /// <summary>
        /// Reverse collection
        /// </summary>
        public ICollection<CORE_PERSON> CORE_PERSONS { get; set; }

        public CORE_USER()
        {
        }
    }
}
