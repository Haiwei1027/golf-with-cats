
/// <summary>
/// Enum for cataloging each letter format
/// </summary>
public enum LetterType : byte
{
    /// <summary>
    /// (ushort userId), Server send to a user when they connect
    /// </summary>
    WELCOME,
    /// <summary>
    /// (string username), User send when they received welcome
    /// </summary>
    INTRODUCE,

    /// <summary>
    /// (), User send when wants to create a lobby
    /// </summary>
    CREATETOWN,
    /// <summary>
    /// (int townId), User send when wants to join a lobby
    /// </summary>
    JOINTOWN,
    /// <summary>
    /// (int townId, ushort newResidentId, int population, foreach resident [ushort Id, string username, int colourId]), 
    /// Server send to all lobby users when a new user join
    /// </summary>
    TOWNWELCOME,
    /// <summary>
    /// (), User send when they want to leave their lobby
    /// </summary>
    LEAVETOWN,
    /// <summary>
    /// (ushort leftResidentId), Server send to all lobby users when one leaves
    /// </summary>
    GOODBYE,
    /// <summary>
    /// (), Server send to all lobby users when the game begins
    /// </summary>
    STARTGAME,

    /// <summary>
    /// (ushort hologramId, ushort prefabId, int ownerId, byte hologramType, ...), Both send when a new network object is spawned
    /// </summary>
    HOLOGRAMCREATE,
    /// <summary>
    /// (ushort hologramId, ...), Both send when a network object is updated
    /// </summary>
    HOLOGRAMUPDATE,
    /// <summary>
    /// (ushort hologramId), Both send when a network object is destroyed
    /// </summary>
    HOLOGRAMDESTROY,

}