using Hotel.Data;
using Hotel.DTO;
using Microsoft.AspNetCore.Identity;

namespace Hotel.Service
{
    public interface IAcountService
    {
        Task<IdentityResult> Add(SingUpDTO DTO);

        Task<SignInResult> SingIn(SingInDTO singIn);

        Task<IdentityResult> AddRole(RoleDTO role);

        Task<IList<string>> GetUserRole(string username);

        Task<IdentityResult> DeleteUser(string ID);
        Task<IdentityResult> UpdateUser(SingUpDTO updateDTO);
        Task<List<IdentityRole>> GetRoles();

        Task<IdentityResult> updateUserRole(string userId, string roleName);

        Task<List<SingUpDTO>> GetAllUsers();
        Task<int> UserCount();
    }
}