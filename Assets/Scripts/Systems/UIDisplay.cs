using UnityEngine;
using TMPro;

public class UIDisplay : MonoBehaviour
{
    public TextMeshProUGUI statsText;
    public TextMeshProUGUI currentState;
    private NPCBrain npc;

    void Start()
    {
        npc = GetComponent<NPCBrain>();
    }

    void Update()
    {
        if (npc == null || statsText == null) return;

        statsText.text =            
            $" Hunger : {npc.hunger:F0}\n" +
            $" Energy : {npc.energy:F0}\n" +
            $" Happiness : {npc.happiness:F0}";

    }
}