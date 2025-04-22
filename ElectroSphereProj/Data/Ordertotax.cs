using System;
using System.Collections.Generic;

namespace ElectroSphereProj.Data;

public partial class Ordertotax
{
    public int Ordertotaxid { get; set; }

    public int? Orderid { get; set; }

    public int? Taxid { get; set; }

    public virtual Order? Order { get; set; }

    public virtual Tax? Tax { get; set; }
}
