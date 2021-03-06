﻿using AutoMapper;
using FollowUP.Core.Domain;
using FollowUP.Infrastructure.DTO;
using FollowUP.Infrastructure.Extensions;
using FollowUP.Infrastructure.Settings;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FollowUP.Infrastructure.Services
{
    public class JwtHandler : IJwtHandler
    {
        private readonly JwtSettings _settings;
        private readonly IMapper _mapper;

        public JwtHandler(JwtSettings settings, IMapper mapper)
        {
            _settings = settings;
            _mapper = mapper;
        }

        public JwtDto CreateToken(Guid userId, string role)
        {
            var now = DateTime.UtcNow;
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, userId.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, now.ToTimestamp().ToString(), ClaimValueTypes.Integer64)
            };

            var expires = now.AddMinutes(_settings.ExpiryMinutes);
            var signingCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Key)),
                SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                issuer: _settings.Issuer,
                claims: claims,
                notBefore: now,
                expires: expires,
                signingCredentials: signingCredentials
            );
            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            var jsonWebToken = new JsonWebToken
            {
                AccessToken = token,
                Expires = expires.ToTimestamp()
            };

            return _mapper.Map<JwtDto>(jsonWebToken);
        }
    }
}
