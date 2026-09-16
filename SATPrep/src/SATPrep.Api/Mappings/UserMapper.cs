using System;
using SATPrep.Api.Entities;
using SATPrep.Api.DTOs;

namespace SATPrep.Api.Mappings;

public static class UserMapper
{
    public static UserGetDto ToGetDto(this User entity)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));

        return new UserGetDto(
            entity.UserId,
            entity.Name ?? string.Empty,
            entity.Email ?? string.Empty,
            entity.Role.ToString(),
            entity.CreatedAt,
            entity.StudyStreak,
            entity.TotalStudyMinutes,
            entity.LastStudiedAt
        );
    }

    public static User UpdateFrom(this User entity, UserUpdateDto dto, string? newPasswordHash = null)
    {
        if (entity is null) throw new ArgumentNullException(nameof(entity));
        if (dto is null) throw new ArgumentNullException(nameof(dto));

        if (dto.Name is not null)
            entity.Name = dto.Name.Trim();

        if (dto.Email is not null)
            entity.Email = dto.Email.Trim().ToLowerInvariant();

        if (!string.IsNullOrEmpty(newPasswordHash))
            entity.Password = newPasswordHash;

        return entity;
    }
}