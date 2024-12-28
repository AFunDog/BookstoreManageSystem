using Avalonia;
using Avalonia.Controls.Primitives;
using Avalonia.Data;

namespace BookstoreManageSystem.Avalonia.Controls;

public class CustomTitleBar : TemplatedControl
{
    public static readonly StyledProperty<string> TitleProperty = AvaloniaProperty.Register<
        CustomTitleBar,
        string
    >(nameof(Title), "");

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
}
