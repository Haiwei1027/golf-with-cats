

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
    CREATETOWN
}
