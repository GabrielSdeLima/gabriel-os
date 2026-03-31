using GabrielOS.Application.Rules;
using GabrielOS.Domain.Enums;

namespace GabrielOS.Tests;

public class ModeCalculatorTests
{
    [Theory]
    [InlineData(1, 5, 5, 5)]  // energy <= 3
    [InlineData(3, 5, 5, 5)]  // energy == 3
    [InlineData(5, 5, 5, 8)]  // tension >= 8
    [InlineData(5, 5, 5, 10)] // tension == 10
    [InlineData(2, 2, 2, 10)] // both
    public void Calculate_ReturnsProtect_WhenLowEnergyOrHighTension(int energy, int mood, int clarity, int tension)
    {
        var result = ModeCalculator.Calculate(energy, mood, clarity, tension);
        Assert.Equal(SuggestedMode.Protect, result);
    }

    [Theory]
    [InlineData(4, 5, 4, 5)]  // clarity <= 4 && energy <= 5
    [InlineData(5, 5, 1, 5)]  // clarity == 1
    [InlineData(4, 5, 4, 7)]  // borderline tension (not >= 8)
    public void Calculate_ReturnsSimplify_WhenLowClarityAndModerateEnergy(int energy, int mood, int clarity, int tension)
    {
        var result = ModeCalculator.Calculate(energy, mood, clarity, tension);
        Assert.Equal(SuggestedMode.Simplify, result);
    }

    [Theory]
    [InlineData(7, 5, 7, 4)]  // all thresholds met exactly
    [InlineData(10, 5, 10, 0)] // max energy, max clarity, min tension
    [InlineData(9, 5, 8, 3)]  // above thresholds
    public void Calculate_ReturnsExpand_WhenHighEnergyAndClarityLowTension(int energy, int mood, int clarity, int tension)
    {
        var result = ModeCalculator.Calculate(energy, mood, clarity, tension);
        Assert.Equal(SuggestedMode.Expand, result);
    }

    [Theory]
    [InlineData(5, 5, 5, 5)]  // normal state
    [InlineData(6, 5, 6, 5)]  // above simplify thresholds, below expand
    [InlineData(7, 5, 7, 5)]  // high energy/clarity but tension = 5 (> 4), so not Expand
    public void Calculate_ReturnsFocus_ForNormalState(int energy, int mood, int clarity, int tension)
    {
        var result = ModeCalculator.Calculate(energy, mood, clarity, tension);
        Assert.Equal(SuggestedMode.Focus, result);
    }

    [Fact]
    public void GetModeDescription_ReturnsNonEmptyString_ForAllModes()
    {
        foreach (SuggestedMode mode in Enum.GetValues<SuggestedMode>())
        {
            var description = ModeCalculator.GetModeDescription(mode);
            Assert.False(string.IsNullOrWhiteSpace(description));
        }
    }
}
