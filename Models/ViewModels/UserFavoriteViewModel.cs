namespace KhielsSkincare.Models.ViewModels
{
    public class UserFavoriteViewModel
    {
        public AppUser User { get; set; }
        public List<FavoriteProduct> FavoriteProducts { get; set; }
    }
}
