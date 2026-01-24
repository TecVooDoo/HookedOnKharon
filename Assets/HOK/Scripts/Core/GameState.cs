namespace HOK.Core
{
    /// <summary>
    /// Core game states for Hooked on Kharon.
    /// Int values used for SOAP IntVariable compatibility.
    /// </summary>
    public enum GameState
    {
        OffDuty = 0,    // Kharon is fishing in peace
        Fishing = 1,    // Actively fishing
        Ferrying = 2,   // Transporting souls
        InMenu = 3      // UI menu is open
    }
}
