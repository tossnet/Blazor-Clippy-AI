using Microsoft.AspNetCore.Components;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.Web;

namespace BlazorClippyAI.Components.Agents;


public partial class Clippy : IAsyncDisposable
{
    [Inject]
    private IJSRuntime js { get; set; } = default!;

    private IJSObjectReference JSClippy { get; set; } = default!;
    private IJSObjectReference JSDragDrop { get; set; } = default!;

    private string _message = string.Empty;
    private string _answers = "Bonjour !";

    private ChatHistory _history = new();
    private IChatCompletionService ChatService { get; set; }

    public Clippy(IChatCompletionService chatService)
    {
        ChatService = chatService;
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        JSClippy ??= await js.InvokeAsync<IJSObjectReference>("import", "./Components/Agents/Clippy.razor.js");
        JSDragDrop ??= await js.InvokeAsync<IJSObjectReference>("import", "./draganddrop-2.js");

        await JSClippy.InvokeVoidAsync("initializeClippy");
        await JSDragDrop.InvokeVoidAsync("makeClippyDraggable");
    }
    protected override async Task OnInitializedAsync()
    {
        ////Kernel kernel = services.GetRequiredService<Kernel>();
        ////kernel.ImportPluginFromType<Planning>();

        //PromptExecutionSettings settings = new OpenAIPromptExecutionSettings()
        //{
        //    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
        //};

        _history.AddUserMessage("Tu t'appelles Clippy. Tu étais un compagnon Office et tu es né fin de l'année 1996");
    }

    private async Task sendMessage()
    {
        _history.AddUserMessage(_message);
        ChatMessageContent assistant = await ChatService.GetChatMessageContentAsync(_history);

        _answers = assistant.ToString();
        _message = string.Empty;

        StateHasChanged();
    }

    
    public async void ValideMessage(KeyboardEventArgs e)
    {
        if (e.Code == "Enter" || e.Code == "NumpadEnter")
        {
            await sendMessage();
        }
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
    }

}
