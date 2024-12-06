using BlazorClippyAI.Components;
using Microsoft.SemanticKernel;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();


// --- SemanticKernel part
string azureEndpoint = builder.Configuration["AI:OpenAI:Endpoint"];
string azureApiKey = builder.Configuration["AI:OpenAI:Key"];
string deploymentName = builder.Configuration["AI:OpenAI:deploymentName"];

builder.Services.AddAzureOpenAIChatCompletion(deploymentName, azureEndpoint, azureApiKey);
builder.Services.AddKernel();

//builder.Services.AddLogging(services => services.AddConsole().SetMinimumLevel(LogLevel.Trace));
// ---

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
