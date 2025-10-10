using UnityEngine;
public enum SeasonMask
{
    None = 0,
    Spring = 1 << 0,
    Summer = 1 << 1,
    Fall = 1 << 2,
    Winter = 1 << 3,
    All = Spring | Summer | Fall | Winter
}

public static class SeasonExtensions
{
    public static bool Has(this SeasonMask mask, SeasonState s) => (mask & s.ToMask()) != 0;
    public static SeasonMask ToMask(this SeasonState s) => (SeasonMask)(1 << (int)s);
}