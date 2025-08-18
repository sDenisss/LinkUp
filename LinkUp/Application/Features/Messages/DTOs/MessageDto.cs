using System;

namespace LinkUp.Application;
public record MessageDto(
    Guid Id,
    Guid SenderId,
    string SenderUniqueName,
    string Content,
    DateTime Timestamp
);