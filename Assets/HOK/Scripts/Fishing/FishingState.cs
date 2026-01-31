namespace HOK.Fishing
{
    /// <summary>
    /// States for the fishing system state machine.
    /// </summary>
    public enum FishingState
    {
        Inactive = 0,       // Not fishing - movement enabled
        Idle = 1,           // Ready to cast - can aim
        Casting = 2,        // Cast animation playing
        LineInWater = 3,    // Waiting for fish bite
        FishBiting = 4,     // Fish is biting - hook window active
        Hooked = 5,         // Fish hooked - reeling phase
        CatchResolution = 6 // Success/fail animation
    }
}
