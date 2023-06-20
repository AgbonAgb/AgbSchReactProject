using Infrastructure.Accounts;
using ApplicationCore.DTO.Request;
using ApplicationCore.DTO.Response;
using ApplicationCore.Services.Interface;
using ApplicationCore.Utility;
using Infrastructure.Accounts;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;

namespace ApplicationCore.Services.Repository
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
       
        private readonly string key;
        private readonly int TokenExpirationPeriod;
        private readonly SecretKeys _secretKey;
        private readonly UserManager<AdminRegistration> _userManager;//to create user
        private readonly SignInManager<AdminRegistration> _signInManager;//  to signin user
        private readonly RoleManager<AdminRole> _roleManager;
        private readonly ILogger<AuthenticationRepository> _iLogger;
        private readonly IMapper _Mapper;

        public AuthenticationRepository(IOptions<SecretKeys> secretKey, UserManager<AdminRegistration> userManager, SignInManager<AdminRegistration> signInManager, RoleManager<AdminRole> roleManager
            , ILogger<AuthenticationRepository> iLogger, IMapper Mapper)
        {
            // this.key = key;
            _secretKey = secretKey.Value;
            this.key = _secretKey.Secrete.ToString();
            TokenExpirationPeriod = _secretKey.TokenExpirationPeriod;//.Secrete.ToString();

            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _iLogger = iLogger;
            _Mapper = Mapper;
        }
        public async Task<LoginResponseDto> Authenticate(LoginDto LoginDetails)
        {
            //agbonwinn@yahoo.com
            //Agbon01

            //if(!users.Any(u =>u.Key== userName && u.Value== Password))
            // {
            //     return null;
            // }

            //ie, email is correct
            var existedUser = await _userManager.FindByEmailAsync(LoginDetails.Email);
            if (existedUser != null)
            {
                //Then check password for the user
                //var result = await _signInManager.PasswordSignInAsync(existedUser, LoginDetails.password, false, false);

                var isCorect = await _userManager.CheckPasswordAsync(existedUser, LoginDetails.Password);

                //Check id signin is successful
                //if (result.Succeeded)
                if (isCorect)
                {

                    //get the user roll
                    //var userRoles = await _userManager.GetRolesAsync(existedUser);


                    //Generate Token and return
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var tokenKey = Encoding.ASCII.GetBytes(key);//let this be in appsetting

                    var claims = await GetUserClaims(existedUser);
                    
                    //
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(claims),
                        //Expires = System.DateTime.UtcNow.AddMinutes(5),
                        Expires = System.DateTime.UtcNow.AddMinutes(TokenExpirationPeriod),
                        //TokenExpirationPeriod
                        //SignIn Credentials
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)

                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    var JwtOken = tokenHandler.WriteToken(token);

                    return new LoginResponseDto()
                    {
                        Token = JwtOken,
                        Success = true,
                        RefreshToken = ""
                    };



                }
                else
                {
                    return null;
                }

            }

            else
            {
                return null;
            }


        }
        //User Claims
        private async Task<IList<Claim>> GetUserClaims(AdminRegistration user)
        {

            var _options = new IdentityResult();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),// add roles if any
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, new Guid().ToString()),// thie will be rquired later for Token Refresh
                //this returns the role for activities
                //new Claim(ClaimTypes.Role, userRole.First())
            };

            //Get the claims we have assigned the user
            var userClaims = await _userManager.GetClaimsAsync(user);//.ge
            claims.AddRange(userClaims);

            //get the user roll
            var userRoles = await _userManager.GetRolesAsync(user);
            foreach (var userRole in userRoles)
            {

                //reconfirm if role exist
                var role = await _roleManager.FindByNameAsync(userRole);

                if (role != null)
                {
                    claims.Add(new Claim(ClaimTypes.Role, userRole));
                    //get claim assigned to this role
                    var roleClaims = await _roleManager.GetClaimsAsync(role);
                    foreach (var roleClaim in roleClaims)
                    {
                        claims.Add(roleClaim);
                    }
                }
            }

            return claims;
        }
        public async Task<UserRegistrationRespDto> RegisterUser(UserRegistrationDto users)
        {
            var newuser = _Mapper.Map<AdminRegistration>(users);
            newuser.NormalizedEmail = newuser.Email;
            newuser.NormalizedUserName = newuser.UserName;


            //Use this module to create Admin or user
            UserRegistrationRespDto RS = new UserRegistrationRespDto();
            //check if email existed before

            var existed = await _userManager.FindByEmailAsync(users.Email);

            if (existed == null)
            {
                //var newuser = new AdminRegistration()
                //{
                //    UserName = users.UserName,
                //    Email = users.Email
                //};

                //var newuser = new AdminRegistration
                //{
                //    FirstName = users.FirstName,// "Godwin",
                //    LastName = users.LastName,// "Agbon",
                //    Email = users.Email,// "agbonwinn1@yahoo.com",
                //    NormalizedEmail = users.Email.ToUpper(),// "AGBONWINN1@YAHOO.COM",
                //    UserName = users.UserName,// "Godwin",
                //    NormalizedUserName = users.UserName.ToUpper(),// "GODWIN",
                //    PhoneNumber = users.PhoneNumber,// "07034539975",
                //    EmailConfirmed = true,
                //    PhoneNumberConfirmed = true,
                //    SecurityStamp = Guid.NewGuid().ToString("D"),
                //    UserType = "Applicant"

                //};


                var isCreated = await _userManager.CreateAsync(newuser, users.Password);
                if (isCreated.Succeeded)
                {
                    //Add user to default role
                    //string DefaultRole = "Applicant";

                    var roleCheck = await _roleManager.RoleExistsAsync(newuser.UserRole);
                    var role = new AdminRole();
                    role.Name = newuser.UserRole;// "Applicant";
                    if (!roleCheck)
                    {
                        //create the roles and seed them to the database  
                        //await RoleManager.CreateAsync(role.RoleName);
                        await _roleManager.CreateAsync(role);
                    }
                    await _userManager.AddToRoleAsync(newuser, newuser.UserRole);

                    RS.Success = true;
                }
                else
                {
                    RS.Errors = isCreated.Errors.Select(x => x.Description).ToList();
                    RS.Success = false;
                }
            }
            else
            {
                RS.Errors = new List<string>()
                    {
                        "Email already in use"
                    };
                RS.Success = false;


            }

            return RS;
        }
        //public Task<bool> AddClaimToUser(UserClaimDto userClaimDto)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<UserRolesResponse> AddUserRole(UserRoles userRole)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<AuthResult> Authenticate(UserCred LoginDetails)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<bool> CreateRole(ApplicationRole role)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IEnumerable<ExistingRoles>> ExistRoles()
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<IList<Claim>> GetUserClaim(string Email)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<ApplicationRole> GetuserRoles(string Id)
        //{
        //    throw new NotImplementedException();
        //}

        //public Task<RegistrationResponse> RegisterUser(UserRegistrationDto users)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
