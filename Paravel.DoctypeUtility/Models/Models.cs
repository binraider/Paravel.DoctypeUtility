using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Sections;

namespace Paravel.DoctypeUtility.Models;

public class DoubleDoctypeBundle
{
    public List<DoubleDoctypeDTO> Doctypes { get; set; } = new List<DoubleDoctypeDTO>();
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; } = true;
}


public class DoubleDoctypeDTO
{
    public int DiffId { get; set; } = 0;
    public int DiffKey { get; set; } = 0;
    public int DiffItems { get; set; } = 0;
    public int ExistLocal { get; set; } = 0;
    public int ExistRemote { get; set; } = 0;
    public DoctypeDTO Local { get; set; } = new DoctypeDTO();
    public DoctypeDTO Remote { get; set; } = new DoctypeDTO();
}

public class DoctypeBundle
{
    public List<DoctypeDTO> Doctypes { get; set; } = new List<DoctypeDTO>();
    public string Message { get; set; } = string.Empty;
    public bool Success { get; set; } = true;
}

public class DoctypeDTO
{
    public PropTypeStatus Status { get; set; } = PropTypeStatus.None;
    public PropTypeStatus ItemsStatus { get; set; } = PropTypeStatus.None;
    public string Name { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
    public int Id { get; set; } = 0;
    public Guid Key { get; set; } = Guid.Empty;
    public List<DoctypeElementDTO> Items { get; set; }

    public DoctypeDTO()
    {
        Items = new List<DoctypeElementDTO>();
    }

}

public class DoctypeElementDTO
{
    public PropTypeStatus Status { get; set; } = PropTypeStatus.None;
    public int Id { get; set; } = 0;
    public string Name { get; set; } = string.Empty;
    public string Alias { get; set; } = string.Empty;
    public string PropertyEditorAlias { get; set; } = string.Empty;
    public int DataTypeId { get; set; } = 0;
    public Guid DataTypeKey { get; set; } = Guid.Empty;
    public ValueStorageType DatabaseType { get; set; } = ValueStorageType.Ntext;
}

public enum PropTypeStatus
{
    None = 0,
    MissingRemote = 1,
    MissingLocal = 2,
    Different = 3
}

public class DTUSettings
{
    public const string SectionName = "DTUSettings";
    public string AuthToken { get; set; } = string.Empty;
    public List<DTUSite> Sites { get; set; } = new List<DTUSite>();
}

public class DTUSite
{
    public bool Local { get; set; } = false;
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}

