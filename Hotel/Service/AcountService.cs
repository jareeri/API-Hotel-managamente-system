using Hotel.Data;
using Hotel.DTO;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Service
{
    public class AcountService: IAcountService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AcountService(UserManager<ApplicationUser> _userManager,
                             SignInManager<ApplicationUser> _signInManager,
                             RoleManager<IdentityRole> _roleManager   )
        {
            userManager = _userManager;
            signInManager = _signInManager;
            roleManager = _roleManager;
        }

        public async Task<IdentityResult> Add(SingUpDTO DTO) 
        {
            bool issucceed = true;

            ApplicationUser user = new ApplicationUser()
            {
                Name = DTO.Name,
                Email = DTO.Email,
                //DOB = DTO.DOB,
                UserName = DTO.Username,

            };
            var result = await userManager.CreateAsync(user , DTO.Password);
            if (result.Succeeded)
            {
                var addrole = await userManager.AddToRoleAsync(user, DTO.RoleName);
                if (!addrole.Succeeded)
                {
                    await userManager.DeleteAsync(user);

                    throw new Exception("hello world");
                    issucceed = false;
                }
            }
            
            else
                issucceed = false;
            return result;

        }

        public async Task<SignInResult> SingIn(SingInDTO singIn)
        {
            //var user =  await userManager.FindByNameAsync(singIn.Username);
            //if (user == null)
            //{
            //    throw new ApplicationException("user not found");
            //}
            return await signInManager.PasswordSignInAsync(singIn.Username , singIn.Password, false, false);
        }

        public async Task<IdentityResult> AddRole(RoleDTO role)
        {
            IdentityRole identity = new IdentityRole()
            {
                Name = role.Name
            };
            return await roleManager.CreateAsync(identity);
        }

        public async Task<IList<string>> GetUserRole(string username)
        {
            var user = await userManager.FindByNameAsync(username);

            return await userManager.GetRolesAsync(user);

        }

        

        public async Task<IdentityResult> UpdateUser(SingUpDTO updateDTO)
        {
            // Find the existing user by username
            var user = await userManager.FindByIdAsync(updateDTO.Id);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Retrieve the user's current roles
            var userRoles = await userManager.GetRolesAsync(user);
            if (userRoles.Any())
            {
                // Remove the user from their current role (assuming one role here)
                var removeRoleResult = await userManager.RemoveFromRoleAsync(user, userRoles[0]);
                if (!removeRoleResult.Succeeded)
                {
                    throw new Exception($"Failed to remove role: {string.Join(", ", removeRoleResult.Errors.Select(e => e.Description))}");
                }
            }

            // Add the user to the new role
            var addRoleResult = await userManager.AddToRoleAsync(user, updateDTO.RoleName);
            if (!addRoleResult.Succeeded)
            {
                throw new Exception($"Failed to add role: {string.Join(", ", addRoleResult.Errors.Select(e => e.Description))}");
            }

            // Update the user's details
            user.Name = updateDTO.Name;
            user.UserName = updateDTO.Username;
            user.Email = updateDTO.Email;

            // Save the user updates
            var updateResult = await userManager.UpdateAsync(user);
            if (!updateResult.Succeeded)
            {
                throw new Exception($"Failed to update user: {string.Join(", ", updateResult.Errors.Select(e => e.Description))}");
            }

            return updateResult;
        }


        public async Task<IdentityResult> updateUserRole(string userId , string roleName)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Retrieve the user's current roles
            var userRoles = await userManager.GetRolesAsync(user);
            if (userRoles.Any())
            {
                // Remove the user from their current role (assuming one role here)
                var removeRoleResult = await userManager.RemoveFromRoleAsync(user, userRoles[0]);
                if (!removeRoleResult.Succeeded)
                {
                    throw new Exception($"Failed to remove role: {string.Join(", ", removeRoleResult.Errors.Select(e => e.Description))}");
                }
            }

            // Add the user to the new role
            var addRoleResult = await userManager.AddToRoleAsync(user, roleName);
            if (!addRoleResult.Succeeded)
            {
                throw new Exception($"Failed to add role: {string.Join(", ", addRoleResult.Errors.Select(e => e.Description))}");
            }
            return addRoleResult;
        }

        public async Task<List<IdentityRole>> GetRoles()
        {
            var roles = await roleManager.Roles.ToListAsync();
            return roles;
        }

        public async Task<List<SingUpDTO>> GetAllUsers()
        {
            var users = await userManager.Users.ToListAsync();
            var userList = new List<SingUpDTO>();

            foreach (var user in users)
            {
                var roles = await userManager.GetRolesAsync(user); // Fetch roles for each user
                userList.Add(new SingUpDTO
                {
                    Id = user.Id,
                    Username = user.UserName,
                    Name = user.Name,
                    Email = user.Email,
                    RoleName = roles.FirstOrDefault() 
                });
            }

            return userList;
        }

        public async Task<IdentityResult> DeleteUser(string username)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                throw new Exception("User not found.");
            }
            return await userManager.DeleteAsync(user);
        }

        public async Task<int> UserCount()
        {
            int count = await userManager.Users.CountAsync();
            return count;
        }

    }
    
}
