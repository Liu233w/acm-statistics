// <copyright file="UiThemeInfo.cs" company="西北工业大学ACM开发组">
// Copyright (c) 西北工业大学ACM开发组. All rights reserved.
// </copyright>

namespace AcmStatisticsAbp.Configuration.Ui
{
    public class UiThemeInfo
    {
        public string Name { get; }

        public string CssClass { get; }

        public UiThemeInfo(string name, string cssClass)
        {
            this.Name = name;
            this.CssClass = cssClass;
        }
    }
}
