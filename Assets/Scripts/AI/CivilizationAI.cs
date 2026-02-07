using System;
using System.Collections;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class CivilizationAI : MonoBehaviour
{
    [Header("API")]
    public string apiUrl = "http://localhost:11434/api/generate";
    public string modelName = "qwen2.5:3b";

    [Header("All NPCs")]
    public NPCBrain[] npcs;

    [Header("UI")]
    public TMP_Text aiText;

    private string lastEventResponded = "none";
    private bool isThinking = false;

    void Start()
    {
        npcs = FindObjectsByType<NPCBrain>(FindObjectsSortMode.None);
        aiText.text = "Mayor AI standing by...";
    }

    void Update()
    {
        string currentEvent = EventManager.currentEvent;

        if (currentEvent != lastEventResponded && currentEvent != "normal" && !isThinking)
        {
            lastEventResponded = currentEvent;
            AskAIForResponse(currentEvent);
        }

        // Manual controls
        if (Input.GetKeyDown(KeyCode.Alpha1))
            GiveOrder("work", "Manual work order", 10f);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            GiveOrder("rest", "Manual rest order", 10f);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            GiveOrder("social", "Manual festival", 10f);

        if (Input.GetKeyDown(KeyCode.Alpha4))
            GiveOrder("eat", "Manual feeding", 10f);
    }

    void AskAIForResponse(string eventType)
    {
        Debug.Log($" AI MAYOR ANALYZING EVENT: {eventType}");

        float avgHunger = 0, avgEnergy = 0, avgHappy = 0, avgMoney = 0, avgHealth = 0;
        int alive = 0;

        foreach (var n in npcs)
        {
            if (n != null && !n.isDead)
            {
                avgHunger += n.hunger;
                avgEnergy += n.energy;
                avgHappy += n.happiness;
                avgMoney += n.money;
                avgHealth += n.health;
                alive++;
            }
        }

        if (alive == 0)
        {
            aiText.text = "Everyone died!";
            return;
        }

        avgHunger /= alive;
        avgEnergy /= alive;
        avgHappy /= alive;
        avgMoney /= alive;
        avgHealth /= alive;

        string eventContext = GetEventContext(eventType, avgHunger, avgEnergy, avgHappy, avgMoney);

        string prompt =
            $"CRISIS ALERT: {eventType}!\n\n" +

            $"POPULATION STATUS:\n" +
            $"Citizens alive: {alive}/{npcs.Length}\n" +
            $"Average hunger: {avgHunger:F0}/100\n" +
            $"Average energy: {avgEnergy:F0}/100\n" +
            $"Average happiness: {avgHappy:F0}/100\n" +
            $"Average money: {avgMoney:F0}\n" +
            $"Average health: {avgHealth:F0}/100\n\n" +

            $"EVENT IMPACT:\n{eventContext}\n\n" +

            "YOUR MAYORAL ACTIONS:\n" +
            "eat - Emergency food distribution (costs city money, saves starving citizens)\n" +
            "rest - Mandatory rest decree (restores energy, stops work)\n" +
            "work - Economic mobilization (earns money but exhausts people)\n" +
            "social - City festival (boosts morale and happiness)\n\n" +

            "INSTRUCTIONS:\n" +
            "1. React directly to the current crisis\n" +
            "2. Choose the action that addresses THIS specific event\n" +
            "3. Your message should reference the event\n\n" +

            "Respond with ONLY this JSON format:\n" +
            "{\"action\":\"eat\",\"message\":\"Famine relief - distributing emergency food!\"}\n\n" +

            "Mayor's decision:";

        StartCoroutine(SendToLLM(prompt));
    }

    string GetEventContext(string eventType, float hunger, float energy, float happy, float money)
    {
        switch (eventType)
        {
            case "Famine":
                return $"Food shortage crisis! Hunger spiked by +30 points.\n" +
                       $"Current hunger level: {hunger:F0}/100 (CRITICAL if >80)\n" +
                       $"Citizens are starving. They need food immediately!";

            case "Sleep Crisis":
                return $"Mass exhaustion event! Energy dropped by -35 points.\n" +
                       $"Current energy level: {energy:F0}/100 (CRITICAL if <20)\n" +
                       $"People are collapsing from fatigue. Rest is needed!";

            case "Depression Wave":
                return $"Mental health crisis! Happiness dropped by -35 points.\n" +
                       $"Current happiness: {happy:F0}/100 (CRITICAL if <30)\n" +
                       $"Citizens are depressed. Morale needs boosting!";

            case "Economic Crash":
                return $"Financial disaster! Everyone lost 30% of their money.\n" +
                       $"Current average money: {money:F0}\n" +
                       $"Economy needs rebuilding, but people are struggling!";

            case "Economic Boom":
                return $"Prosperity event! Everyone gained +30 money.\n" +
                       $"Current average money: {money:F0}\n" +
                       $"Good time to celebrate or invest in happiness!";

            default:
                return "Normal conditions. Monitor and maintain stability.";
        }
    }

    IEnumerator SendToLLM(string prompt)
    {
        isThinking = true;
        aiText.text = " Mayor thinking...";

        string cleanPrompt = prompt.Replace("\n", " ").Replace("\"", "'");

        string json =
            "{ \"model\": \"" + modelName + "\", " +
            "\"prompt\": \"" + cleanPrompt + "\", " +
            "\"stream\": false }";

        byte[] bodyRaw = Encoding.UTF8.GetBytes(json);

        using (UnityWebRequest req = new UnityWebRequest(apiUrl, "POST"))
        {
            req.uploadHandler = new UploadHandlerRaw(bodyRaw);
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json");

            yield return req.SendWebRequest();

            if (req.result == UnityWebRequest.Result.Success)
            {
                string raw = req.downloadHandler.text;
                Debug.Log(" AI Response: " + raw);

                OllamaResponse ollama = JsonUtility.FromJson<OllamaResponse>(raw);

                if (ollama != null && !string.IsNullOrEmpty(ollama.response))
                {
                    ParseAndExecute(ollama.response);
                }
                else
                {
                    Debug.LogError("Empty AI response");
                    FallbackResponse();
                }
            }
            else
            {
                Debug.LogError("AI API Error: " + req.error);
                FallbackResponse();
            }
        }

        isThinking = false;
    }

    void ParseAndExecute(string response)
    {
        try
        {
            string cleaned = response.Replace("'", "\"").Trim();

            int start = cleaned.IndexOf("{");
            int end = cleaned.LastIndexOf("}");

            if (start >= 0 && end > start)
            {
                string jsonOnly = cleaned.Substring(start, end - start + 1);
                Debug.Log(" Extracted JSON: " + jsonOnly);

                AIResponse parsed = JsonUtility.FromJson<AIResponse>(jsonOnly);

                if (parsed != null && !string.IsNullOrEmpty(parsed.action))
                {
                    string action = parsed.action.ToLower().Trim();
                    string message = parsed.message;

                    Debug.Log($" AI Decision: {action} - {message}");

                    if (action.Contains("eat") || action.Contains("food"))
                        GiveOrder("eat", message, 15f);
                    else if (action.Contains("rest") || action.Contains("sleep"))
                        GiveOrder("rest", message, 15f);
                    else if (action.Contains("work") || action.Contains("produc"))
                        GiveOrder("work", message, 12f);
                    else if (action.Contains("social") || action.Contains("festival") || action.Contains("happy"))
                        GiveOrder("social", message, 20f);
                    else
                        FallbackResponse();
                }
                else
                {
                    Debug.LogError("Failed to parse AI action");
                    FallbackResponse();
                }
            }
            else
            {
                Debug.LogError("No JSON found in response");
                FallbackResponse();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Parse error: " + e.Message);
            FallbackResponse();
        }
    }

    void FallbackResponse()
    {
        string currentEvent = EventManager.currentEvent;
        Debug.Log(" Using fallback response for: " + currentEvent);

        switch (currentEvent)
        {
            case "Famine":
                GiveOrder("eat", "Emergency food distribution", 15f);
                break;
            case "Sleep Crisis":
                GiveOrder("rest", "Mandatory rest period", 15f);
                break;
            case "Depression Wave":
                GiveOrder("social", "City festival!", 20f);
                break;
            case "Economic Crash":
                GiveOrder("work", "Rebuild economy!", 12f);
                break;
            case "Economic Boom":
                GiveOrder("social", "Celebrate!", 15f);
                break;
            default:
                aiText.text = "Mayor monitoring...";
                break;
        }
    }

    void GiveOrder(string action, string message, float duration)
    {
        aiText.text = " MAYOR: " + message;
        Debug.Log($" MAYOR ORDER: {message} → {action} for {duration}s");

        int count = 0;
        foreach (var n in npcs)
        {
            if (n != null && !n.isDead)
            {
                n.ReceiveEmergencyOrder(action, duration);
                count++;
            }
        }

        Debug.Log($"   → {count} citizens affected");
    }
}

[Serializable]
public class AIResponse
{
    public string action;
    public string message;
}

[Serializable]
public class OllamaResponse
{
    public string response;
}