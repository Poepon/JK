using System.Security.Claims;
using Abp.Authorization;
using JK.Front;

namespace JK.Identity
{
    public class JKLoginResult<TUser, TUserLogin, TUserClaim, TUserToken>
             where TUser : FrontUserBase<TUserLogin, TUserClaim, TUserToken>
        where TUserLogin : FrontUserLogin, new()
        where TUserClaim : FrontUserClaim, new()
        where TUserToken : FrontUserToken, new()
    {
        public AbpLoginResultType Result { get; private set; }


        public TUser User { get; private set; }

        public ClaimsIdentity Identity { get; private set; }

        public JKLoginResult(AbpLoginResultType result, TUser user = null)
        {
            Result = result;
            User = user;
        }

        public JKLoginResult(TUser user, ClaimsIdentity identity)
            : this(AbpLoginResultType.Success)
        {
            User = user;
            Identity = identity;
        }
    }
}
