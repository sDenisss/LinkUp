using LinkUp.Domain;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LinkUp.Application;
public record RegisterUserCommand(
    string Username,
    string Email,
    string Password,
    string UniqueName) : IRequest<Result>;
