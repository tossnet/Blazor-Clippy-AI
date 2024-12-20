﻿using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

namespace BlazorClippyAI.Agents;

public class ClippyService
{
    private IChatCompletionService ChatService;
    private Kernel KernelService;
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

        _history.AddSystemMessage("Tu t'appelles Clippy. Tu étais un compagnon Office et tu es né fin de l'année 1996");

        _history.AddSystemMessage("La page Counter est un exemple Blazor qui explique simplement le principe d'interactivité avec un bouton et un compteur.");
        _history.AddSystemMessage("La page Wheather est un exemple Blazor qui simule le chargement de donnée via une API et affiche des données fictives de météo.");
    }

    public async Task<string> GetResponse(string message)
    {
        _history.AddUserMessage(message);

        string response;

        try
        {
            ChatMessageContent assistant = await ChatService.GetChatMessageContentAsync(_history, _settings, KernelService); 
            response = assistant.ToString();
        }
        catch (Exception)
        {
            response = "Je suis désolé, j'ai rencontré une erreur inattendue.";
        }

        return response;
    }

    public void AddHistory(string message)
    {
        _history.AddUserMessage(message);
    }
}
