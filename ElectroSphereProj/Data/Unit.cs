﻿using System;
using System.Collections.Generic;

namespace ElectroSphereProj.Data;

public partial class Unit
{
    public int UnitId { get; set; }

    public string? Name { get; set; }

    public string? Shortname { get; set; }

    public DateTime? Createdat { get; set; }

    public DateTime? Updatedat { get; set; }

    public int? Createdby { get; set; }

    public int? Updatedby { get; set; }

    public virtual User? CreatedbyNavigation { get; set; }

    public virtual User? UpdatedbyNavigation { get; set; }
}
