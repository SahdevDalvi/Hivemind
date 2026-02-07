using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class NPCBrain : MonoBehaviour
{
    [Header("Survival")]
    public float health = 100f;
    public bool isDead = false;

    [Header("Stats")]
    public float hunger;
    public float money;
    public float happiness;
    public float energy;

    [Header("Location")]
    public GameObject homeLocation;
    public GameObject workLocation;
    public GameObject socialLocation;
    public GameObject foodLocation;

    private NavMeshAgent agent;
    private string currentAction = "idle";
    private float decisionTimer = 0f;
    private bool isBusy = false;

    // Mayor emergency override only
    private string emergencyOrder = "";
    private float emergencyTimer = 0f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Healthy start
        hunger = Random.Range(25f, 50f);
        energy = Random.Range(50f, 75f);
        happiness = Random.Range(45f, 70f);
        money = Random.Range(30f, 60f);
        health = 100f;

        agent.avoidancePriority = Random.Range(10, 80);
        agent.stoppingDistance = 1.5f;
        agent.radius = 0.4f;
        agent.speed = Random.Range(3.2f, 4.5f);
    }

    Vector3 GetPoint(GameObject zone)
    {
        Vector3 rand = zone.transform.position +
            new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));

        NavMeshHit hit;
        if (NavMesh.SamplePosition(rand, out hit, 6f, NavMesh.AllAreas))
            return hit.position;

        return zone.transform.position;
    }

    void Update()
    {
        if (isDead) return;

        // Clamp stats
        hunger = Mathf.Clamp(hunger, 0, 100);
        energy = Mathf.Clamp(energy, 0, 100);
        happiness = Mathf.Clamp(happiness, 0, 100);

        // Health system - slow drain
        if (hunger > 90f) health -= Time.deltaTime * 0.5f;
        else if (hunger > 80f) health -= Time.deltaTime * 0.2f;

        if (energy < 8f) health -= Time.deltaTime * 0.5f;
        else if (energy < 15f) health -= Time.deltaTime * 0.2f;

        if (happiness < 10f) health -= Time.deltaTime * 0.3f;

        // Slow recovery when healthy
        if (hunger < 40f && energy > 50f && happiness > 40f)
            health += Time.deltaTime * 0.4f;

        health = Mathf.Clamp(health, 0, 100);

        // Visual feedback
        Renderer rend = GetComponent<Renderer>();
        if (health < 30f)
            rend.material.color = Color.Lerp(Color.red, Color.yellow, health / 30f);
        else
            rend.material.color = Color.white;

        if (health <= 0)
        {
            Die();
            return;
        }

        //  EMERGENCY MAYOR OVERRIDE 
        if (emergencyTimer > 0f)
        {
            emergencyTimer -= Time.deltaTime;

            if (currentAction != emergencyOrder)
            {
                isBusy = false;
                StopAllCoroutines();
                ExecuteAction(emergencyOrder);
            }
            return;
        }

        // NORMAL LIFE
        if (isBusy) return;

        decisionTimer -= Time.deltaTime;
        if (decisionTimer <= 0f)
        {
            MakeDecision();
            decisionTimer = Random.Range(4f, 7f);
        }
    }

    void MakeDecision()
    {
        // Critical survival
        if (hunger > 75f && money > 5f)
        {
            ExecuteAction("eat");
            return;
        }

        if (energy < 25f)
        {
            ExecuteAction("rest");
            return;
        }

        // Normal utility scoring
        float eatScore = (hunger > 60f && money > 10f) ? hunger / 100f : 0f;
        float workScore = (energy > 35f && hunger < 65f) ? Mathf.Clamp01((50f - money) / 50f) : 0f;
        float restScore = (energy < 50f) ? (100f - energy) / 100f * 0.8f : 0f;
        float socialScore = (happiness < 50f && energy > 30f) ? (100f - happiness) / 100f * 0.6f : 0f;

        // Pick best
        if (eatScore > workScore && eatScore > restScore && eatScore > socialScore && eatScore > 0.4f)
            ExecuteAction("eat");
        else if (workScore > restScore && workScore > socialScore)
            ExecuteAction("work");
        else if (restScore > socialScore)
            ExecuteAction("rest");
        else if (socialScore > 0.3f)
            ExecuteAction("social");
        else
            ExecuteAction("rest"); // Default
    }

    void ExecuteAction(string action)
    {
        currentAction = action;
        isBusy = true;
        StopAllCoroutines();

        switch (action)
        {
            case "work":
                agent.SetDestination(GetPoint(workLocation));
                StartCoroutine(DoWork());
                break;
            case "rest":
                agent.SetDestination(GetPoint(homeLocation));
                StartCoroutine(DoRest());
                break;
            case "social":
                agent.SetDestination(GetPoint(socialLocation));
                StartCoroutine(DoSocial());
                break;
            case "eat":
                agent.SetDestination(GetPoint(foodLocation));
                StartCoroutine(DoEat());
                break;
        }
    }

    IEnumerator DoWork()
    {
        yield return new WaitForSeconds(Random.Range(1f, 2f));

        for (int i = 0; i < 3; i++)
        {
            if (currentAction != "work" && emergencyOrder != "work") break;

            money += Random.Range(8f, 15f);
            hunger += 10f;
            energy -= 12f;
            happiness -= 4f;

            yield return new WaitForSeconds(3f);
        }

        isBusy = false;
        currentAction = "idle";
    }

    IEnumerator DoRest()
    {
        yield return new WaitForSeconds(Random.Range(1f, 2f));

        for (int i = 0; i < 3; i++)
        {
            if (currentAction != "rest" && emergencyOrder != "rest") break;

            energy += 25f;
            hunger += 6f;

            yield return new WaitForSeconds(3f);
        }

        isBusy = false;
        currentAction = "idle";
    }

    IEnumerator DoSocial()
    {
        yield return new WaitForSeconds(Random.Range(1f, 2f));

        for (int i = 0; i < 3; i++)
        {
            if (currentAction != "social" && emergencyOrder != "social") break;

            happiness += 18f;
            energy -= 6f;
            hunger += 5f;

            yield return new WaitForSeconds(3f);
        }

        isBusy = false;
        currentAction = "idle";
    }

    IEnumerator DoEat()
    {
        yield return new WaitForSeconds(Random.Range(1f, 2f));

        for (int i = 0; i < 2; i++)
        {
            if (currentAction != "eat" && emergencyOrder != "eat") break;

            if (money > 5f)
            {
                hunger -= 30f;
                money -= 8f;
                happiness += 5f;
                health += 3f;
            }

            yield return new WaitForSeconds(2f);
        }

        isBusy = false;
        currentAction = "idle";
    }

    void Die()
    {
        isDead = true;
        StopAllCoroutines();

        if (agent != null)
            agent.enabled = false;

        GetComponent<Renderer>().material.color = Color.black;
    }

    public void ReceiveEmergencyOrder(string order, float duration)
    {
        // Only accept if not in personal crisis
        if (hunger > 85f || energy < 12f || health < 25f)
            return;

        emergencyOrder = order.ToLower();
        emergencyTimer = duration;
    }
}