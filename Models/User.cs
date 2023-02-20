using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BlogAPI.Models;

public class User
{

    public User()
    {
        Posts = new List<Post>();
    }
    
    [Key] 
    public int Id { get; set; }
    public string username { get; set; }
    public byte[] passwordHash { get; set; }
    public byte[] passwordSalt { get; set; }
    
    [JsonIgnore]
    public List<Post> Posts { get; set; }
}