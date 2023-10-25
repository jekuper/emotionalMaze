using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TipsTrigger : MonoBehaviour
{
    [SerializeField] private TipsManager centerTip;
    private Dictionary<string, List<string>> triggers = new Dictionary<string, List<string>> () {
        {"spawned", new List<string> { "Oh crap!", "I'm here again, stuck with my fears and depression", "I don't even know if there is a way out from here" } },
        {"starFound", new List<string>{"I have no idea what this yellow object is", "I feel like I need it, if I want to move any further" } },
        {"starGot", new List<string>{"WOW, I feel, I can now move further from my fears" } },
        {"walkEnemyFound", new List<string>{"I need to avoid them" } },
        {"shootEnemyFound", new List<string>{"I need to avoid its bullets" } },
        {"campFound", new List<string>{"Well, my good memories", "they help me not to go crazy", "\"Remember suicide is not a way out\"" } },
        {"gotHit", new List<string>{"Ouch, that hurts" } },
        {"lightShrinks", new List<string>{"M-m, without good memories is so uncomfortable", "I see less now" } },
    };
    private Dictionary<string, bool> triggerUsed = new Dictionary<string, bool>();

    [SerializeField] private UnityEngine.Rendering.Universal.Light2D pointLight;
    private bool isCurrentlyTriggered = false;

    public void Trigger (string triggerTag, TipsManager tipObj = null) {
        if (tipObj == null)
            tipObj = centerTip;
        if (!triggers.ContainsKey (triggerTag)) {
            Debug.LogError ("tag not found");
            return;
        }
        if (isCurrentlyTriggered)
            return;
        if (triggerUsed.ContainsKey(triggerTag) && triggerUsed[triggerTag])
            return;
        Debug.Log (triggerTag);
        triggerUsed[triggerTag] = true;
        isCurrentlyTriggered = true;
        tipObj.ShowTip (triggers[triggerTag], TextEndedCallback);
    }

    private void TextEndedCallback () {
        isCurrentlyTriggered = false;
    }

    private void Awake () {
        Trigger ("spawned");
    }
    private void Update () {
        SearchAndTrigger ("star", "starFound");
        SearchEnemies ();
    }
    private void SearchEnemies () {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag ("enemy");
        foreach (GameObject enemy in enemies) {
            if (Vector2.Distance (enemy.transform.position, transform.position) <= pointLight.pointLightOuterRadius / 2) {
                if (enemy.GetComponent<targetingEnemy>() != null) {
                    Trigger ("shootEnemyFound");
                } else {
                    Trigger ("walkEnemyFound");
                }
            }
        }
    }
    private void SearchAndTrigger (string searchTag, string triggerTag) {
        GameObject[] stars = GameObject.FindGameObjectsWithTag (searchTag);
        foreach (GameObject star in stars) {
            if (Vector2.Distance (star.transform.position, transform.position) <= pointLight.pointLightOuterRadius - 0.5f) {
                Trigger (triggerTag);
            }
        }
    }
}
