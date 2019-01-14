using System.Collections.Generic;
using JK.Roles.Dto;

namespace JK.Web.Models.Common
{
    public interface IPermissionsEditViewModel
    {
        List<FlatPermissionDto> Permissions { get; set; }
    }
}