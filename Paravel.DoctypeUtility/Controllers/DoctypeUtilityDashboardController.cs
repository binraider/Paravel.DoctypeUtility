using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Paravel.DoctypeUtility.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;

namespace Paravel.DoctypeUtility.Controllers;


public class DoctypeUtilityDashboardController : UmbracoAuthorizedApiController
{
    private readonly ILogger<DoctypeUtilityDashboardController> _logger;
    private readonly IContentTypeService _contentTypeService;
    private DTUSettings _dtuSettings;
    private readonly IConfiguration _config;

    public DoctypeUtilityDashboardController(IConfiguration config, ILogger<DoctypeUtilityDashboardController> logger, IContentTypeService contentTypeService)
    {
        _logger = logger;
        _contentTypeService = contentTypeService;
        _config = config;
        var settings = _config.GetSection(DTUSettings.SectionName);
        if (settings != null)
        {
            _dtuSettings = settings.Get<DTUSettings>();
        }
        else
        {
            _dtuSettings = new DTUSettings();
        }
    }


    [HttpGet]
    public List<DTUSite> ListSites()
    {
        var sites = new List<DTUSite>();
        if (_dtuSettings != null && _dtuSettings.Sites != null)
        {
            foreach (var site in _dtuSettings.Sites)
            {
                sites.Add(new DTUSite
                {
                    Name = site.Name,
                    Url = site.Url
                });
            }
        }
        return sites;
    }

    [HttpGet]
    public async Task<DoubleDoctypeBundle> ListDifferences([FromQuery] string siteName)
    {
        DoubleDoctypeBundle outy = new DoubleDoctypeBundle();

        outy = await CompareDoctypesPrivate(siteName);

        return outy;
    }

    private async Task<DoubleDoctypeBundle> CompareDoctypesPrivate(string siteName)
    {
        // Here we are comparing on Alias - and ignoring Key and Id
        DoubleDoctypeBundle outcome = new DoubleDoctypeBundle();

        var composite = new DoctypeBundle();
        var remote = new DoctypeBundle(); //= await GetRemote();
        var local = new DoctypeBundle(); //= GetDoctypeBundle();
        bool IsFound = false;

        List<DoubleDoctypeDTO> commondoctypes = new List<DoubleDoctypeDTO>();
        List<DoubleDoctypeDTO> output = new List<DoubleDoctypeDTO>();
        List<DoctypeDTO> missingremote = new List<DoctypeDTO>();
        List<DoctypeDTO> missinglocal = new List<DoctypeDTO>();
        List<DoctypeDTO> localdoctypes = new List<DoctypeDTO>();
        List<DoctypeDTO> remotedoctypes = new List<DoctypeDTO>();

        remote = await GetRemoteDoctypes(siteName);
        if (remote.Success)
        {
            local = GetDoctypeBundleLocal();
            localdoctypes.AddRange(local.Doctypes);
            remotedoctypes.AddRange(remote.Doctypes);

            for (int a = 0; a < localdoctypes.Count; a++)
            {
                IsFound = false;
                for (int i = 0; i < remotedoctypes.Count; i++)
                {
                    // If local one is in Doctype list
                    if (localdoctypes[a].Alias == remotedoctypes[i].Alias)
                    {
                        // Its common to both
                        IsFound = true;

                        commondoctypes.Add(new DoubleDoctypeDTO
                        {
                            Local = localdoctypes[a],
                            Remote = remotedoctypes[i]
                        });
                        break;
                    }
                }
                // If local one is not in remote list - then tag it as missing from remote
                if (!IsFound)
                {
                    missingremote.Add(localdoctypes[a]);
                    output.Add(new DoubleDoctypeDTO { Local = localdoctypes[a], ExistLocal = 1 });
                }
            }

            for (int i = 0; i < remotedoctypes.Count; i++)
            {
                IsFound = false;
                for (int a = 0; a < localdoctypes.Count; a++)
                {
                    if (localdoctypes[a].Alias == remotedoctypes[i].Alias)
                    {
                        // Its common to both
                        IsFound = true;
                        break;
                    }
                }
                if (!IsFound)
                {
                    missinglocal.Add(remotedoctypes[i]);
                    output.Add(new DoubleDoctypeDTO { Remote = remotedoctypes[i], ExistRemote = 1 });
                }
            }


            for (int a = 0; a < commondoctypes.Count; a++)
            {
                commondoctypes[a].ExistLocal = 1;
                commondoctypes[a].ExistRemote = 1;
                commondoctypes[a].DiffId = (commondoctypes[a].Local.Id != commondoctypes[a].Remote.Id) ? 1 : 0;
                commondoctypes[a].DiffKey = (commondoctypes[a].Local.Key != commondoctypes[a].Remote.Key) ? 1 : 0;
                commondoctypes[a].DiffVbc = (commondoctypes[a].Local.Vbc != commondoctypes[a].Remote.Vbc) ? 1 : 0;
                commondoctypes[a].DiffItems = AreDoctypeItemsDifferent(commondoctypes[a].Local.Items, commondoctypes[a].Remote.Items) ? 1 : 0;

                if (commondoctypes[a].DiffId > 0 || commondoctypes[a].DiffKey > 0 || commondoctypes[a].DiffVbc > 0 || commondoctypes[a].DiffItems > 0)
                {
                    commondoctypes[a].Local.Status = PropTypeStatus.Different;
                    commondoctypes[a].Remote.Status = PropTypeStatus.Different;
                    output.Add(commondoctypes[a]);
                }
            }

            outcome.Doctypes = output;
        }
        else
        {
            outcome.Success = false;
            outcome.Message = "Unable to get remote doctypes.";
        }

        return outcome;

    }


    private bool AreDoctypeItemsDifferent(List<DoctypeElementDTO> local, List<DoctypeElementDTO> remote)
    {
        bool different = false;
        int localcount = 0;
        int remotecount = 0;
        bool IsFound = false;

        if (local != null)
        {
            localcount = local.Count;
        }
        if (remote != null)
        {
            remotecount = remote.Count;
        }

        if (localcount != remotecount)
        {
            return true;
        }
        // find the differences between the two lists


        foreach (var prop in local)
        {
            var remoteProp = remote.FirstOrDefault(x => x.Alias == prop.Alias);
            if (remoteProp == null)
            {
                return true;
            }
            else
            {
                if (prop.DatabaseType != remoteProp.DatabaseType || prop.DataTypeKey != remoteProp.DataTypeKey || prop.Vbc != remoteProp.Vbc)
                {
                    return true;
                }
            }
        }

        foreach (var remoteProp in remote)
        {
            var localProp = local.FirstOrDefault(x => x.Alias == remoteProp.Alias);
            if (localProp == null)
            {
                return true;
            }
        }

        return different;
    }


    private async Task<DoctypeBundle> GetRemoteDoctypes(string sitename)
    {
        var outy = new DoctypeBundle();

        DoctypeBundle incoming = new DoctypeBundle();

        string site = "";
        string apiUrl = ""; // "http://umbraco13a4.local/getdoctypesauth"; // https://epiris2024.local/GetDocTypes";
        if (_dtuSettings.Sites != null && _dtuSettings.Sites.Count > 0)
        {
            var siteObjs = _dtuSettings.Sites;
            var siteObj = siteObjs.Where(x => x.Name == sitename).FirstOrDefault();
            if (siteObj != null)
            {
                if (siteObj.Url.EndsWith("/"))
                {
                    site = siteObj.Url.Substring(0, site.Length - 1);
                }
                else
                {
                    site = siteObj.Url;
                }
            }
        }

        outy.Message += $"sitename:{sitename} Site: {site} - Sites.Count: {_dtuSettings.Sites.Count}";

        if (site.Length > 0)
        {
            apiUrl = $"{site}/umbraco/api/DoctypeUtility/GetDoctypesAuth";

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    // Your token value


                    // Add token to request headers
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _dtuSettings.AuthToken);

                    // Make GET request (change HttpMethod as needed)
                    HttpResponseMessage response = await client.GetAsync(apiUrl);

                    // Check if request is successful
                    if (response.IsSuccessStatusCode)
                    {
                        // Read response content
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // Deserialize JSON response
                        incoming = JsonConvert.DeserializeObject<DoctypeBundle>(responseBody);

                        outy.Doctypes = incoming.Doctypes;
                    }
                    else
                    {
                        outy.Success = false;
                        outy.Message = "Failed to call API. Status code: " + response.StatusCode;
                    }
                }
                catch (Exception ex)
                {
                    outy.Success = false;
                    if (ex.InnerException != null)
                    {
                        outy.Message += $"{ex.Message} - {ex.InnerException.Message}";
                    }
                    else
                    {
                        outy.Message += $"{ex.Message}";
                    }
                }
            }

        }
        return outy;
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
            Vbc = (int)x.Variations, // x.VariesByCulture() ? 1 : 0,
            Items = x.PropertyTypes.Select(y => new DoctypeElementDTO
            {
                Id = y.Id,
                Name = y.Name,
                Alias = y.Alias,
                PropertyEditorAlias = y.PropertyEditorAlias,
                DataTypeId = y.DataTypeId,
                DataTypeKey = y.Key,
                DatabaseType = y.ValueStorageType,
                Vbc = (int)y.Variations //y.VariesByCulture() ? 1 : 0
            }).ToList()
        });

        outy.Doctypes.AddRange(doctypes);
        return outy;
    }
}
