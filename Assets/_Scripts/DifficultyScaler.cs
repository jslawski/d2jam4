using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Difficulty { EASY, EASYMEDIUM, MEDIUM, MEDIUMHARD, HARD, VERYHARD }
public static class DifficultyScaler
{
    public static Difficulty currentDifficulty = Difficulty.EASY;

    public static void UpdateDifficulty()
    {
        float currentPlaytime = GameManager.instance.GetCurrentPlaytimeInSeconds();

        if (currentPlaytime > 150)
        {
            currentDifficulty = Difficulty.VERYHARD;
        }
        else if (currentPlaytime > 120)
        {
            currentDifficulty = Difficulty.HARD;
        }
        else if (currentPlaytime > 90)
        {
            currentDifficulty = Difficulty.MEDIUMHARD;
        }
        else if (currentPlaytime > 60)
        {
            currentDifficulty = Difficulty.MEDIUM;
        }
        else if (currentPlaytime > 30)
        {
            currentDifficulty = Difficulty.EASYMEDIUM;
        }
        else
        {
            currentDifficulty = Difficulty.EASY;
        }        
    }
}
