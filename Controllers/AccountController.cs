using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Multi_Tenant_API.Data;
using Multi_Tenant_API.Dtos;
using Multi_Tenant_API.Models;

[ApiController]
[Route("accounts")]
public class AccountController : ControllerBase
{
  private AppDbContext _context;
  private IMapper _mapper;

  public AccountController(AppDbContext context, IMapper mapper)
  {
    _context = context;
    _mapper = mapper;
  }

  [HttpGet]
  public void ListTenantAccounts()
  {
    // Permission: All
  }
  
  [HttpPatch("{id}/role")]
  public void UpdateAccountRole()
  {
    // Permission: Admin
  }
}