namespace BlogAPI.Models;

public class User
{
    public string username { get; set; }
    public byte[] passwordHash { get; set; }
    public byte[] passwordSalt { get; set; }
}