

public enum LetterType : byte
{
    /// <summary>
    /// Letter for server to tell user they've connected and their client id
    /// </summary>
    WELCOME,
    /// <summary>
    /// Letter for user to tell the server their username
    /// </summary>
    INTRODUCE,
    /// <summary>
    /// Letter for user to create a lobby
    /// </summary>
    CREATETOWN,
    /// <summary>
    /// Letter for user to tell server they want to join a town
    /// </summary>
    JOINTOWN,
    /// <summary>
    /// Letter for server to update users on town population
    /// </summary>
    TOWNWELCOME,
    /// <summary>
    /// Letter for user to tell server they want to leave town
    /// </summary>
    LEAVETOWN,
    /// <summary>
    /// Letter for server to tell users who has just left
    /// </summary>
    GOODBYE,
    /// <summary>
    /// Letter for user to tell server to start game
    /// </summary>
    STARTGAME,
    HOLOGRAMCREATE,
    HOLOGRAMUPDATE,
    HOLOGRAMDESTROY,

}