// <copyright file="UserMapProfile.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Users.Dto
{
    using AcmStatisticsAbp.Authorization.Users;
    using AutoMapper;

    public class UserMapProfile : Profile
    {
        public UserMapProfile()
        {
            this.CreateMap<UserDto, User>();
            this.CreateMap<UserDto, User>().ForMember(x => x.Roles, opt => opt.Ignore());

            this.CreateMap<CreateUserDto, User>();
            this.CreateMap<CreateUserDto, User>().ForMember(x => x.Roles, opt => opt.Ignore());
        }
    }
}
