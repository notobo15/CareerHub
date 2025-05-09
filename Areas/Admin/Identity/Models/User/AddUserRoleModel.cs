using System.Collections.Generic;
using System.ComponentModel;
using RecruitmentApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RecruitmentApp.Areas.Admin.Identity.Models.UserViewModels
{
  public class AddUserRoleModel
  {
    public AppUser user { get; set; }

    [DisplayName("Các role gán cho user")]
    public string[] RoleNames { get; set; }

    public List<IdentityRoleClaim<string>> claimsInRole { get; set; }
    public List<IdentityUserClaim<string>> claimsInUserClaim { get; set; }

  }
}