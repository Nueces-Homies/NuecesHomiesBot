using Core.Database;

namespace Core.Test;

using FluentAssertions;

public class CrystalTest
{
    [Fact]
    public void RapidGenerationIsMonotonicallyIncreasing()
    {
        var crystals = new Crystal[10000];
        for (var i = 0; i < crystals.Length; i++)
        {
            crystals[i] = Crystal.New();
        }

        var nonRandomBits = crystals.Select(c => c.WithoutNoise()).ToArray();
        nonRandomBits.Should().BeInAscendingOrder();
    }

    [Fact]
    public void SlowGenerationIsStrictlyIncreasing()
    {
        var crystals = new Crystal[50];
        for (var i = 0; i < crystals.Length; i++)
        {
            crystals[i] = Crystal.New();
            Thread.Sleep(20);
        }

        var nonRandomBits = crystals.Select(c => c.WithoutNoise());
        nonRandomBits.Should().BeInAscendingOrder().And.OnlyHaveUniqueItems();
    }
}

file static class CrystalUtils
{
    public static ulong WithoutNoise(this Crystal crystal)
    {
        ulong mask = 0xFFFF;
        mask = ~mask;
        return crystal.Value & mask;
    }
}