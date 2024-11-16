using Hotel.DTO;
using Hotel.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Hotel.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "admin")]
    public class AccountController : ControllerBase
    {
        private readonly IAcountService acountService;
        private readonly IRoomService roomService;
        private readonly IGuestService guestService;
        private readonly IBookService bookService;
        private readonly IInvoiceService invoiceService;

        public AccountController(IAcountService _acountService, IRoomService _roomService  , IGuestService _guestService , IBookService _bookService , IInvoiceService _invoiceService)
        {
            acountService = _acountService;
            roomService = _roomService;
            guestService = _guestService;
            bookService = _bookService;
            invoiceService = _invoiceService;
        }

        [HttpPost]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> SingUp(SingUpDTO dto)
        {
            try
            {
                var result = await acountService.Add(dto);

                if (result.Succeeded)
                {

                    return Ok();
                    //return NotFound("not work");
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, result.Errors);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            //return BadRequest("");
        }

        [HttpPost]
        [Route("login")]
        [AllowAnonymous]
        public async Task<IActionResult> SignIn(SingInDTO signin)
        {

            var result = await acountService.SingIn(signin);
            if (result.Succeeded)
            {
                List<Claim> authClaim = new List<Claim>()
                {
                    new Claim(ClaimTypes.Name ,signin.Username),
                    new Claim("UniqueValue",Guid.NewGuid().ToString())
                };

                var roles = await acountService.GetUserRole(signin.Username);
                foreach (var role in roles)
                {
                    authClaim.Add(new Claim(ClaimTypes.Role, role.ToString()));
                }

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("5ea0b74f736081455e758a02bf2fdbb8ae94900a0b28fcdb3713eea1acefe02b"));

                var token = new JwtSecurityToken(
                            issuer: "http://localhost",
                            audience: "User",
                            expires: DateTime.Now.AddDays(15),
                            claims: authClaim,
                            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                            );
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token)
                });
            }
            //else
            //{
            //    return StatusCode((int)HttpStatusCode.BadRequest, result.Errors);
            //}
            else
            {
                return Unauthorized();
            }
        }

        [HttpPost]
        [Route("AddRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddRole(RoleDTO role)
        {
            try {
                var result = await acountService.AddRole(role);

                if (result.Succeeded)
                {
                    return StatusCode((int)HttpStatusCode.OK, "Role added ");
                    //return NotFound("not work");
                }
                else
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, result.Errors);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        [Route("deleteUser{username}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> deleteUser(string username)
        {
            try
            {
                var resulte = await acountService.DeleteUser(username);

                if (!resulte.Succeeded)
                {
                    return NotFound(new { message = $" User not found" });
                }
                return Ok(new { Message = $"User '{username}' deleted successfully." });
            }
            catch (Exception ex) {
                
                return StatusCode(500,new { message = "An error occurred while processing your request. Please try again later." });
            }

        }

        [HttpPut]
        [Route("updateUser")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUser(SingUpDTO updateDTO)
        {
           var resulte = await acountService.UpdateUser(updateDTO);

            if (resulte.Succeeded)
            {
                return Ok(resulte);
            }
            else
            {
                return BadRequest(resulte.Errors);
            }
        }

        [HttpPut]
        [Route("updateUserRole")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateUserRole(string userId ,string roleName)
        {
            var resulte = await acountService.updateUserRole(userId,roleName);

            if (resulte.Succeeded)
            {
                return Ok(resulte);
            }
            else
            {
                return BadRequest(resulte.Errors);
            }
        }

        [HttpGet("getroles")]
        public async Task<ActionResult<List<IdentityRole>>> GetRoles()
        {
            var roles = await acountService.GetRoles();

            if (roles == null || !roles.Any())
            {
                return NotFound("No roles found.");
            }

            return Ok(roles);
        }

        [HttpGet("getUserRoles/{username}")]
        public async Task<IList<string>> GetUserRole(string username)
        {
            return await acountService.GetUserRole(username);
        }

        [HttpGet("GetAllUsers")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {

            var resulte = await acountService.GetAllUsers();

            if (resulte != null)
            {
                return Ok(resulte);
            }
            else
            {
                return BadRequest(resulte);
            }
        }

        [HttpGet("GetStatistics")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetStatistics()
        {

            int UserCount = await acountService.UserCount();
            int RoomCount = await roomService.RoomCount();
            int GuestCount = await guestService.GuestCount();
            int BookCount= await bookService.BookCount();
            double TotalIncome=await invoiceService.TotalIncome();

            var stat = new
            {
                UserCount = UserCount,
                RoomCount = RoomCount,
                GuestCount = GuestCount,
                BookCount = BookCount,
                TotalIncome = TotalIncome
            };

            return Ok(stat);
        }

    }
}
