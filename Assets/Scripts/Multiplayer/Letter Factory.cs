using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public static class LetterFactory
{
    private static LinkedPool<Letter> pooledLetters = new LinkedPool<Letter>(() => new Letter(), (letter) => letter.Clear());

    public static Letter Get()
    {
        return pooledLetters.Get();
    }

    public static void Release(Letter letter)
    {
        pooledLetters.Release(letter);
    }
}
