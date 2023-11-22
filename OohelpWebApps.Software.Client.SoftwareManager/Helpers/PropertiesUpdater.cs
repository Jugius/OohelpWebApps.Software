using OohelpWebApps.Software.Domain;

namespace SoftwareManager.Helpers
{
    public static class PropertiesUpdater
    {
        public static void UpdatePropertiesBy(this ViewModels.Entities.ApplicationInfoVM model, ApplicationInfo apiInfo)
        {
            model.Name = apiInfo.Name;
            model.Description = apiInfo.Description;
            model.IsPublic = apiInfo.IsPublic;
        }
        public static void UpdatePropertiesBy(this ViewModels.Entities.ApplicationReleaseVM model, ApplicationRelease apiRelease)
        {
            model.Kind = apiRelease.Kind;
            model.Version = apiRelease.Version;

            model.ReleaseDate = apiRelease.ReleaseDate;
        }
        public static void UpdatePropertiesBy(this ViewModels.Entities.ReleaseDetailVM model, ReleaseDetail apiDetail)
        {
            model.Description = apiDetail.Description;
            model.Kind = apiDetail.Kind;
        }
    }
}
