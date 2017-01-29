using System.Linq;
using UnityEngine;
using Assets.Scripts.Ant;
using Assets.Scripts.Ant.AgentTypes;
using Assets.Scripts.Jobs;
using Assets.Scripts.Manager;
using Assets.Scripts.Norms;
using Assets.Scripts.SettingClasses;
using Assets.Scripts.UI;
using UnityEngine.SceneManagement;


public enum AntType
{
    AdaptiveAnt, AltruisticAnt, CunningAnt, EgoistAnt, FreeriderAnt, SloppyAnt
}

// Four different groups an ant can belong to
public enum Groups
{
    Green, Blue, Turquoise, Purple
};


public enum ActionType
{
    Distributing, Verifying, Listening
};

public class AntAI : MonoBehaviour
{    
    [SerializeField]
    public Agent Agent;
    public GameObject CurrentJob;
    public Animator Ani;

    // Meetings
    public AntAI Partner;

    // Settings
    private readonly Setting _settings = EnvironmentManager.Instance.Settings;   
    
    public NavMeshAgent Nav;
    public Collider[] HitColliders;
    public Speechbubble StateBubble;
    public ParticleSystem TalkingSystem;
    public ParticleSystem AttackingSystem;
    public float RankThreshold = 1;
    public MoveSinus Sinus;
    public Transform Meeting;
    
    private GlobalFunctions _helper;

    // Use this for initialization
    public void Start()
    {
        _helper = new GlobalFunctions();
        Sinus = GetComponent<MoveSinus>();
        Nav = GetComponent<NavMeshAgent>();
        if (EnvironmentManager.Instance.Settings.NavmeshPriorityCollision)
            Nav.avoidancePriority = 0;
        StateBubble = GetComponentInChildren<Speechbubble>();
        Ani = GetComponent<Animator>();        
        Agent.Jobs = GetComponent<JobSlots>();
        Agent.NavAgent = Nav;
        Agent.Targets = GetComponent<WorkingUnitProcessor>();
        SetColor();        
        InvokeRepeating("Radar", 0, _settings.RefreshRate);
        InvokeRepeating("ScaleToRank", 0, _settings.RefreshRate);
        InvokeRepeating("Cleanup", 60, 180);
        DisableStatebubble();
        
    }

    // Update is called once per frame
    public void Update()
    {
        Ani.SetBool("Meeting", Meeting);
        Ani.SetBool("GotWU", Agent.Jobs.GetPendingJobs().Any());       
        Ani.SetBool("Died", Agent.Rank <= 1);
        Die();
    }

    void DisableStatebubble()
    {
        StateBubble.BubbleImageComponent.enabled = false;
    }
       

    public void Die()
    {
        try
        {
            if (Ani.GetBool("Died"))
            {
                Destroy(Ani);                   
                Agent.Targets = null;
                foreach (var jobSlot in Agent.Jobs.GetComponentsInChildren<Job>())
                {
                    Destroy(jobSlot);
                }
                Destroy(gameObject);
            }
        } catch { }
    }

    public void MoveToRandomPoint()
    {
        if (Nav.remainingDistance > 0.5f) { return; }
        
        Nav.SetDestination(Random.insideUnitSphere *_settings.WalkRadius);
    }

    public void Radar()
    {
        if (Ani.GetCurrentAnimatorStateInfo(0).IsName("Exploring") || Ani.GetCurrentAnimatorStateInfo(0).IsName("Working"))
        {
            HitColliders = Physics.OverlapSphere(transform.position, _settings.SearchRadius);
            Agent.FindMeetingPartner(HitColliders);
        }
    }

    public void Cleanup()
    {
        if (Agent.TrustRatings.Count > 50)
            Agent.TrustRatings = Agent.TrustRatings.Take(50).ToList();
        Agent.Denials.Clear();
    }

    void OnMouseDown()
    {
        if (EnvironmentManager.Instance.Player) return;
        GameObject camera = GameObject.Find("MainCamera");
        SmoothFollow smoothFollow = camera.GetComponent<SmoothFollow>();
        smoothFollow.distance = 50;
        smoothFollow.height = 40;   
        smoothFollow.target = gameObject.transform;
    }

    // Ants scale depends on the rank
    public void ScaleToRank()
    {
        float scale = (((float)Agent.Rank + 1) / ((float)_settings.MaximumRank + 1));
        transform.localScale = Vector3.one * 4 * (scale + 0.5f);
        
        //Agent.NavAgent.avoidancePriority = _settings.MaximumRank-Agent.Rank;
    }

    private void SetColor()
    {        
        var body = transform.FindChild("Body");
        var rend = body.GetComponentInChildren<Renderer>();

        //rend.material.SetColor("_EmissionColor", GlobalFunctions.AntTypeColorMapping[Agent.AntType]);

        string mat = "Blue";
        switch (Agent.AntType)
        {
            case AntType.AdaptiveAnt:
                mat = "Blue";
                break;
            case AntType.AltruisticAnt:
                mat = "Green";
                break;
            case AntType.CunningAnt:
                mat = "Red";
                break;
            case AntType.EgoistAnt:
                mat = "Purple";
                break;
            case AntType.FreeriderAnt:
                mat = "Orange";
                break;
            case AntType.SloppyAnt:
                mat = "Yellow";
                break;
        }
        var shader = Shader.Find("Custom/" + mat);
        rend.material.shader = shader;

        // Set Group
        var groupBubble = transform.FindChild("Group");
        groupBubble.GetComponent<Renderer>().material.color = GlobalFunctions.GroupColorMapping(Agent.Group);

    }

    // Anteater "soaking" ability
    public void OnParticleCollision(GameObject particles)
    {
        if (particles.tag != "Soak") return;
        Shrink();
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Food") return;
        Destroy(collision.gameObject, 1);
        Grow();
    }

    public void Grow()
    {
        // Apply Rank upgrade when hit with food
        if (EnvironmentManager.Instance.GameManager.TaskPanel.ActiveTasks.Any(n => n.Target.Id == Agent.Id))
        {
            var hasIncentive = EnvironmentManager.Instance.GameManager.TaskPanel.ActiveTasks.Any(n => n.Target.Id == Agent.Id && n.Policy.RewardType == RewardType.Incentive);
            if (hasIncentive)
            {
                var task = EnvironmentManager.Instance.GameManager.TaskPanel.ActiveTasks.First(n => n.Target.Id == Agent.Id && n.Policy.RewardType == RewardType.Incentive);
                task.Hits++;
                EnvironmentManager.Instance.FlowManager.TotalHits++;
                Agent.IncreaseRank();
            }
        }
    }

    public void Shrink()
    {
        if (EnvironmentManager.Instance.GameManager.TaskPanel.ActiveTasks.Any(n => n.Target.Id == Agent.Id))
        {
            bool hasSanction = EnvironmentManager.Instance.GameManager.TaskPanel.ActiveTasks.Any(n => n.Target.Id == Agent.Id && n.Policy.RewardType == RewardType.Sanction);
            if (hasSanction)
            {
                EnvironmentManager.Instance.FlowManager.SoakingParticlesHit++;
                var task = EnvironmentManager.Instance.GameManager.TaskPanel.ActiveTasks.First(n => n.Target.Id == Agent.Id && n.Policy.RewardType == RewardType.Sanction);
                task.ParticleHits += EnvironmentManager.Instance.FlowManager.Flow.SoakingEffectivity;
                if (task.ParticleHits > 0.99f)
                {
                    Agent.DecreaseRank();
                    task.ParticleHits = 0;
                    task.Hits++;
                }
            }
        }
    }
}
