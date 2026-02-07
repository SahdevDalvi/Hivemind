using UnityEngine;
using TMPro;
using System.Collections;

public class EventManager : MonoBehaviour
{
    private NPCBrain[] npcs;
    private float eventTimer;
    public static string currentEvent = "normal";

    [Header("UI")]
    public TMP_Text eventText;

    [Header("Event Settings")]
    public float minTimeBetweenEvents = 30f;  // ← Faster events!
    public float maxTimeBetweenEvents = 60f;

    void Start()
    {
        npcs = FindObjectsByType<NPCBrain>(FindObjectsSortMode.None);
        eventTimer = Random.Range(5f, 10f);  // First event comes quickly

        Debug.Log("EventManager started - first event in " + eventTimer + " seconds");
    }

    void Update()
    {
        eventTimer -= Time.deltaTime;

        if (eventTimer <= 0f)
        {
            TriggerRandomEvent();
            eventTimer = Random.Range(minTimeBetweenEvents, maxTimeBetweenEvents);

            Debug.Log($" Next event in {eventTimer:F0} seconds");
        }
    }

    void TriggerRandomEvent()
    {
        int r = Random.Range(0, 6);  // ← 6 options now (including normal)

        if (r == 0) Famine();
        else if (r == 1) EconomicCrash();
        else if (r == 2) SadnessWave();
        else if (r == 3) SleepDeprivation();
        else if (r == 4) EconomicBoom();
        else NormalPeriod();  // ← Peace time
    }

    void ShowEvent(string msg)
    {
        StopAllCoroutines();
        StartCoroutine(ShowEventRoutine(msg));
    }

    IEnumerator ShowEventRoutine(string msg)
    {
        eventText.text = "EVENT: " + msg;
        Debug.Log(" EVENT: " + msg);  // ← See events in console
        yield return new WaitForSeconds(6f);
        eventText.text = "";
    }

    void NormalPeriod()
    {
        currentEvent = "normal";
        ShowEvent("Peace - No crisis");
    }

    void Famine()
    {
        currentEvent = "Famine";

        int affected = 0;
        foreach (var n in npcs)
        {
            if (n != null && !n.isDead)
            {
                n.hunger += 30f;  // ← Reduced from 35
                affected++;
            }
        }

        ShowEvent($"Famine - {affected} citizens affected");
    }

    void EconomicCrash()
    {
        currentEvent = "Economic Crash";

        int affected = 0;
        foreach (var n in npcs)
        {
            if (n != null && !n.isDead)
            {
                n.money *= 0.7f;  // ← Less harsh (was 0.5)
                affected++;
            }
        }

        ShowEvent($"Economic Crash - {affected} citizens lost money");
    }

    void SadnessWave()
    {
        currentEvent = "Depression Wave";

        int affected = 0;
        foreach (var n in npcs)
        {
            if (n != null && !n.isDead)
            {
                n.happiness -= 35f;  // ← Reduced from 40
                affected++;
            }
        }

        ShowEvent($"Depression - {affected} citizens sad");
    }

    void SleepDeprivation()
    {
        currentEvent = "Sleep Crisis";

        int affected = 0;
        foreach (var n in npcs)
        {
            if (n != null && !n.isDead)
            {
                n.energy -= 35f;  // ← Reduced from 40
                affected++;
            }
        }

        ShowEvent($"Sleep Crisis - {affected} citizens exhausted");
    }

    void EconomicBoom()
    {
        currentEvent = "Economic Boom";

        int affected = 0;
        foreach (var n in npcs)
        {
            if (n != null && !n.isDead)
            {
                n.money += 30f;  // ← Reduced from 40
                affected++;
            }
        }

        ShowEvent($"Economic Boom - {affected} citizens got richer");
    }
}