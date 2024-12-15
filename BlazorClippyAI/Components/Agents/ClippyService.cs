using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Runtime;

namespace BlazorClippyAI.Agents;

public class ClippyService
{
    private IChatCompletionService ChatService { get; set; } = default!;
    private Kernel KernelService { get; set; } = default!;

    private PromptExecutionSettings _settings;
    private ChatHistory _history = new();

    public ClippyService(IChatCompletionService chatService,
                     Kernel kernel)
    {
        KernelService = kernel;
        ChatService = chatService;

        KernelService.ImportPluginFromType<NavigationPlugin>();

        _settings = new OpenAIPromptExecutionSettings()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
        };

        _history.AddUserMessage("Tu t'appelles Clippy. Tu étais un compagnon Office et tu es né fin de l'année 1996");

        _history.AddUserMessage("La page Counter est un exemple Blazor qui explique simplement le principe d'interactivité avec un bouton et un compteur.");
        _history.AddUserMessage("La page Wheather est un exemple Blazor qui simule le chargement de donnée via une API et affiche des données fictives de météo.");
    }

    public async Task<string> GetResponse(string message)
    {
        _history.AddUserMessage(message);

        ChatMessageContent assistant;
        try
        {
            assistant = await ChatService.GetChatMessageContentAsync(_history, _settings, KernelService);
        }
        catch (Exception)
        {
            throw;
        }

        return assistant.ToString();
    }

    public void AddHistory(string message)
    {
        _history.AddUserMessage(message);
    }
}
