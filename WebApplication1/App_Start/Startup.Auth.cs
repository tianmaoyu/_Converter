using Microsoft.Owin.Security.Cookies;
using Owin;

namespace AClassroom.DocConverter
{
    public partial class Startup
    {
       
        public static string PublicClientId { get; private set; }

        // 有关配置身份验证的详细信息，请访问 http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // 将数据库上下文和用户管理器配置为对每个请求使用单个实例
          

            // 使应用程序可以使用 Cookie 来存储已登录用户的信息
            // 并使用 Cookie 来临时存储有关使用第三方登录提供程序登录的用户的信息
            app.UseCookieAuthentication(new CookieAuthenticationOptions());
         

            
          
        }
    }
}
