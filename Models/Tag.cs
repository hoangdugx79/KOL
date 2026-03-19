using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

[Index("Name", Name = "UQ__Tags__737584F657C1C0A3", IsUnique = true)]
public partial class Tag
{
    [Key]
    public long Id { get; set; }

    [StringLength(80)]
    public string Name { get; set; } = null!;

    [ForeignKey("TagId")]
    [InverseProperty("Tags")]
    public virtual ICollection<KolProfile> KolUsers { get; set; } = new List<KolProfile>();
}
