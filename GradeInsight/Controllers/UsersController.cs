using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GradeInsight.Data;
using GradeInsight.Model;
using GradeInsight.Utilities;
using GradeInsight.SpecificRepositories.UserRepositories.cs;
using NuGet.DependencyResolver;

namespace GradeInsight.Controllers
{
    
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly GradeInsightContext _context;
        private readonly IUserRepositories _userRepositories;

        public UsersController(GradeInsightContext context,IUserRepositories userRepositories)
        {
            _context = context;
            _userRepositories = userRepositories;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUser()
        {
            return await _context.User.ToListAsync();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.User.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/Users/5
     
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            var existingUser = await _context.User.FindAsync(id);
            if (existingUser == null)
            {
                return NotFound();
            }

       
         
           
            var getUserByemail = await _userRepositories.GetUserDetailFromUserEmail(existingUser.UserEmail);

            if (getUserByemail != null)
            {
                if (getUserByemail.UserId != id)
                {
                    return BadRequest("User with Email already exists. Choose another Email!");
                }
            }

            Request.Headers.TryGetValue("userPassword", out var userPasswordFromHeader);
            if (!string.IsNullOrEmpty(userPasswordFromHeader.ToString()))
            {
                string hashPasswordResult = HashingAndVerification.ComputeHash(userPasswordFromHeader.ToString(), HashingAndVerification.Supported_HA.SHA256, null);
                existingUser.UserPassword = hashPasswordResult;
            }
            existingUser.UserName = user.UserName;
            existingUser.UserEmail = user.UserEmail;
            existingUser.UserFullName = user.UserFullName;
            // Update only the fields that are provided



            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

         


            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<User>> PostUser(User user)
        {

         
            var getUserByemail = await _userRepositories.GetUserDetailFromUserEmail(user.UserEmail);
            if (getUserByemail != null || getUserByemail?.UserEmail == user.UserEmail)
            {
                return BadRequest("User Name is already taken. Choose another User Name!");
            }

            Request.Headers.TryGetValue("userPassword", out var userPasswordFromHeader);
            if (!string.IsNullOrEmpty(userPasswordFromHeader.ToString()))
            {
                string hashPasswordResult = HashingAndVerification.ComputeHash(userPasswordFromHeader.ToString(), HashingAndVerification.Supported_HA.SHA256, null);
                user.UserPassword = hashPasswordResult;
            }

            user.DateCreated = DateTime.Now;


            _context.User.Add(user);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUser", new { id = user.UserId }, user);
        }
        [HttpGet]
        [Route("~/api/authLogin")]
        public async Task<ActionResult<User>> GetAuthLogin()
        {

            Request.Headers.TryGetValue("userEmail", out var userEmail);
            Request.Headers.TryGetValue("userPassword", out var userPassword);
            var userDetail = await _userRepositories.GetUserDetailFromUserEmail(userEmail.ToString());
            if (userDetail == null)
            {
                const string result = "INVALID_USER";
                return BadRequest(result);
            }
           
            else
            {
                bool confirmResult = HashingAndVerification.Confirm(userPassword.ToString(), userDetail.UserPassword, HashingAndVerification.Supported_HA.SHA256);
                if (confirmResult)
                {
                   
                    return Ok(userDetail);
                }
                else
                {
                    const string result = "INVALID_CREDENTIALS";
                    return BadRequest(result);
                }
            }
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.User.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.User.Any(e => e.UserId == id);
        }
    }
}
