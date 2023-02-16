using System.ComponentModel.DataAnnotations;

namespace BlogAPI.Models;

public class Post
{
    // public static List<Post> Posts = new List<Post>();

    public Post(string title, string description)
    {
        this.title = title;
        this.description = description;
    }
    
    [Key]
    public int Id { get; set; }
    public string title { get; set; }
    public string description { get; set; }
}