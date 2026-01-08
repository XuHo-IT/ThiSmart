using System;
using System.Collections.Generic;

namespace ThiSmartUI.Models;

public partial class Category
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? ParentId { get; set; }

    public int? TeacherId { get; set; }

    public virtual ICollection<Category> InverseParent { get; set; } = new List<Category>();

    public virtual Category? Parent { get; set; }

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    public virtual User? Teacher { get; set; }
}
