using Microsoft.Maui;
using Microsoft.Maui.Hosting;

namespace MoulesMachines;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<MoulesApp>();

        return builder.Build();
    }
}
