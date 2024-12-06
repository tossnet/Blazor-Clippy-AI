using Microsoft.AspNetCore.Components;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Routing;
using BlazorClippyAI.Components.Agents.Plugin;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace BlazorClippyAI.Components.Agents;


public partial class Clippy : IAsyncDisposable, IDisposable
{
    [Inject] private IJSRuntime Js { get; set; } = default!;
    [Inject] private NavigationManager NavManager { get; set; } = default!;

    private IJSObjectReference JSClippy { get; set; } = default!;
    private IJSObjectReference JSDragDrop { get; set; } = default!;
    private IJSObjectReference JsSpeak { get; set; } = default!;
    private IChatCompletionService ChatService { get; set; }
    private Kernel KernelService { get; set; }
    private IDisposable _registration;
    private PromptExecutionSettings _settings;
    private ChatHistory _history = new();
    private string _message = string.Empty;
    private string _answers = "Bonjour !";

    public Clippy(IChatCompletionService chatService, Kernel kernel)
    {
        ChatService = chatService;
        KernelService = kernel;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        JSClippy ??= await Js.InvokeAsync<IJSObjectReference>("import", "./Components/Agents/Clippy.razor.js");
        JSDragDrop ??= await Js.InvokeAsync<IJSObjectReference>("import", "./draganddrop-2.js");
        JsSpeak ??= await Js.InvokeAsync<IJSObjectReference>("import", "./js/speak.js");

        await JSClippy.InvokeVoidAsync("initializeClippy");
        await JSDragDrop.InvokeVoidAsync("makeClippyDraggable");
    }

    protected override async Task OnInitializedAsync()
    {
        _registration = NavManager.RegisterLocationChangingHandler(LocationChangingHandler);

        KernelService.ImportPluginFromType<NavigationPlugin>();

        _settings = new OpenAIPromptExecutionSettings()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
        };

        _history.AddUserMessage("Tu t'appelles Clippy. Tu étais un compagnon Office et tu es né fin de l'année 1996");

        _history.AddUserMessage("La page Counter est un exemple Blazor qui explique simplement le principe d'interactivité avec un bouton et un compteur.");
        _history.AddUserMessage("La page Wheather est un exemple Blazor qui simule le chargement de donnée via une API et affiche des données fictives de météo.");
    }

    private async Task SendMessage()
    {
        _history.AddUserMessage(_message);

        ChatMessageContent assistant;
        try
        {
            assistant = await ChatService.GetChatMessageContentAsync(_history, _settings, KernelService);
        }
        catch (Exception)
        {
            throw;
        }

        _answers = assistant.ToString();
        _message = string.Empty;

        StateHasChanged();
    }

    private async ValueTask LocationChangingHandler(LocationChangingContext context)
    {
        if (!context.IsNavigationIntercepted)
            return;

        _history.AddUserMessage("oubli ma précédante navigation");

        string page = GetPath(context.TargetLocation);

        _history.AddUserMessage($"Je navigue dans la page {page}. Fait une réponse courte");

        _ = SendMessage();
    }

    static string GetPath(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return string.Empty;

        Uri? uri;
        if (Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri))
        {
            if (!uri.IsAbsoluteUri)
            {
                return url;
            }

            return uri.AbsolutePath;
        }
        else
        {
            return url;
        }
    }


    
    public async Task ValideMessage(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
        {
            await SendMessage();
        }
    }

    private async Task ClippySpeak()
    {
        await JsSpeak.InvokeVoidAsync("speak", _answers);
    }

    public async ValueTask DisposeAsync()
    {

        if (JSClippy is not null)
        {
            try
            {
                await JSClippy.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                throw;
            }
        }

        if (JSDragDrop is not null)
        {
            try
            {
                await JSDragDrop.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                throw;
            }
        }

        if (JsSpeak is not null)
        {
            try
            {
                await JsSpeak.DisposeAsync();
            }
            catch (JSDisconnectedException)
            {
                throw;
            }
        }

        
    }

    public void Dispose() => _registration?.Dispose();

}
