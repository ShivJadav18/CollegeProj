using System;
using System.Collections.Generic;

namespace ElectroSphereProj.Data;

public partial class Cart
{
    public int Cartid { get; set; }

    public int? Customerid { get; set; }

    public int? Productid { get; set; }

    public DateTime? Createdat { get; set; }

    public int? Quantity { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Item? Product { get; set; }
}
