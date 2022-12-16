using System.Security.Principal;

namespace WebApiDemo.AspNetCustoms
{
    public class LoadUserMiddleware
    {
        readonly RequestDelegate next;

        public LoadUserMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        /// <summary>
        /// 请求拦截处理
        /// </summary>
        /// <param name="context">HTTP请求</param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var clientId = GetValueFromClaims(context, "clientId");
                _ = int.TryParse(GetValueFromClaims(context, "userId"), out int userId);
                var userName = GetValueFromClaims(context, "userName");

                context.Items["ClientId"] = clientId;
                var userinfo = new AuthUserInfo(userId, userName)
                {
                    IpAddress = GetClientIpAddress(context)
                };
                context.User = new GenericPrincipal(userinfo, null);
            }
            await next(context);
        }

        private static string GetValueFromClaims(HttpContext context, string claimKey)
        {
            var claim = context.User.FindFirst(claimKey);
            if (claim != null)
            {
                return claim.Value;
            }
            return string.Empty;
        }

        private static string GetClientIpAddress(HttpContext context)
        {
            return context.Connection.RemoteIpAddress?.ToString() + context.Connection.RemotePort;
        }

        /// <summary>
        /// 用户信息
        /// </summary>
        public class OptUserInfo
        {
            /// <summary>
            /// 用户ID
            /// </summary>
            public int UserId { get; set; }

            /// <summary>
            /// 用户名称
            /// </summary>
            public string UserName { get; set; }

            /// <summary>
            /// 用户组
            /// </summary>
            public string UserGroup { get; set; }

            /// <summary>
            /// IP地址
            /// </summary>
            public string IpAddress { get; set; }

            protected OptUserInfo()
            {

            }

            public OptUserInfo(int userId, string userName)
            {
                UserId = userId;
                UserName = userName;
            }

            public OptUserInfo Clone()
            {
                return (OptUserInfo)MemberwiseClone();
            }

            /// <summary>
            /// 最小权限系统用户
            /// </summary>
            public static readonly OptUserInfo SystemUser;

            public static readonly OptUserInfo UnitTestUser;

            static OptUserInfo()
            {
                SystemUser = new OptUserInfo(0, "系统");
                UnitTestUser = new OptUserInfo(0, "单元测试");
            }
        }

        public class AuthUserInfo : OptUserInfo, IIdentity
        {
            /// <summary>
            /// 用户名
            /// </summary>
            public string Name => UserName;

            public string AuthenticationType => "JWT";

            public bool IsAuthenticated => true;

            public AuthUserInfo(int userId, string userName)
                : base(userId, userName)
            {

            }
        }
    }
}
