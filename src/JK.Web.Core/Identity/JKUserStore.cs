﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Abp;
using Abp.Dependency;
using Abp.Domain.Repositories;
using Abp.Domain.Uow;
using Abp.Extensions;
using Abp.Linq;
using Abp.Runtime.Session;
using Castle.Core.Logging;
using JetBrains.Annotations;
using JK.Front;
using Microsoft.AspNetCore.Identity;

namespace JK.Identity
{
    public class JKUserStore<TUser, TUserLogin, TUserClaim, TUserToken> :
        IUserStore<TUser>,
        IUserLoginStore<TUser>,
        IUserClaimStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IUserEmailStore<TUser>,
        IUserLockoutStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IUserTwoFactorStore<TUser>,
        IUserAuthenticationTokenStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserAuthenticatorKeyStore<TUser>,
        ITransientDependency
        where TUser : FrontUserBase<TUserLogin, TUserClaim, TUserToken>
        where TUserLogin : FrontUserLogin, new()
        where TUserClaim : FrontUserClaim, new()
        where TUserToken : FrontUserToken, new()
    {
        public ILogger Logger { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="IdentityErrorDescriber"/> for any error that occurred with the current operation.
        /// </summary>
        public IdentityErrorDescriber ErrorDescriber { get; set; }

        /// <summary>
        /// Gets or sets a flag indicating if changes should be persisted after CreateAsync, UpdateAsync and DeleteAsync are called.
        /// </summary>
        /// <value>
        /// True if changes should be automatically persisted, otherwise false.
        /// </value>
        public bool AutoSaveChanges { get; set; } = true;

        public IAbpSession AbpSession { get; set; }

        public IQueryable<TUser> Users => CustomerRepository.GetAll();

        public IRepository<TUser, long> CustomerRepository { get; }

        private readonly IAsyncQueryableExecuter _asyncQueryableExecuter;
        private readonly IRepository<TUserLogin, long> _customerLoginRepository;
        private readonly IRepository<TUserClaim, long> _customerClaimRepository;

        private readonly IUnitOfWorkManager _unitOfWorkManager;

        public JKUserStore(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<TUser, long> customerRepository,
            IAsyncQueryableExecuter asyncQueryableExecuter,
            IRepository<TUserLogin, long> customerLoginRepository,
            IRepository<TUserClaim, long> customerClaimRepository)
        {
            _unitOfWorkManager = unitOfWorkManager;
            CustomerRepository = customerRepository;
            _asyncQueryableExecuter = asyncQueryableExecuter;
            _customerLoginRepository = customerLoginRepository;
            _customerClaimRepository = customerClaimRepository;

            AbpSession = NullAbpSession.Instance;
            ErrorDescriber = new IdentityErrorDescriber();
            Logger = NullLogger.Instance;
        }

        /// <summary>Saves the current store.</summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        protected Task SaveChanges(CancellationToken cancellationToken)
        {
            if (!AutoSaveChanges || _unitOfWorkManager.Current == null)
            {
                return Task.CompletedTask;
            }

            return _unitOfWorkManager.Current.SaveChangesAsync();
        }

        /// <summary>
        /// Gets the user identifier for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose identifier should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the identifier for the specified <paramref name="user"/>.</returns>
        public virtual Task<string> GetUserIdAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Id.ToString());
        }

        /// <summary>
        /// Gets the user name for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose name should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the name for the specified <paramref name="user"/>.</returns>
        public virtual Task<string> GetUserNameAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.UserName);
        }

        /// <summary>
        /// Sets the given <paramref name="userName" /> for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose name should be set.</param>
        /// <param name="userName">The user name to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetUserNameAsync([NotNull] TUser user, string userName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.UserName = userName;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the normalized user name for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose normalized name should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the normalized user name for the specified <paramref name="user"/>.</returns>
        public virtual Task<string> GetNormalizedUserNameAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.NormalizedUserName);
        }

        /// <summary>
        /// Sets the given normalized name for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose name should be set.</param>
        /// <param name="normalizedName">The normalized name to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetNormalizedUserNameAsync([NotNull] TUser user, string normalizedName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.NormalizedUserName = normalizedName;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Creates the specified <paramref name="user"/> in the user store.
        /// </summary>
        /// <param name="user">The user to create.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the creation operation.</returns>
        public virtual async Task<IdentityResult> CreateAsync([NotNull] TUser customer, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(customer, nameof(customer));

            await CustomerRepository.InsertAsync(customer);
            await SaveChanges(cancellationToken);

            return IdentityResult.Success;
        }

        /// <summary>
        /// Updates the specified <paramref name="customer"/> in the user store.
        /// </summary>
        /// <param name="customer">The customer to update.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.</returns>
        public virtual async Task<IdentityResult> UpdateAsync([NotNull] TUser customer, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(customer, nameof(customer));

            customer.ConcurrencyStamp = Guid.NewGuid().ToString();
            await CustomerRepository.UpdateAsync(customer);

            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (AbpDbConcurrencyException ex)
            {
                Logger.Warn(ex.ToString(), ex);
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }

            await SaveChanges(cancellationToken);

            return IdentityResult.Success;
        }

        /// <summary>
        /// Deletes the specified <paramref name="customer"/> from the user store.
        /// </summary>
        /// <param name="customer">The customer to delete.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the <see cref="IdentityResult"/> of the update operation.</returns>
        public virtual async Task<IdentityResult> DeleteAsync([NotNull] TUser customer, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(customer, nameof(customer));

            await CustomerRepository.DeleteAsync(customer);

            try
            {
                await SaveChanges(cancellationToken);
            }
            catch (AbpDbConcurrencyException ex)
            {
                Logger.Warn(ex.ToString(), ex);
                return IdentityResult.Failed(ErrorDescriber.ConcurrencyFailure());
            }

            await SaveChanges(cancellationToken);

            return IdentityResult.Success;
        }

        /// <summary>
        /// Finds and returns a user, if any, who has the specified <paramref name="userId"/>.
        /// </summary>
        /// <param name="customerId">The customer ID to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="userId"/> if it exists.
        /// </returns>
        public virtual Task<TUser> FindByIdAsync(string customerId, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            return CustomerRepository.FirstOrDefaultAsync(customerId.To<long>());
        }

        /// <summary>
        /// Finds and returns a user, if any, who has the specified normalized user name.
        /// </summary>
        /// <param name="normalizedUserName">The normalized user name to search for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing the user matching the specified <paramref name="normalizedUserName"/> if it exists.
        /// </returns>
        public virtual Task<TUser> FindByNameAsync([NotNull] string normalizedUserName, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(normalizedUserName, nameof(normalizedUserName));

            return CustomerRepository.FirstOrDefaultAsync(u => u.NormalizedUserName == normalizedUserName);
        }

        /// <summary>
        /// Sets the password hash for a user.
        /// </summary>
        /// <param name="user">The user to set the password hash for.</param>
        /// <param name="passwordHash">The password hash to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetPasswordHashAsync([NotNull] TUser user, string passwordHash, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.Password = passwordHash;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the password hash for a user.
        /// </summary>
        /// <param name="user">The user to retrieve the password hash for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the password hash for the user.</returns>
        public virtual Task<string> GetPasswordHashAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Password);
        }

        /// <summary>
        /// Returns a flag indicating if the specified user has a password.
        /// </summary>
        /// <param name="user">The user to retrieve the password hash for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> containing a flag indicating if the specified user has a password. If the 
        /// user has a password the returned value with be true, otherwise it will be false.</returns>
        public virtual Task<bool> HasPasswordAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Password != null);
        }

        /// <summary>
        /// Dispose the store
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// Get the claims associated with the specified <paramref name="user"/> as an asynchronous operation.
        /// </summary>
        /// <param name="customer">The customer whose claims should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>A <see cref="Task{TResult}"/> that contains the claims granted to a user.</returns>
        public virtual async Task<IList<Claim>> GetClaimsAsync([NotNull] TUser customer, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(customer, nameof(customer));

            await CustomerRepository.EnsureCollectionLoadedAsync(customer, u => u.Claims, cancellationToken);

            return customer.Claims.Select(c => new Claim(c.ClaimType, c.ClaimValue)).ToList();
        }

        /// <summary>
        /// Adds the <paramref name="claims"/> given to the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to add the claim to.</param>
        /// <param name="claims">The claim to add to the user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual async Task AddClaimsAsync([NotNull] TUser user, [NotNull] IEnumerable<Claim> claims, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            Check.NotNull(claims, nameof(claims));

            await CustomerRepository.EnsureCollectionLoadedAsync(user, u => u.Claims, cancellationToken);

            foreach (var claim in claims)
            {
                user.Claims.Add(new TUserClaim()
                {
                    UserId = user.Id,
                    TenantId = user.TenantId,
                    ClaimType = claim.Type,
                    ClaimValue = claim.Value
                });
            }
        }

        /// <summary>
        /// Replaces the <paramref name="claim"/> on the specified <paramref name="user"/>, with the <paramref name="newClaim"/>.
        /// </summary>
        /// <param name="user">The user to replace the claim on.</param>
        /// <param name="claim">The claim replace.</param>
        /// <param name="newClaim">The new claim replacing the <paramref name="claim"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual async Task ReplaceClaimAsync([NotNull] TUser user, [NotNull] Claim claim, [NotNull] Claim newClaim, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            Check.NotNull(claim, nameof(claim));
            Check.NotNull(newClaim, nameof(newClaim));

            await CustomerRepository.EnsureCollectionLoadedAsync(user, u => u.Claims, cancellationToken);

            var userClaims = user.Claims.Where(uc => uc.ClaimValue == claim.Value && uc.ClaimType == claim.Type);
            foreach (var userClaim in userClaims)
            {
                userClaim.ClaimType = claim.Type;
                userClaim.ClaimValue = claim.Value;
            }
        }

        /// <summary>
        /// Removes the <paramref name="claims"/> given from the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to remove the claims from.</param>
        /// <param name="claims">The claim to remove.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual async Task RemoveClaimsAsync([NotNull] TUser user, [NotNull] IEnumerable<Claim> claims, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            Check.NotNull(claims, nameof(claims));

            await CustomerRepository.EnsureCollectionLoadedAsync(user, u => u.Claims, cancellationToken);

            foreach (var claim in claims)
            {
                user.Claims.RemoveAll(c => c.ClaimValue == claim.Value && c.ClaimType == claim.Type);
            }
        }

        /// <summary>
        /// Adds the <paramref name="login"/> given to the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to add the login to.</param>
        /// <param name="login">The login to add to the user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual async Task AddLoginAsync([NotNull] TUser user, [NotNull] UserLoginInfo login, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            Check.NotNull(login, nameof(login));

            await CustomerRepository.EnsureCollectionLoadedAsync(user, u => u.Logins, cancellationToken);

            user.Logins.Add(new TUserLogin()
            {
                TenantId = user.TenantId,
                UserId = user.Id,
                LoginProvider = login.LoginProvider,
                ProviderKey = login.ProviderKey
            });
        }

        /// <summary>
        /// Removes the <paramref name="loginProvider"/> given from the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user to remove the login from.</param>
        /// <param name="loginProvider">The login to remove from the user.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual async Task RemoveLoginAsync([NotNull] TUser user, [NotNull] string loginProvider, [NotNull] string providerKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));
            Check.NotNull(loginProvider, nameof(loginProvider));
            Check.NotNull(providerKey, nameof(providerKey));

            await CustomerRepository.EnsureCollectionLoadedAsync(user, u => u.Logins, cancellationToken);

            user.Logins.RemoveAll(userLogin => userLogin.LoginProvider == loginProvider && userLogin.ProviderKey == providerKey);
        }

        /// <summary>
        /// Retrieves the associated logins for the specified <param ref="user"/>.
        /// </summary>
        /// <param name="user">The user whose associated logins to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> for the asynchronous operation, containing a list of <see cref="UserLoginInfo"/> for the specified <paramref name="user"/>, if any.
        /// </returns>
        public virtual async Task<IList<UserLoginInfo>> GetLoginsAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            await CustomerRepository.EnsureCollectionLoadedAsync(user, u => u.Logins, cancellationToken);

            return user.Logins.Select(l => new UserLoginInfo(l.LoginProvider, l.ProviderKey, l.LoginProvider)).ToList();
        }

        /// <summary>
        /// Retrieves the user associated with the specified login provider and login provider key..
        /// </summary>
        /// <param name="loginProvider">The login provider who provided the <paramref name="providerKey"/>.</param>
        /// <param name="providerKey">The key provided by the <paramref name="loginProvider"/> to identify a user.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> for the asynchronous operation, containing the user, if any which matched the specified login provider and key.
        /// </returns>
        [UnitOfWork]
        public virtual Task<TUser> FindByLoginAsync([NotNull] string loginProvider, [NotNull] string providerKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(loginProvider, nameof(loginProvider));
            Check.NotNull(providerKey, nameof(providerKey));

            var query = from userLogin in _customerLoginRepository.GetAll()
                        join user in CustomerRepository.GetAll() on userLogin.UserId equals user.Id
                        where userLogin.LoginProvider == loginProvider &&
                              userLogin.ProviderKey == providerKey &&
                              userLogin.TenantId == AbpSession.TenantId
                        select user;

            return _asyncQueryableExecuter.FirstOrDefaultAsync(query);
        }

        /// <summary>
        /// Gets a flag indicating whether the email address for the specified <paramref name="user"/> has been verified, true if the email address is verified otherwise
        /// false.
        /// </summary>
        /// <param name="user">The user whose email confirmation status should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous operation, a flag indicating whether the email address for the specified <paramref name="user"/>
        /// has been confirmed or not.
        /// </returns>
        public virtual Task<bool> GetEmailConfirmedAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.IsEmailConfirmed);
        }

        /// <summary>
        /// Sets the flag indicating whether the specified <paramref name="user"/>'s email address has been confirmed or not.
        /// </summary>
        /// <param name="user">The user whose email confirmation status should be set.</param>
        /// <param name="confirmed">A flag indicating if the email address has been confirmed, true if the address is confirmed otherwise false.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public virtual Task SetEmailConfirmedAsync([NotNull] TUser user, bool confirmed, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.IsEmailConfirmed = confirmed;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Sets the <paramref name="email"/> address for a <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose email should be set.</param>
        /// <param name="email">The email to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public virtual Task SetEmailAsync([NotNull] TUser user, string email, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.Email = email;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the email address for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose email should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object containing the results of the asynchronous operation, the email address for the specified <paramref name="user"/>.</returns>
        public virtual Task<string> GetEmailAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.Email);
        }

        /// <summary>
        /// Returns the normalized email for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose email address to retrieve.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous lookup operation, the normalized email address if any associated with the specified user.
        /// </returns>
        public virtual Task<string> GetNormalizedEmailAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.NormalizedEmail);
        }

        /// <summary>
        /// Sets the normalized email for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose email address to set.</param>
        /// <param name="normalizedEmail">The normalized email to set for the specified <paramref name="user"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        public virtual Task SetNormalizedEmailAsync([NotNull] TUser user, string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.NormalizedEmail = normalizedEmail;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the user, if any, associated with the specified, normalized email address.
        /// </summary>
        /// <param name="normalizedEmail">The normalized email address to return the user for.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The task object containing the results of the asynchronous lookup operation, the user if any associated with the specified normalized email address.
        /// </returns>
        public virtual Task<TUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            return CustomerRepository.FirstOrDefaultAsync(u => u.NormalizedEmail == normalizedEmail);
        }

        /// <summary>
        /// Gets the last <see cref="DateTimeOffset"/> a user's last lockout expired, if any.
        /// Any time in the past should be indicates a user is not locked out.
        /// </summary>
        /// <param name="user">The user whose lockout date should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// A <see cref="Task{TResult}"/> that represents the result of the asynchronous query, a <see cref="DateTimeOffset"/> containing the last time
        /// a user's lockout expired, if any.
        /// </returns>
        public virtual Task<DateTimeOffset?> GetLockoutEndDateAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            if (!user.LockoutEnd.HasValue)
            {
                return Task.FromResult<DateTimeOffset?>(null);
            }

            var lockoutEndDate = user.LockoutEnd.Value;

            return Task.FromResult<DateTimeOffset?>(lockoutEndDate);
        }

        /// <summary>
        /// Locks out a user until the specified end date has passed. Setting a end date in the past immediately unlocks a user.
        /// </summary>
        /// <param name="user">The user whose lockout date should be set.</param>
        /// <param name="lockoutEnd">The <see cref="DateTimeOffset"/> after which the <paramref name="user"/>'s lockout should end.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetLockoutEndDateAsync([NotNull] TUser user, DateTimeOffset? lockoutEnd, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.LockoutEnd = lockoutEnd?.UtcDateTime;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Records that a failed access has occurred, incrementing the failed access count.
        /// </summary>
        /// <param name="user">The user whose cancellation count should be incremented.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the incremented failed access count.</returns>
        public virtual Task<int> IncrementAccessFailedCountAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.AccessFailedCount++;

            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        /// Resets a user's failed access count.
        /// </summary>
        /// <param name="user">The user whose failed access count should be reset.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        /// <remarks>This is typically called after the account is successfully accessed.</remarks>
        public virtual Task ResetAccessFailedCountAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.AccessFailedCount = 0;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Retrieves the current failed access count for the specified <paramref name="user"/>..
        /// </summary>
        /// <param name="user">The user whose failed access count should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the failed access count.</returns>
        public virtual Task<int> GetAccessFailedCountAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.AccessFailedCount);
        }

        /// <summary>
        /// Retrieves a flag indicating whether user lockout can enabled for the specified user.
        /// </summary>
        /// <param name="user">The user whose ability to be locked out should be returned.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, true if a user can be locked out, otherwise false.
        /// </returns>
        public virtual Task<bool> GetLockoutEnabledAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.IsLockoutEnabled);
        }

        /// <summary>
        /// Set the flag indicating if the specified <paramref name="user"/> can be locked out..
        /// </summary>
        /// <param name="user">The user whose ability to be locked out should be set.</param>
        /// <param name="enabled">A flag indicating if lock out can be enabled for the specified <paramref name="user"/>.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetLockoutEnabledAsync([NotNull] TUser user, bool enabled, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.IsLockoutEnabled = enabled;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Sets the telephone number for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose telephone number should be set.</param>
        /// <param name="phoneNumber">The telephone number to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetPhoneNumberAsync([NotNull] TUser user, string phoneNumber, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.PhoneNumber = phoneNumber;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Gets the telephone number, if any, for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose telephone number should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the user's telephone number, if any.</returns>
        public virtual Task<string> GetPhoneNumberAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.PhoneNumber);
        }

        /// <summary>
        /// Gets a flag indicating whether the specified <paramref name="user"/>'s telephone number has been confirmed.
        /// </summary>
        /// <param name="user">The user to return a flag for, indicating whether their telephone number is confirmed.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, returning true if the specified <paramref name="user"/> has a confirmed
        /// telephone number otherwise false.
        /// </returns>
        public virtual Task<bool> GetPhoneNumberConfirmedAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.IsPhoneNumberConfirmed);
        }

        /// <summary>
        /// Sets a flag indicating if the specified <paramref name="user"/>'s phone number has been confirmed..
        /// </summary>
        /// <param name="user">The user whose telephone number confirmation status should be set.</param>
        /// <param name="confirmed">A flag indicating whether the user's telephone number has been confirmed.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetPhoneNumberConfirmedAsync([NotNull] TUser user, bool confirmed, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.IsPhoneNumberConfirmed = confirmed;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Sets the provided security <paramref name="stamp"/> for the specified <paramref name="user"/>.
        /// </summary>
        /// <param name="user">The user whose security stamp should be set.</param>
        /// <param name="stamp">The security stamp to set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetSecurityStampAsync([NotNull] TUser user, string stamp, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.SecurityStamp = stamp;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Get the security stamp for the specified <paramref name="user" />.
        /// </summary>
        /// <param name="user">The user whose security stamp should be set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation, containing the security stamp for the specified <paramref name="user"/>.</returns>
        public virtual Task<string> GetSecurityStampAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.SecurityStamp);
        }

        /// <summary>
        /// Sets a flag indicating whether the specified <paramref name="user"/> has two factor authentication enabled or not,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose two factor authentication enabled status should be set.</param>
        /// <param name="enabled">A flag indicating whether the specified <paramref name="user"/> has two factor authentication enabled.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual Task SetTwoFactorEnabledAsync([NotNull] TUser user, bool enabled, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            user.IsTwoFactorEnabled = enabled;

            return Task.CompletedTask;
        }

        /// <summary>
        /// Returns a flag indicating whether the specified <paramref name="user"/> has two factor authentication enabled or not,
        /// as an asynchronous operation.
        /// </summary>
        /// <param name="user">The user whose two factor authentication enabled status should be set.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> that represents the asynchronous operation, containing a flag indicating whether the specified 
        /// <paramref name="user"/> has two factor authentication enabled or not.
        /// </returns>
        public virtual Task<bool> GetTwoFactorEnabledAsync([NotNull] TUser user, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            return Task.FromResult(user.IsTwoFactorEnabled);
        }

        /// <summary>
        /// Retrieves all users with the specified claim.
        /// </summary>
        /// <param name="claim">The claim whose users should be retrieved.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>
        /// The <see cref="Task"/> contains a list of users, if any, that contain the specified claim. 
        /// </returns>
        [UnitOfWork]
        public virtual async Task<IList<TUser>> GetUsersForClaimAsync([NotNull] Claim claim, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(claim, nameof(claim));

            var query = from userclaims in _customerClaimRepository.GetAll()
                        join user in CustomerRepository.GetAll() on userclaims.UserId equals user.Id
                        where userclaims.ClaimValue == claim.Value && userclaims.ClaimType == claim.Type && userclaims.TenantId == AbpSession.TenantId
                        select user;

            return await _asyncQueryableExecuter.ToListAsync(query);
        }

        /// <summary>
        /// Sets the token value for a particular user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="loginProvider">The authentication provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="value">The value of the token.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public virtual async Task SetTokenAsync([NotNull] TUser user, string loginProvider, string name, string value, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            await CustomerRepository.EnsureCollectionLoadedAsync(user, u => u.Tokens, cancellationToken);

            var token = user.Tokens.FirstOrDefault(t => t.LoginProvider == loginProvider && t.Name == name);
            if (token == null)
            {
                user.Tokens.Add(new TUserToken()
                {
                    TenantId = user.TenantId,
                    UserId = user.Id,
                    LoginProvider = loginProvider,
                    Name = name,
                    Value = value
                });
            }
            else
            {
                token.Value = value;
            }
        }

        /// <summary>
        /// Deletes a token for a user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="loginProvider">The authentication provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async Task RemoveTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            await CustomerRepository.EnsureCollectionLoadedAsync(user, u => u.Tokens, cancellationToken);

            user.Tokens.RemoveAll(t => t.LoginProvider == loginProvider && t.Name == name);
        }

        /// <summary>
        /// Returns the token value.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="loginProvider">The authentication provider for the token.</param>
        /// <param name="name">The name of the token.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> used to propagate notifications that the operation should be canceled.</param>
        /// <returns>The <see cref="Task"/> that represents the asynchronous operation.</returns>
        public async Task<string> GetTokenAsync(TUser user, string loginProvider, string name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            await CustomerRepository.EnsureCollectionLoadedAsync(user, u => u.Tokens, cancellationToken);

            return user.Tokens.FirstOrDefault(t => t.LoginProvider == loginProvider && t.Name == name)?.Value;
        }

        /// <summary>
        /// Tries to find a user with user name or email address in current tenant.
        /// </summary>
        /// <param name="userNameOrEmailAddress">User name or email address</param>
        /// <returns>User or null</returns>
        public virtual async Task<TUser> FindByNameOrEmailAsync(string userNameOrEmailAddress)
        {
            return await CustomerRepository.FirstOrDefaultAsync(
                user => (user.UserName == userNameOrEmailAddress || user.Email == userNameOrEmailAddress)
                );
        }

        /// <summary>
        /// Tries to find a user with user name or email address in given tenant.
        /// </summary>
        /// <param name="tenantId">Tenant Id</param>
        /// <param name="userNameOrEmailAddress">User name or email address</param>
        /// <returns>User or null</returns>
        [UnitOfWork]
        public virtual async Task<TUser> FindByNameOrEmailAsync(int? tenantId, string userNameOrEmailAddress)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                return await FindByNameOrEmailAsync(userNameOrEmailAddress);
            }
        }

        public virtual async Task<TUser> FindAsync(UserLoginInfo login)
        {
            var userLogin = await _customerLoginRepository.FirstOrDefaultAsync(
                ul => ul.LoginProvider == login.LoginProvider && ul.ProviderKey == login.ProviderKey
            );

            if (userLogin == null)
            {
                return null;
            }

            return await CustomerRepository.FirstOrDefaultAsync(u => u.Id == userLogin.UserId);
        }

        [UnitOfWork]
        public virtual Task<List<TUser>> FindAllAsync(UserLoginInfo login)
        {
            var query = from userLogin in _customerLoginRepository.GetAll()
                        join user in CustomerRepository.GetAll() on userLogin.UserId equals user.Id
                        where userLogin.LoginProvider == login.LoginProvider && userLogin.ProviderKey == login.ProviderKey
                        select user;

            return Task.FromResult(query.ToList());
        }

        [UnitOfWork]
        public virtual Task<TUser> FindAsync(int? tenantId, UserLoginInfo login)
        {
            using (_unitOfWorkManager.Current.SetTenantId(tenantId))
            {
                var query = from userLogin in _customerLoginRepository.GetAll()
                            join user in CustomerRepository.GetAll() on userLogin.UserId equals user.Id
                            where userLogin.LoginProvider == login.LoginProvider && userLogin.ProviderKey == login.ProviderKey
                            select user;

                return Task.FromResult(query.FirstOrDefault());
            }
        }

        public async Task<string> GetUserNameFromDatabaseAsync(long userId)
        {
            using (var uow = _unitOfWorkManager.Begin(new UnitOfWorkOptions
            {
                Scope = TransactionScopeOption.Suppress
            }))
            {
                var user = await CustomerRepository.GetAsync(userId);
                await uow.CompleteAsync();
                return user.UserName;
            }
        }

        private const string InternalLoginProvider = "[AspNetUserStore]";
        private const string AuthenticatorKeyTokenName = "AuthenticatorKey";
        private const string TokenValidityKeyProvider = "TokenValidityKeyProvider";

        public virtual async Task SetAuthenticatorKeyAsync(TUser user, string key, CancellationToken cancellationToken)
        {
            await SetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, key, cancellationToken);
        }

        public async Task<string> GetAuthenticatorKeyAsync(TUser user, CancellationToken cancellationToken)
        {
            return await GetTokenAsync(user, InternalLoginProvider, AuthenticatorKeyTokenName, cancellationToken);
        }

        public virtual async Task AddTokenValidityKeyAsync([NotNull] TUser user, string tokenValidityKey, DateTime expireDate, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            await CustomerRepository.EnsureCollectionLoadedAsync(user, u => u.Tokens, cancellationToken);

            user.Tokens.Add(new TUserToken
            {
                TenantId = user.TenantId,
                UserId = user.Id,
                LoginProvider = TokenValidityKeyProvider,
                Name = tokenValidityKey,
                Value = null,
                ExpireDate = expireDate
            });
        }

        public virtual async Task<bool> IsTokenValidityKeyValidAsync([NotNull] TUser user, string tokenValidityKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            await CustomerRepository.EnsureCollectionLoadedAsync(user, u => u.Tokens, cancellationToken);
            var isValidityKeyValid = user.Tokens.Any(t => t.LoginProvider == TokenValidityKeyProvider &&
                                               t.Name == tokenValidityKey &&
                                               t.ExpireDate > DateTime.UtcNow);

            user.Tokens.RemoveAll(t => t.LoginProvider == TokenValidityKeyProvider && t.ExpireDate <= DateTime.UtcNow);

            return isValidityKeyValid;
        }

        public virtual async Task RemoveTokenValidityKeyAsync([NotNull] TUser user, string tokenValidityKey, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            Check.NotNull(user, nameof(user));

            await CustomerRepository.EnsureCollectionLoadedAsync(user, u => u.Tokens, cancellationToken);

            user.Tokens.Remove(user.Tokens.FirstOrDefault(t =>
                t.LoginProvider == TokenValidityKeyProvider && t.Name == tokenValidityKey));
        }
    }
}
