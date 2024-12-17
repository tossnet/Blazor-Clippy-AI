using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.Routing;

namespace BlazorClippyAI.Agents;

public partial class Clippy : IAsyncDisposable, IDisposable
{
    [Inject] private IJSRuntime Js { get; set; } = default!;
    [Inject] private NavigationManager NavManager { get; set; } = default!;
    [Inject] private ClippyService _ClippyService { get; set; } = default!;

    private IJSObjectReference? JSClippy;
    private IJSObjectReference? JSDragDrop;
    private IJSObjectReference? JsSpeak;

    private IDisposable _registration;
    private string _message = string.Empty;
    private string _answers = "Bonjour !";


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
    }

    private async Task SendMessage()
    {
        _answers = await _ClippyService.GetResponse(_message);

        _message = string.Empty;

        StateHasChanged();
    }

    private async ValueTask LocationChangingHandler(LocationChangingContext context)
    {
        if (!context.IsNavigationIntercepted)
            return;

        _ClippyService.AddHistory("oubli ma précédante navigation");

        string page = GetPath(context.TargetLocation);

        _ClippyService.AddHistory($"Je navigue dans la page {page}. Fait une réponse courte");

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
