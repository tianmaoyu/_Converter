using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
namespace AClassroom.DocConverter
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
