using System;
using Veldrid;
using Veldrid.Sdl2;
using Veldrid.StartupUtilities;

namespace Engine.Core;

class Program
{
    private static RgbaFloat _clearColor = new(0.1f, 0.2f, 0.3f, 1f);
    private static Random _random = new();

    static void Main()
    {
        Console.WriteLine("🚀 BYK Engine v0.1 Starting...");

        var windowCI = new WindowCreateInfo
        {
            X = 100,
            Y = 100,
            WindowWidth = 1280,
            WindowHeight = 720,
            WindowTitle = "BYK Engine - DirectX 11 | F2: Color | F11: Fullscreen | ESC: Exit"
        };

        var graphicsOptions = new GraphicsDeviceOptions(
            debug: false,
            swapchainDepthFormat: null,  // Depth buffer YOK
            syncToVerticalBlank: false,
            resourceBindingModel: ResourceBindingModel.Improved,
            preferDepthRangeZeroToOne: true,
            preferStandardClipSpaceYDirection: true);

        VeldridStartup.CreateWindowAndGraphicsDevice(
            windowCI,
            graphicsOptions,
            GraphicsBackend.Direct3D11,
            out var window,
            out var graphicsDevice);

        Console.WriteLine($"✅ {graphicsDevice.BackendType} initialized");
        Console.WriteLine("🎮 Controls: ESC=Exit | F2=Random Color | F11=Fullscreen");

        var commandList = graphicsDevice.ResourceFactory.CreateCommandList();

        while (window.Exists)
        {
            ProcessInput(window);

            // Draw
            commandList.Begin();
            commandList.SetFramebuffer(graphicsDevice.SwapchainFramebuffer);
            commandList.ClearColorTarget(0, _clearColor);
            commandList.End();

            graphicsDevice.SubmitCommands(commandList);
            graphicsDevice.SwapBuffers();
        }

        commandList.Dispose();
        graphicsDevice.Dispose();
        Console.WriteLine("✅ Engine shutdown.");
    }

    static void ProcessInput(Sdl2Window window)
    {
        var input = window.PumpEvents();

        foreach (var key in input.KeyEvents)
        {
            if (key.Key == Key.Escape && key.Down)
            {
                Console.WriteLine("👋 Exiting...");
                window.Close();
            }

            if (key.Key == Key.F2 && key.Down)
            {
                // Random color
                _clearColor = new RgbaFloat(
                    (float)_random.NextDouble() * 0.5f + 0.1f,
                    (float)_random.NextDouble() * 0.5f + 0.1f,
                    (float)_random.NextDouble() * 0.5f + 0.1f,
                    1f);

                Console.WriteLine($"🎨 Color: R={_clearColor.R:F2}, G={_clearColor.G:F2}, B={_clearColor.B:F2}");
            }

            if (key.Key == Key.F11 && key.Down)
            {
                // Fullscreen toggle
                window.WindowState = window.WindowState == WindowState.Normal
                    ? WindowState.FullScreen
                    : WindowState.Normal;

                Console.WriteLine($"🖥️ Fullscreen: {window.WindowState}");
            }
        }
    }
}