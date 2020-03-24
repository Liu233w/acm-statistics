using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace AcmStatisticsBackend.Localization
{
    public static class AcmStatisticsBackendLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(AcmStatisticsBackendConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(AcmStatisticsBackendLocalizationConfigurer).GetAssembly(),
                        "AcmStatisticsBackend.Localization.SourceFiles")));
        }
    }
}
