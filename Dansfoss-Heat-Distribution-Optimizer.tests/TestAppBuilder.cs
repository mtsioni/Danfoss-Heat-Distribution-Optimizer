using Avalonia;
using Avalonia.Headless;
using Danfoss_Heat_Distribution_Optimizer;
using ReactiveUI;

[assembly: AvaloniaTestApplication(typeof(TestAppBuilder))]

public class TestAppBuilder
{
    public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
    .UseHeadless(new AvaloniaHeadlessPlatformOptions());
}