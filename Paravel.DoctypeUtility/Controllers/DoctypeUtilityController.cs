using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Paravel.DoctypeUtility.Models;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Cms.Web.Common.Controllers;

namespace Paravel.DoctypeUtility.Controllers;

// https://www.youtube.com/watch?v=2PfrBPXD0zw
// https://dev.to/kevinjump/umbraco-10-razor-class-library-packages-pt3-40bb
// https://codeshare.co.uk/blog/how-to-create-a-nuget-package-for-a-simple-app_plugin-style-umbraco-v9-package/
// https://our.umbraco.com/forum/using-umbraco-and-getting-started/110642-creating-nugat-package-for-umbraco


public class DoctypeUtilityController : UmbracoApiController
{
    private readonly ILogger<DoctypeUtilityController> _logger;
    private readonly IContentTypeService _contentTypeService;
    private DTUSettings _dtuSettings;
    private readonly IConfiguration _config;
    //private string token = "6c1fddb5-003c-4059-bbe4-8be016e12e5f";

    //public DoctypeUtilityController(IOptions<DTUSettings> dtuSettingOptions, ILogger<DoctypeUtilityController> logger, IContentTypeService contentTypeService)
    public DoctypeUtilityController(IConfiguration config, ILogger<DoctypeUtilityController> logger, IContentTypeService contentTypeService)
    {
        _logger = logger;
        _contentTypeService = contentTypeService;
        _config = config;
        var settings = _config.GetSection(DTUSettings.SectionName);
        if(settings != null)
        {
            _dtuSettings = settings.Get<DTUSettings>();
        }
        else
        {
            _dtuSettings = new DTUSettings();
        }

        //_dtuSettings = dtuSettingOptions.Value;
    }

    // [HttpGet("GetDoctypesAuth")]
    [HttpGet] // https://localhost:44324/umbraco/api/DoctypeUtility/GetDoctypesAuth
    public ActionResult<DoctypeBundle> GetDoctypesAuth()
    {
        if (Request.Headers.TryGetValue("Authorization", out var token))
        {
            // Token is in the format "Bearer {token_value}"
            string tokenValue = "";
            string[] arr = token.ToString().Split(' ');
            if(arr.Length > 1)
            {
                tokenValue = arr[1];
            }

            if (tokenValue != _dtuSettings.AuthToken)
            {
                return Unauthorized();
            }
        }
        else
        {
            return Unauthorized();
        }

        var doctypes = GetDoctypeBundleLocal();

        return Ok(doctypes);
    }

    private DoctypeBundle GetDoctypeBundleLocal()
    {
        var outy = new DoctypeBundle();
        var doctypes = _contentTypeService.GetAll().Select(x => new DoctypeDTO
        {
            Name = x.Name,
            Key = x.Key,
            Alias = x.Alias,
            Id = x.Id,
            Items = x.PropertyTypes.Select(y => new DoctypeElementDTO
            {
                Id = y.Id,
                Name = y.Name,
                Alias = y.Alias,
                PropertyEditorAlias = y.PropertyEditorAlias,
                DataTypeId = y.DataTypeId,
                DataTypeKey = y.Key,
                DatabaseType = y.ValueStorageType
            }).ToList()
        });

        outy.Doctypes.AddRange(doctypes);
        return outy;
    }

}

