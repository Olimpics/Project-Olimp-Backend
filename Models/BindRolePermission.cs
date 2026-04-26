using System;
using System.Collections.Generic;

namespace OlimpBack.Models;

public partial class Bindrolepermission
{
    public int? IdBindRolePermission { get; set; }

    public int? RoleId { get; set; }

    public int? PermissionId { get; set; }
}
