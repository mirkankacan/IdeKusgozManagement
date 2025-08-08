using IdeKusgozManagement.Application.DTOs.UserDTOs;
using IdeKusgozManagement.Domain.Entities;
using Mapster;

namespace IdeKusgozManagement.Application.Mappings
{
    public static class UserMappingConfig
    {
        public static void Configure()
        {
            // ApplicationUser -> UserDTO
            TypeAdapterConfig<ApplicationUser, UserDTO>
                .NewConfig()
                .Map(dest => dest.Role, src => (string?)null); // Infrastructure'da manuel set edilecek

            // CreateUserDTO -> ApplicationUser
            TypeAdapterConfig<CreateUserDTO, ApplicationUser>
                .NewConfig()
                .Map(dest => dest.Email, src => src.UserName) // Username = Email
                .Map(dest => dest.IsActive, src => true)
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.PasswordHash)
                .Ignore(dest => dest.SecurityStamp)
                .Ignore(dest => dest.ConcurrencyStamp)
                .Ignore(dest => dest.RefreshToken)
                .Ignore(dest => dest.RefreshTokenExpires);

            // UpdateUserDTO -> ApplicationUser
            TypeAdapterConfig<UpdateUserDTO, ApplicationUser>
                .NewConfig()
                .Map(dest => dest.Email, src => src.UserName) // Username = Email
                .Ignore(dest => dest.Id)
                .Ignore(dest => dest.PasswordHash)
                .Ignore(dest => dest.SecurityStamp)
                .Ignore(dest => dest.ConcurrencyStamp)
                .Ignore(dest => dest.RefreshToken)
                .Ignore(dest => dest.RefreshTokenExpires);
        }
    }
}