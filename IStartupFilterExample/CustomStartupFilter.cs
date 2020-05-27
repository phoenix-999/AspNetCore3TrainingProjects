using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IStartupFilterExample
{
    public class CustomStartupFilter : IStartupFilter
    {
        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                builder.UseMiddleware<SomeMiddleware>();//Начало конвеера приложения
                next(builder);//Запуск следующего метода Configure
                //builder.UseMiddleware<SomeMiddleware>();//Конец конвеера приложения. Может быть не вызвано, если запрос будет обработан раньше в конвеере
            };
        }
    }
}
