namespace ElectroSphereProj.ViewModel;

public class ProductListViewModel{

    public int producId{get;set;}
    public string productName{get;set;}
    public string companyName{get;set;}
    public string productDescription{get;set;}
    public decimal productPrice{get;set;}
    public string productImgUrl{get;set;}

}

public class DashBoardView{
    public List<ProductListViewModel> productListView{get;set;}
    public int cartCount{get;set;}
}