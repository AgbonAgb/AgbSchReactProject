using ApplicationCore.DTO.Request;
using AutoMapper;
using Infrastructure.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Utility
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            CreateMap<AdminRegistration, UserRegistrationDto>().ReverseMap();
        }
    }
}
