using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Salao.Repository.Models;
using Salao.Repository.Models.Menus.View;
using Salao.Repository.Models.Usuario.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
