using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Identity.Firebase.Models
{
    using global::Microsoft.AspNetCore.Identity;
    using global::Microsoft.AspNetCore.Identity.EntityFrameworkCore;
    using global::Microsoft.Extensions.Logging;
    using global::Microsoft.Extensions.Options;
    using System.Threading.Tasks;

    namespace Microsoft.Identity.Firebase.Models
    {
        // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

        public class FirebaseApplicationUserManager : UserManager<FirebaseUser>
        {
            public FirebaseApplicationUserManager(IUserStore<FirebaseUser> store, IOptions<IdentityOptions> identityOptions, IPasswordHasher<FirebaseUser> passwordHasher, IEnumerable<IUserValidator<FirebaseUser>> userValidators, IEnumerable<IPasswordValidator<FirebaseUser>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errorDescriber, IServiceProvider services, ILogger<UserManager<FirebaseUser>> logger)
                : base(store, identityOptions, passwordHasher, userValidators,passwordValidators,keyNormalizer, errorDescriber, services, logger)
            {
            }

            /*
            public static ApplicationUserManager Create(IdentityFactoryOptions<FirebaseApplicationUserManager> options, IOwinContext context)
            {
                var manager = new ApplicationUserManager(new UserStore<FirebaseUser>(context.Get<ApplicationDbContext>()));
                // Configure validation logic for usernames
                manager.UserValidator = new UserValidator<FirebaseUser>(manager)
                {
                    AllowOnlyAlphanumericUserNames = false,
                    RequireUniqueEmail = true
                };
                // Configure validation logic for passwords
                manager.PasswordValidator = new PasswordValidator
                {
                    RequiredLength = 6,
                    RequireNonLetterOrDigit = true,
                    RequireDigit = true,
                    RequireLowercase = true,
                    RequireUppercase = true,
                };
                var dataProtectionProvider = options.DataProtectionProvider;
                if (dataProtectionProvider != null)
                {
                    manager.UserTokenProvider = new DataProtectorTokenProvider<FirebaseUser>(dataProtectionProvider.Create("ASP.NET Identity"));
                }
                return manager;
            }
            */
        }
    }

}
