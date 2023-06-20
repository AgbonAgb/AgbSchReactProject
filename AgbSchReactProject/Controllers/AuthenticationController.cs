using ApplicationCore.DTO.Request;
using ApplicationCore.Services.Interface;
using AutoMapper;
using Infrastructure.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace AgbSchReactProject.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController :ControllerBase
    {
        private readonly IAuthenticationRepository _authenticationRepository;
        private readonly IMapper _Mapper;    
        public AuthenticationController(IAuthenticationRepository authenticationRepository, IMapper Mapper)
        {
            _authenticationRepository = authenticationRepository;
            _Mapper = Mapper;
        }

        [HttpPost("RegisterUser")]
        public async Task<IActionResult> RegisterUser(UserRegistrationDto userreg)
        {
           // var mapper = _Mapper.Map<AdminRegistration>(userreg);


            if(ModelState.IsValid)
            {
                var resp = await _authenticationRepository.RegisterUser(userreg);
                if(resp.Success)
                {
                    return Ok(resp);
                }
                else
                {
                    return Ok(resp);
                }
            
            }
            else
            {
                return BadRequest("Ensure you supplied complete information");
            }
        }
        [HttpPost("Authenticate")]
        public async Task<IActionResult> Authenticate(LoginDto Logindetails)
        {

            if (ModelState.IsValid)
            {
                var resp = await _authenticationRepository.Authenticate(Logindetails);
                if (resp.Success)
                {
                    return Ok(resp.Token);
                }
                else
                {
                    return Ok(resp);
                }

            }
            else
            {
                return BadRequest("Ensure you supplied complete information");
            }
        }
    }
}
