namespace Serene.Services;


/// <summary>
/// Manages the application theme (Light or Dark) for the Serene app.
/// </summary>
/// <remarks>
/// This service allows toggling between light and dark themes, retrieving
/// the current theme, and setting a specific theme. It exposes an <see cref="OnChange"/>
/// event to notify subscribers whenever the theme changes, enabling the UI
/// to react dynamically to theme updates.
/// </remarks>
public class ThemeService
{
    public enum Theme
    {
        Light,
        Dark
    }

    private Theme currentTheme = Theme.Light; //default
    public event Action? OnChange;

    public Theme GetCurrentTheme() => currentTheme;

    public void SetCurrentTheme(Theme theme)
    {
        if (currentTheme != theme)
        {
            currentTheme = theme;
            NotifyStateChanged();
        }
    }

    public void ToggleTheme()
    {
        currentTheme = currentTheme == Theme.Light ? Theme.Dark : Theme.Light;
        NotifyStateChanged();
    }
    private void NotifyStateChanged() => OnChange?.Invoke();
}