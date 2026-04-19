namespace OlimpBack.Utils;

public static class PermissionMaskHelper
{
    public const int MinBitIndex = 0;
    public const int MaxBitIndex = 62;

    public static long ToMask(int bitIndex)
    {
        ValidateBitIndex(bitIndex);
        return 1L << bitIndex;
    }

    public static long BuildMask(IEnumerable<int> bitIndexes)
    {
        long mask = 0;

        foreach (var bitIndex in bitIndexes)
            mask |= ToMask(bitIndex);

        return mask;
    }

    public static bool HasAny(long userMask, long requiredMask) =>
        (userMask & requiredMask) != 0;

    public static bool HasAll(long userMask, long requiredMask) =>
        (userMask & requiredMask) == requiredMask;

    private static void ValidateBitIndex(int bitIndex)
    {
        if (bitIndex < MinBitIndex || bitIndex > MaxBitIndex)
            throw new ArgumentOutOfRangeException(nameof(bitIndex),
                $"Bit index must be in range [{MinBitIndex}, {MaxBitIndex}] for Int64 mask.");
    }
}
