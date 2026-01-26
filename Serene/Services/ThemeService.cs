namespace Serene.Services;

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