namespace ElectroSphereProj.ViewModel;
public class CartViewModel{
    public decimal Totalamount{get;set;}
    public int totalItems{get;set;}
    public List<cartItem> cartItems{get;set;}
}

public class cartItem{
    public string itemImg{get;set;}
    public int itemId{get;set;}
    public string itemName{get;set;}
    public decimal itemPrice{get;set;}
    public int itemQun{get;set;}
}