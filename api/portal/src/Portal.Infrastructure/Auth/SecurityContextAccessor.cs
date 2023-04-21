﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Portal.Infrastructure.Auth;

public class SecurityContextAccessor : ISecurityContextAccessor
{
    private readonly ILogger<SecurityContextAccessor> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SecurityContextAccessor(
        ILogger<SecurityContextAccessor> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    public string UserId
    {
        get
        {
            var userId = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is not null)
                return userId;
            _logger.LogError("User ID not found in HttpContext");
            throw new Exception("User ID not found in HttpContext");
        }
    }

    public string JwtToken => _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].ToString().Replace("Bearer ", "") ?? string.Empty;

    public string RefreshToken => _httpContextAccessor.HttpContext?.Request.Headers["RefreshToken"].ToString() ?? string.Empty;

    public string IpAddress => _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? string.Empty;

    public bool IsAuthenticated
    {
        get
        {
            var isAuthenticated = _httpContextAccessor.HttpContext?.User.Identity?.IsAuthenticated;
            if (isAuthenticated is not null)
                return isAuthenticated.Value;
            _logger.LogError("IsAuthenticated not found in HttpContext");
            throw new Exception("IsAuthenticated not found in HttpContext");
        }
    }

    public string Role
    {
        get
        {
            var role = _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
            if (role is not null)
                return role;
            _logger.LogError("Role not found in HttpContext");
            throw new Exception("Role not found in HttpContext");
        }
    }

    public string Permission
    {
        get
        {
            var permission = _httpContextAccessor.HttpContext?.User.FindFirstValue("Permission");
            if (permission is not null)
                return permission;
            _logger.LogError("Permission not found in HttpContext");
            throw new Exception("Permission not found in HttpContext");
        }
    }
}