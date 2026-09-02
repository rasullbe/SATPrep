using System;
using SATPrep.Api.Entities;

namespace SATPrep.Api.DTOs;

public record UserGetDto(
    long UserId,
    string Name,
    string Email,
    Role Role,
    DateTime CreatedAt
);
