using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Globals
{
    public static float distIncrement = 0;
    public static GameObject player;
    public static settingsProfile mainSettings = new settingsProfile();
    public static readonly Vector2 origScreenSize = new Vector2(800, 480);
    public static bool isFirstLoad = false;

    public static AudioManager audioManager;


    public static void Shuffle<T>(this IList<T> list) {
        int n = list.Count;
        while (n > 1) {
            n--;
            int k = UnityEngine.Random.Range(0, n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    public static GameObject GetObjectAtPosition(Vector3 pos, string tag) {
        RaycastHit2D[] colliders = Physics2D.RaycastAll(pos, Vector3.zero);
        Debug.Log(pos);
        if ((colliders).Length >= 1) {
            foreach (var item in colliders) {
                if (item.transform.tag == tag)
                    return item.transform.gameObject;
            }
        }
        return null;
    }

    public static void GoToScene(string name) {
        SceneManager.LoadScene(name);
    }
    public static float vectorToAngle(Vector3 dir) {
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        return angle;
    }
    public static float angleToPostive(float angle) {
        return (angle + 360) % 360;
    }

}
public class Pair<T, U>
{
    public Pair() {
    }

    public Pair(T first, U second) {
        this.First = first;
        this.Second = second;
    }


    public T First { get; set; }
    public U Second { get; set; }
}

