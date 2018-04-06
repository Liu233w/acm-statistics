// <copyright file="ValidationHelper.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Validation
{
    using System.Text.RegularExpressions;
    using Abp.Extensions;

    public static class ValidationHelper
    {
        public const string EmailRegex = @"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";

        public static bool IsEmail(string value)
        {
            if (value.IsNullOrEmpty())
            {
                return false;
            }

            var regex = new Regex(EmailRegex);
            return regex.IsMatch(value);
        }
    }
}
