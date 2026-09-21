namespace Application.Common.Identity;

public class IdentityUserResult
{
    public IdentityUserResult(string id , string email ,  string displayName , string userName)
    {
        Id = id;
        Email = email;
        DisplayName = displayName;
        UserName = userName;

    }

    public string Id { get; set; }
    public string DisplayName { get; set; } 
    public string Email { get; set; } 
    public string UserName { get; set; }
}