namespace ProjectBackend.api.Models.Domain
{
    public enum UserRole
    {
        Guest = 0,
        User = 1,
        Customer = User,
        Admin = 2
    }
}
