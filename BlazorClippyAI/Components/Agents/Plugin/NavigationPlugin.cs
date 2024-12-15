using Microsoft.AspNetCore.Components;
using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace BlazorClippyAI.Agents;

public class NavigationPlugin
{
    private NavigationManager NavigationManager { get; set; }

    public NavigationPlugin(NavigationManager navigationManager)
    {
        NavigationManager = navigationManager;
    }

    [KernelFunction("Go_to_weather_information")]
    [Description("affiche la page avec les informations de la météo")]
    public void GotoWeather()
    {
        NavigationManager.NavigateTo("weather");
    }


    [KernelFunction("Go_to_Counter")]
    [Description("affiche la page avec un compteur")]
    public void GotoCounter()
    {
        NavigationManager.NavigateTo("counter");
    }
}
