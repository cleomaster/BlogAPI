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
   [Authorize]
   public async Task<ActionResult<List<Post>>> getPosts()
   {
      var posts = await _context.Posts.ToListAsync();
      return Ok(posts);
   }

   [HttpPost]
   public async Task<ActionResult<Post>> createPost([FromBody] PostDTO request)
   {
      var post = new Post(request.title, request.description);
      // Post.Posts.Add(post);
      await _context.Posts.AddAsync(post);
      await _context.SaveChangesAsync();
      return Ok(post);
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
   public async Task<ActionResult<Post>> deletePost(int Id)
   {
     // var post = Post.Posts.FindAll(u => u.Id == Id).FirstOrDefault();
     var post = await _context.Posts.FindAsync(Id);
     if (post == null) return BadRequest();
     _context.Posts.Remove(post);
     await _context.SaveChangesAsync();
      return Ok(post);
   }

   [HttpPost]
   [Route("/update")]
   public async Task<ActionResult<Post>> updatePost(int Id, PostDTO request)
   {
      // var post = Post.Posts.FindAll(u => u.Id == Id).FirstOrDefault();
      var post = await _context.Posts.FindAsync(Id);
      if (post == null) return BadRequest();
      post.description = request.description;
      post.title = request.title;
      _context.Posts.Update(post);
      await _context.SaveChangesAsync();
      return Ok(post);
   }



}