using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score
{
    public int enemyCount = 0;
    public int startCount = 0;
    public int campCount = 0;
    public bool starSearchHelp = true;
    public bool keySearchHelp = true;

    public Score(settingsProfile settings)
    {
        enemyCount = settings.enemiesCount;
        startCount = settings.starCount;
        campCount = settings.campCount;
        starSearchHelp = settings.starSearchHelp;
        keySearchHelp = settings.keySearchHelp;
    }
}
