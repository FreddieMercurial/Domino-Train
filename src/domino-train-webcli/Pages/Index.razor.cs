using HACC.Components;
using Microsoft.AspNetCore.Components;
using Terminal.Gui;

namespace DominoTrain.WebCli.Pages;

public partial class Index : ComponentBase
{
    /// <summary>
    ///     This is NULL until after render
    /// </summary>
    private WebConsole? _webConsole;

    protected void InitApp()
    {
        if (this._webConsole is null)
            throw new InvalidOperationException(message: "_webConsole reference was not set");

        this._webConsole.WebApplication!.Shutdown();
        this._webConsole.WebApplication.Init();

        var label = new Label(text: "Enter your name:")
        {
            X = Pos.Center(),
            Y = 0,
        };
        var text = new TextField("gui.cs:")
        {
            X = Pos.Center(),
            Y = 2,
            Width = 20,
        };
        var button = new Button(text: "Say Hello")
        {
            X = Pos.Center(),
            Y = 4
        };
        button.Clicked += () => MessageBox.Query("Say Hello", $"Welcome {text.Text}", "Ok");
        var text2 = new TextField("this is horiz/vert centered")
        {
            X = Pos.Center(),
            Y = Pos.Center(),
            Width = 30,
        };
        var lblMouse = new Label()
        {
            Y = Pos.Center() + 2,
            Height = 2,
            AutoSize = true
        };
        var mouseCount = 0;
        Application.RootMouseEvent = (e) =>
        {
            lblMouse.Text = $"Mouse: X:{e.X};Y:{e.Y};Button:{e.Flags};\nView:{e.View};Count:{++mouseCount}";
        };
        var lblKey = new Label()
        {
            Y = Pos.Center() + 5,
            Height = 2,
            AutoSize = true
        };
        var keyCount = 0;
        Application.RootKeyEvent = (e) =>
        {
            var mk = ShortcutHelper.GetModifiersKey(e);
            lblKey.Text = $"Key:{e.Key};KeyValue:{e.KeyValue};KeyChar:{(char) e.KeyValue}\nAlt:{mk.HasFlag(Key.AltMask)};Ctrl:{mk.HasFlag(Key.CtrlMask)};Shift:{mk.HasFlag(Key.ShiftMask)};Count:{++keyCount}";
            return false;
        };

        var win = new Window("HACC Demo")
        {
            Width = Dim.Fill(),
            Height = Dim.Fill()
        };

        win.Add(label, text, button, text2, lblMouse, lblKey);
        Application.Top.Add(win);
        this._webConsole.WebApplication.Run();
    }
}