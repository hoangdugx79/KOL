using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

[Index("Code", Name = "UQ__Roles__A25C5AA7A32F6946", IsUnique = true)]
public partial class Role
{
    [Key]
    public long Id { get; set; }

    [StringLength(50)]
    public string Code { get; set; } = null!;

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [ForeignKey("RoleId")]
    [InverseProperty("Roles")]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
