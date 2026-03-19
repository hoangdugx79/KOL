using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

[Keyless]
public partial class ViewPlatformRevenue
{
    [StringLength(4000)]
    public string? RevenueMonth { get; set; }

    [Column(TypeName = "decimal(38, 2)")]
    public decimal? TotalPlatformFee { get; set; }

    public int? TotalBookings { get; set; }
}
