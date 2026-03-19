using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace KOL_KOC_TAAA.Models;

public partial class File
{
    [Key]
    public Guid Id { get; set; }

    public Guid UploaderUserId { get; set; }

    public string Url { get; set; } = null!;

    [StringLength(120)]
    public string? MimeType { get; set; }

    public long? SizeBytes { get; set; }

    [StringLength(128)]
    public string? Checksum { get; set; }

    public DateTime CreatedAt { get; set; }

    [InverseProperty("Attachment")]
    public virtual ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();

    [ForeignKey("UploaderUserId")]
    [InverseProperty("Files")]
    public virtual User UploaderUser { get; set; } = null!;

    [ForeignKey("FileId")]
    [InverseProperty("Files")]
    public virtual ICollection<Deliverable> Deliverables { get; set; } = new List<Deliverable>();
}
