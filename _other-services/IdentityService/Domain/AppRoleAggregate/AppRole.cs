using Microsoft.AspNetCore.Identity;

namespace IdentityService.Domain.AppRoleAggregate;

public class AppRole(string name) : IdentityRole(name);