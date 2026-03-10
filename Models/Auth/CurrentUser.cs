namespace CarsInsideGarage.Models.Auth
{
    public class CurrentUser
    {
        public string UserId { get; set; } = string.Empty;
        public bool IsAdmin { get; set; }
        public bool IsOwner { get; set; }
        public bool IsDriver { get; set; }

        public bool IsInRole(string roleName)
        {
            return roleName switch
            {
                "Admin" => IsAdmin,
                "GarageOwner" => IsOwner,
                "Driver" => IsDriver,
                _ => false
            };
        }
    }

}
