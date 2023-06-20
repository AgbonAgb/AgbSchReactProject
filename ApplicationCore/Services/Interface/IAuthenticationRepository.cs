using ApplicationCore.DTO.Request;
using ApplicationCore.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Services.Interface
{
    public interface IAuthenticationRepository
    {

        Task<LoginResponseDto> Authenticate(LoginDto LoginDetails);
        Task<UserRegistrationRespDto> RegisterUser(UserRegistrationDto users);
        //Task<bool> AddClaimToUser(UserClaimDto userClaimDto);
        //Task<UserRolesResponse> AddUserRole(UserRoles userRole);
        //Task<bool> CreateRole(ApplicationRole role);
        //Task<IEnumerable<ExistingRoles>> ExistRoles();
        //Task<ApplicationRole> GetuserRoles(string Id);
        //Task<IList<Claim>> GetUserClaim(string Email);
    }
}
