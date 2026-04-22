using Lifes.Application.Features.VersionIncrease.Commands;
using Lifes.Core.Interfaces;
using Lifes.Infrastructure.Common.Configuration;
using Lifes.Infrastructure.Features.VersionIncrease.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Add CORS to allow Electron/Angular frontend (usually localhost:4200) to connect
builder.Services.AddCors(options =>
{
    options.AddPolicy("ElectronVuePolicy",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

// 2. Register Application & Infrastructure Services (similar to WPF's App.xaml.cs)
builder.Services.AddSingleton<IProjectScanner, ProjectScanner>();
builder.Services.AddSingleton<IProjectFileService, ProjectFileService>();
builder.Services.AddSingleton<IVersionService, VersionService>();
builder.Services.AddSingleton<ISettingsService, SettingsService>();
builder.Services.AddSingleton<IGitService, Lifes.Infrastructure.Features.VersionIncrease.Git.GitService>();

builder.Services.AddTransient<IScanProjectsCommand, ScanProjectsCommand>();
builder.Services.AddTransient<IUpdateVersionsCommand, UpdateVersionsCommand>();

// 3. Add Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("ElectronVuePolicy");
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
