using System;

namespace LinkUp.Application;
public record UserDto(
    Guid Id,
    string Username,
    // string SenderName,
    string UniqueName
);