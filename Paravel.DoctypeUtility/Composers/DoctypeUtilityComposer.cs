using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Manifest;

namespace Paravel.DoctypeUtility.Composers;

internal class DoctypeUtilityComposer : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.ManifestFilters().Append<DTUManifestFilter>();
    }
}

internal class DTUManifestFilter : IManifestFilter
{
    public void Filter(List<PackageManifest> manifests)
    {

        var assembly = typeof(DTUManifestFilter).Assembly;

        manifests.Add(new PackageManifest
        {
            PackageId = "Paravel.DoctypeUtility",
            PackageName = "Paravel.DoctypeUtility",
            Version = assembly.GetName()?.Version?.ToString(3) ?? "0.1.0",
            AllowPackageTelemetry = true,
            Scripts = new string[]
                {
                    "/App_Plugins/DoctypeUtility/doctypeutility.service.js",
                    "/App_Plugins/DoctypeUtility/doctypeutility.controller.js"
                },
            Dashboards = new[]
            {
                new ManifestDashboard
                {
                    Alias = "doctypeUtility",
                    View = "/App_Plugins/DoctypeUtility/doctypeutility.html",
                    Sections = new[] { "settings" },
                    Weight = 100
                }
            }

        });
    }
}

