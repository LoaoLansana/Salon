using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Salao.Repository.Models;
using Salao.Repository.Models.Menus.View;
using Salao.Repository.Models.Usuario.Views;

namespace Salao.Repository.Profiles
{
    public class SalaoProfile : Profile
    {
        public SalaoProfile()
        {
            CreateMap<MenuViewModel, Menu>();
            CreateMap<Menu, MenuViewModel>();

            CreateMap<UsuarioViewModel, IdentityUser>();
            CreateMap<IdentityUser, UsuarioViewModel>();
        }
    }
}
