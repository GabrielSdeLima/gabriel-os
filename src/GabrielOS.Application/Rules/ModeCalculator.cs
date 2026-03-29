using GabrielOS.Domain.Enums;

namespace GabrielOS.Application.Rules;

public static class ModeCalculator
{
    public static SuggestedMode Calculate(int energy, int mood, int clarity, int tension)
    {
        if (energy <= 3 || tension >= 8)
            return SuggestedMode.Protect;

        if (clarity <= 4 && energy <= 5)
            return SuggestedMode.Simplify;

        if (energy >= 7 && clarity >= 7 && tension <= 4)
            return SuggestedMode.Expand;

        return SuggestedMode.Focus;
    }

    public static string GetModeDescription(SuggestedMode mode) => mode switch
    {
        SuggestedMode.Protect => "Low energy or high tension. Minimize demands, protect your base, do only what's essential.",
        SuggestedMode.Simplify => "Low clarity. Pick one thing and give it your attention. Cut everything else today.",
        SuggestedMode.Expand => "High energy and clarity. Good day to plan, explore, or tackle something ambitious.",
        SuggestedMode.Focus => "Stable state. Execute on your priorities, close open cycles.",
        _ => "Focus on what matters most today."
    };
}
