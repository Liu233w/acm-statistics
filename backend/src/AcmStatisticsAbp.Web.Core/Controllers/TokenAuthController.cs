// <copyright file="TokenAuthController.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Abp.Authorization;
    using Abp.Authorization.Users;
    using Abp.MultiTenancy;
    using Abp.Runtime.Security;
    using Abp.UI;
    using AcmStatisticsAbp.Authentication.External;
    using AcmStatisticsAbp.Authentication.JwtBearer;
    using AcmStatisticsAbp.Authorization;
    using AcmStatisticsAbp.Authorization.Users;
    using AcmStatisticsAbp.Models.TokenAuth;
    using AcmStatisticsAbp.MultiTenancy;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/[controller]/[action]")]
    public class TokenAuthController : AcmStatisticsAbpControllerBase
    {
        private readonly LogInManager logInManager;
        private readonly ITenantCache tenantCache;
        private readonly AbpLoginResultTypeHelper abpLoginResultTypeHelper;
        private readonly TokenAuthConfiguration configuration;
        private readonly IExternalAuthConfiguration externalAuthConfiguration;
        private readonly IExternalAuthManager externalAuthManager;
        private readonly UserRegistrationManager userRegistrationManager;

        public TokenAuthController(
            LogInManager logInManager,
            ITenantCache tenantCache,
            AbpLoginResultTypeHelper abpLoginResultTypeHelper,
            TokenAuthConfiguration configuration,
            IExternalAuthConfiguration externalAuthConfiguration,
            IExternalAuthManager externalAuthManager,
            UserRegistrationManager userRegistrationManager)
        {
            this.logInManager = logInManager;
            this.tenantCache = tenantCache;
            this.abpLoginResultTypeHelper = abpLoginResultTypeHelper;
            this.configuration = configuration;
            this.externalAuthConfiguration = externalAuthConfiguration;
            this.externalAuthManager = externalAuthManager;
            this.userRegistrationManager = userRegistrationManager;
        }

        [HttpPost]
        public async Task<AuthenticateResultModel> Authenticate([FromBody] AuthenticateModel model)
        {
            var loginResult = await this.GetLoginResultAsync(
                model.UserNameOrEmailAddress,
                model.Password,
                this.GetTenancyNameOrNull());

            var accessToken = this.CreateAccessToken(CreateJwtClaims(loginResult.Identity));

            return new AuthenticateResultModel
            {
                AccessToken = accessToken,
                EncryptedAccessToken = this.GetEncrpyedAccessToken(accessToken),
                ExpireInSeconds = (int)this.configuration.Expiration.TotalSeconds,
                UserId = loginResult.User.Id,
            };
        }

        [HttpGet]
        public List<ExternalLoginProviderInfoModel> GetExternalAuthenticationProviders()
        {
            return this.ObjectMapper.Map<List<ExternalLoginProviderInfoModel>>(this.externalAuthConfiguration.Providers);
        }

        [HttpPost]
        public async Task<ExternalAuthenticateResultModel> ExternalAuthenticate([FromBody] ExternalAuthenticateModel model)
        {
            var externalUser = await this.GetExternalUserInfo(model);

            var loginResult = await this.logInManager.LoginAsync(new UserLoginInfo(model.AuthProvider, model.ProviderKey, model.AuthProvider), this.GetTenancyNameOrNull());

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    {
                        var accessToken = this.CreateAccessToken(CreateJwtClaims(loginResult.Identity));
                        return new ExternalAuthenticateResultModel
                        {
                            AccessToken = accessToken,
                            EncryptedAccessToken = this.GetEncrpyedAccessToken(accessToken),
                            ExpireInSeconds = (int)this.configuration.Expiration.TotalSeconds,
                        };
                    }

                case AbpLoginResultType.UnknownExternalLogin:
                    {
                        var newUser = await this.RegisterExternalUserAsync(externalUser);
                        if (!newUser.IsActive)
                        {
                            return new ExternalAuthenticateResultModel
                            {
                                WaitingForActivation = true,
                            };
                        }

                        // Try to login again with newly registered user!
                        loginResult = await this.logInManager.LoginAsync(new UserLoginInfo(model.AuthProvider, model.ProviderKey, model.AuthProvider), this.GetTenancyNameOrNull());
                        if (loginResult.Result != AbpLoginResultType.Success)
                        {
                            throw this.abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                                loginResult.Result,
                                model.ProviderKey,
                                this.GetTenancyNameOrNull());
                        }

                        return new ExternalAuthenticateResultModel
                        {
                            AccessToken = this.CreateAccessToken(CreateJwtClaims(loginResult.Identity)),
                            ExpireInSeconds = (int)this.configuration.Expiration.TotalSeconds,
                        };
                    }

                default:
                    {
                        throw this.abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(
                            loginResult.Result,
                            model.ProviderKey,
                            this.GetTenancyNameOrNull());
                    }
            }
        }

        private async Task<User> RegisterExternalUserAsync(ExternalAuthUserInfo externalUser)
        {
            var user = await this.userRegistrationManager.RegisterAsync(
                externalUser.Name,
                externalUser.Surname,
                externalUser.EmailAddress,
                externalUser.EmailAddress,
                Authorization.Users.User.CreateRandomPassword(),
                true);

            user.Logins = new List<UserLogin>
            {
                new UserLogin
                {
                    LoginProvider = externalUser.Provider,
                    ProviderKey = externalUser.ProviderKey,
                    TenantId = user.TenantId,
                },
            };

            await this.CurrentUnitOfWork.SaveChangesAsync();

            return user;
        }

        private async Task<ExternalAuthUserInfo> GetExternalUserInfo(ExternalAuthenticateModel model)
        {
            var userInfo = await this.externalAuthManager.GetUserInfo(model.AuthProvider, model.ProviderAccessCode);
            if (userInfo.ProviderKey != model.ProviderKey)
            {
                throw new UserFriendlyException(this.L("CouldNotValidateExternalUser"));
            }

            return userInfo;
        }

        private string GetTenancyNameOrNull()
        {
            if (!this.AbpSession.TenantId.HasValue)
            {
                return null;
            }

            return this.tenantCache.GetOrNull(this.AbpSession.TenantId.Value)?.TenancyName;
        }

        private async Task<AbpLoginResult<Tenant, User>> GetLoginResultAsync(string usernameOrEmailAddress, string password, string tenancyName)
        {
            var loginResult = await this.logInManager.LoginAsync(usernameOrEmailAddress, password, tenancyName);

            switch (loginResult.Result)
            {
                case AbpLoginResultType.Success:
                    return loginResult;
                default:
                    throw this.abpLoginResultTypeHelper.CreateExceptionForFailedLoginAttempt(loginResult.Result, usernameOrEmailAddress, tenancyName);
            }
        }

        private string CreateAccessToken(IEnumerable<Claim> claims, TimeSpan? expiration = null)
        {
            var now = DateTime.UtcNow;

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: this.configuration.Issuer,
                audience: this.configuration.Audience,
                claims: claims,
                notBefore: now,
                expires: now.Add(expiration ?? this.configuration.Expiration),
                signingCredentials: this.configuration.SigningCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }

        private static List<Claim> CreateJwtClaims(ClaimsIdentity identity)
        {
            var claims = identity.Claims.ToList();
            var nameIdClaim = claims.First(c => c.Type == ClaimTypes.NameIdentifier);

            // Specifically add the jti (random nonce), iat (issued timestamp), and sub (subject/user) claims.
            claims.AddRange(new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, nameIdClaim.Value),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64),
            });

            return claims;
        }

        private string GetEncrpyedAccessToken(string accessToken)
        {
            return SimpleStringCipher.Instance.Encrypt(accessToken, AppConsts.DefaultPassPhrase);
        }
    }
}
