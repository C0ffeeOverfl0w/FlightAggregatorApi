﻿global using FlightAggregator.Application.Features.Bookings.Commands;
global using FlightAggregator.Application.Features.Flights.Queries;
global using FlightAggregator.Infrastructure.Authentication;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using FlightAggregator.Infrastructure.Extensions;
global using FlightAggregator.Application.Extensions;
global using FlightAggregator.Infrastructure.Logging;
global using Microsoft.AspNetCore.Authorization;
global using FlightAggregator.Application.DTOs;
global using FlightAggregator.Api.Middleware;
global using FlightAggregator.Api.Extensions;
global using Microsoft.IdentityModel.Tokens;
global using Microsoft.OpenApi.Models;
global using Microsoft.AspNetCore.Mvc;
global using FluentValidation;
global using Serilog.Context;
global using System.Text;
global using MediatR;
global using Serilog;