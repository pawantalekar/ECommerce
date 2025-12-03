using System;
using System.Collections.Generic;

namespace Ecom.Domain.Entities;

public partial class SellerRequest
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Status { get; set; } = null!;

    public DateTime RequestedAt { get; set; }

    public Guid? ApprovedById { get; set; }

    public DateTime? ApprovedAt { get; set; }
    public virtual User User { get; set; } = null!;
    public virtual User? ApprovedBy { get; set; }

}
