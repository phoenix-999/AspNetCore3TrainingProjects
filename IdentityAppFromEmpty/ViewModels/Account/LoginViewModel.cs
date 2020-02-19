using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityAppFromEmpty.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Не введено имя пользователя")]
        [Display(Name ="Имя пользователя")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Не введен пароль")]
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
