using System.Security.Claims;
using BlogAPI.Models;
using BlogAPI.Models.Dto;
using BlogAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlogAPI.Controllers;

[ApiController]
[Route("/api/posts")]
public class PostController : ControllerBase
{

   private AppDBContext _context;

   public PostController(AppDBContext context)
   {
      _context = context;
   }
   
   [HttpGet]
   public async Task<ActionResult<List<Post>>> getPosts()
   {
      var posts = await _context.Posts.ToListAsync();
      return Ok(posts);
   }

   [HttpPost]
   [Authorize]
   public async Task<ActionResult<Post>> createPost([FromBody] PostDTO request)
   {
      var post = new Post(request.title, request.description);
      var user = await _context.Users.FindAsync(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
      user.Posts.Add(post);
     // post.user = user;
     // await _context.Posts.AddAsync(post);
      await _context.SaveChangesAsync();
      return Ok(user);
   }

   [HttpGet("Id")]
   public async Task<ActionResult<Post>> getPostById(int Id)
   {
     // var post = Post.Posts.FindAll(u => u.Id == Id).FirstOrDefault();
      var post = await _context.Posts.FindAsync(Id);
      if (post == null) return BadRequest();
      return Ok(post);
   }

   [HttpPost("Id")]
   [Authorize]
   public async Task<ActionResult<Post>> deletePost(int Id)
   {
     // var post = Post.Posts.FindAll(u => u.Id == Id).FirstOrDefault();
     var post = await _context.Posts.FindAsync(Id);
     if (post == null) return BadRequest();
     var user = await _context.Users.FindAsync(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
     if(post.user.Id != user.Id) return BadRequest();
      // _context.Posts.Remove(post);
       user.Posts.Remove(post);
     await _context.SaveChangesAsync();
      return Ok(post);
   }

   [HttpPost]
   [Route("/update")]
   [Authorize]
   public async Task<ActionResult<Post>> updatePost(int Id, PostDTO request)
   {
      // var post = Post.Posts.FindAll(u => u.Id == Id).FirstOrDefault();
      var post = await _context.Posts.FindAsync(Id);
      if (post == null) return BadRequest();
      var user = await _context.Users.FindAsync(int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)));
      if(post.user.Id != user.Id) return BadRequest();
      post.description = request.description;
      post.title = request.title;
      _context.Posts.Update(post);
      await _context.SaveChangesAsync();
      return Ok(post);
   }



}