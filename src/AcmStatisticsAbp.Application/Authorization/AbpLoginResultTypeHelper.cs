// <copyright file="AbpLoginResultTypeHelper.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Authorization
{
    using System;
    using Abp;
    using Abp.Authorization;
    using Abp.Dependency;
    using Abp.UI;

    public class AbpLoginResultTypeHelper : AbpServiceBase, ITransientDependency
    {
        public AbpLoginResultTypeHelper()
        {
            this.LocalizationSourceName = AcmStatisticsAbpConsts.LocalizationSourceName;
        }

        public Exception CreateExceptionForFailedLoginAttempt(AbpLoginResultType result, string usernameOrEmailAddress, string tenancyName)
        {
            switch (result)
            {
                case AbpLoginResultType.Success:
                    return new Exception("Don't call this method with a success result!");
                case AbpLoginResultType.InvalidUserNameOrEmailAddress:
                case AbpLoginResultType.InvalidPassword:
                    return new UserFriendlyException(this.L("LoginFailed"), this.L("InvalidUserNameOrPassword"));
                case AbpLoginResultType.InvalidTenancyName:
                    return new UserFriendlyException(this.L("LoginFailed"), this.L("ThereIsNoTenantDefinedWithName{0}", tenancyName));
                case AbpLoginResultType.TenantIsNotActive:
                    return new UserFriendlyException(this.L("LoginFailed"), this.L("TenantIsNotActive", tenancyName));
                case AbpLoginResultType.UserIsNotActive:
                    return new UserFriendlyException(this.L("LoginFailed"), this.L("UserIsNotActiveAndCanNotLogin", usernameOrEmailAddress));
                case AbpLoginResultType.UserEmailIsNotConfirmed:
                    return new UserFriendlyException(this.L("LoginFailed"), this.L("UserEmailIsNotConfirmedAndCanNotLogin"));
                case AbpLoginResultType.LockedOut:
                    return new UserFriendlyException(this.L("LoginFailed"), this.L("UserLockedOutMessage"));
                default: // Can not fall to default actually. But other result types can be added in the future and we may forget to handle it
                    this.Logger.Warn("Unhandled login fail reason: " + result);
                    return new UserFriendlyException(this.L("LoginFailed"));
            }
        }

        public string CreateLocalizedMessageForFailedLoginAttempt(AbpLoginResultType result, string usernameOrEmailAddress, string tenancyName)
        {
            switch (result)
            {
                case AbpLoginResultType.Success:
                    throw new Exception("Don't call this method with a success result!");
                case AbpLoginResultType.InvalidUserNameOrEmailAddress:
                case AbpLoginResultType.InvalidPassword:
                    return this.L("InvalidUserNameOrPassword");
                case AbpLoginResultType.InvalidTenancyName:
                    return this.L("ThereIsNoTenantDefinedWithName{0}", tenancyName);
                case AbpLoginResultType.TenantIsNotActive:
                    return this.L("TenantIsNotActive", tenancyName);
                case AbpLoginResultType.UserIsNotActive:
                    return this.L("UserIsNotActiveAndCanNotLogin", usernameOrEmailAddress);
                case AbpLoginResultType.UserEmailIsNotConfirmed:
                    return this.L("UserEmailIsNotConfirmedAndCanNotLogin");
                default: // Can not fall to default actually. But other result types can be added in the future and we may forget to handle it
                    this.Logger.Warn("Unhandled login fail reason: " + result);
                    return this.L("LoginFailed");
            }
        }
    }
}
