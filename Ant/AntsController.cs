using System;
using UnityEngine;
using System.Linq;
using Assets.Scripts.Ant;
using Assets.Scripts.Ant.AgentTypes;
using Assets.Scripts.Jobs;
using Assets.Scripts.Manager;
using Assets.Scripts.SettingClasses;
using Assets.Scripts.UI.Development;
using Random = UnityEngine.Random;


public class  AntsController : MonoBehaviour {
	
	// The prefabs
	public GameObject antPrefab; // = (GameObject) Resources.Load("Prefabs/FireAntPrefab");
	public GameObject bubblePrefab; // = (GameObject) Resources.Load("Prefabs/BubblePrefab");

    // GeneralSettings
    public Setting GeneralSettings;

	// Flag set when Start finishes (Delay of instantiate)
	public bool loaded = false;

	// A flag that determines wether music is playing or not
	public bool musicEnabled = false;
	
	// Stores the inital camera transformation
	public Vector3 cameraPosInit;
	public Quaternion cameraRotInit;

    // Game Objects
    public GameObject GoCamera;


	private float zoomZ = 6.0f;
	private float minY = 20.0f;
	private float maxY = 550.0f;
	

	// Use this for initialization
	void Start ()
    {
        GeneralSettings = EnvironmentManager.Instance.Settings;
        // Spawn Ants for each Ant Type Setting
        foreach (var antTypeSetting in GeneralSettings.AntTypeSettings)
        {
            SpawnAntForAntType(antTypeSetting);
        }


        // Invoke simulation routines
        InvokeRepeating("SpawnJob", 0, GeneralSettings.JobSetting.JobSpawnRate);
        //InvokeRepeating ("RankDowngrade", 0, 20);

        // Save the initial camera tranformation
        cameraPosInit = GameObject.Find("MainCamera").transform.position;
        cameraRotInit = GameObject.Find("MainCamera").transform.rotation;
        GoCamera = GameObject.Find("MainCamera");
        loaded = true;

        // Check the Mode the Player has chosen
	    GameObject.Find("GUI").GetComponentInChildren<ModePanel>(true).CheckApplicationMode();
    }

    // Update is called once per frame
    void Update () 
    {
		CheckInput();
	}

    public void ToggleJobSpawning(bool value)
    {
        if (value)
            InvokeRepeating("SpawnJob", 0, GeneralSettings.JobSetting.JobSpawnRate);
        else 
            CancelInvoke("SpawnJob");
    }

    public void CheckInput()
    {
        SmoothFollow smoothFollow = GoCamera.GetComponent<SmoothFollow>();

        // Finish following an ant by pressing escape
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (smoothFollow.target != null)
            {
                smoothFollow.target.gameObject.GetComponent<AudioSource>().mute = !musicEnabled;
                smoothFollow.target = null;
                GoCamera.transform.position = cameraPosInit;
                GoCamera.transform.rotation = cameraRotInit;
                CanvasGroup zoom = GameObject.Find("ZoomButton").GetComponent<CanvasGroup>();
                zoom.alpha = 0;
            }
        }

        if (smoothFollow.target == null)
        {
            // Mouse wheel moving forwards --> zoom out
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && GoCamera.transform.position.y < maxY)
            {
                float y = Mathf.Lerp(GoCamera.transform.position.y, GoCamera.transform.position.y + zoomZ, Time.time);
                GoCamera.transform.position = new Vector3(GoCamera.transform.position.x, y, GoCamera.transform.position.z);
            }

            // Mouse wheel moving backwards --> zoom in
            if (Input.GetAxis("Mouse ScrollWheel") < 0 && GoCamera.transform.position.y > minY)
            {
                float y = Mathf.Lerp(GoCamera.transform.position.y, GoCamera.transform.position.y - zoomZ, Time.time);
                GoCamera.transform.position = new Vector3(GoCamera.transform.position.x, y, GoCamera.transform.position.z);
            }
        }
        else
        {
            // Mouse wheel moving forwards --> zoom out
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && smoothFollow.height < smoothFollow.maxHeight)
            {
                smoothFollow.height += 5;
            }

            // Mouse wheel moving backwards --> zoom in
            if (Input.GetAxis("Mouse ScrollWheel") < 0 && smoothFollow.height > smoothFollow.minHeight)
            {
                smoothFollow.height -= 5;
            }
        }
    }

	// Every n seconds a job gets distributed randomly among all ants
	public void SpawnJob()
	{
	    var ants = EnvironmentManager.Instance.Ants;
		if (!ants.Any()) return;

        AntAI ant = ants[Random.Range(0, ants.Count)];
		int enqueuedJobs = ant.Agent.Jobs.EnqueuedJobs.transform.childCount;
		
        // Check if Space for Job is available
	    if (enqueuedJobs >= EnvironmentManager.Instance.Settings.Capacity) return;

	    Transform transPendingSlot = ant.Agent.Jobs.EnqueuedJobs.transform;
	    GameObject goJob = CreateNewJob(enqueuedJobs);
	    Job newJob = goJob.GetComponent<Job>();
        newJob.Owner = ant.Agent;
        newJob.GenerateRandomData();
	    goJob.transform.SetParent(transPendingSlot, false);
	}

	public GameObject CreateNewJob(int childCount){
		return (GameObject) Instantiate (bubblePrefab, new Vector3(0.0f, (float)childCount * bubblePrefab.transform.localScale.y, 0.0f), Quaternion.identity);
	}

	// Every n seconds the rank of all ants gets decreased
	public void RankDowngrade(){
		foreach (Agent ant in EnvironmentManager.Instance.Ants.Select(n => n.Agent)) {
			ant.DecreaseRank();
		}		
	}

	public void reloadInvokeRepeatings() {
		CancelInvoke("SpawnJob");
		CancelInvoke("RankDowngrade");		
		InvokeRepeating("SpawnJob", 0, GeneralSettings.JobSetting.JobSpawnRate);
		InvokeRepeating("RankDowngrade", 0, 5);
	}

	public void enableMusic() {
		GameObject.Find("MainCamera").GetComponent<AudioSource>().mute = false;
		musicEnabled = true;
		SmoothFollow smoothFollow = GameObject.Find ("MainCamera").GetComponent<SmoothFollow> ();
		if (smoothFollow.target != null) {
			smoothFollow.target.gameObject.GetComponent<AudioSource>().mute = !musicEnabled;
		}
	}

	public void disableMusic() {
		GameObject.Find("MainCamera").GetComponent<AudioSource>().mute = true;
		musicEnabled = false;
		SmoothFollow smoothFollow = GameObject.Find ("MainCamera").GetComponent<SmoothFollow> ();
		if (smoothFollow.target != null) {
			smoothFollow.target.gameObject.GetComponent<AudioSource>().mute = !musicEnabled;
		}
	}

    public void SpawnAntForAntType(AntTypeSetting setting)
    {
        foreach (Groups group in Enum.GetValues(typeof(Groups)))
        {
            SpawnAnts(group, setting);
        }
    }

    public void SpawnAnts(Groups group, AntTypeSetting setting)
    {
        var factory = new AgentFactory();
        // Get a random spawn point
        Vector3 randomDirection = Random.insideUnitSphere * GeneralSettings.SpawnRadius;
        randomDirection += gameObject.transform.position;
        NavMeshHit hit;
        // Get amount for random spawning
        int randomCount = Random.Range(0, setting.DistributionSetting.RandomCount.Find(n => n.AntGroup == group).Count);

        // Spawn ants
        for (int i = 0; i < setting.DistributionSetting.FixedCount.Find(n => n.AntGroup == group).Count + randomCount; i++)
        {
            // Find an appropriate spawning place
            for (int j = 0; j < 300; j++)
            {
                if (!NavMesh.SamplePosition(randomDirection, out hit, GeneralSettings.SpawnRadius, 1)) continue;
                // Get Agent
                var agent = factory.CreateAgent(group, setting.AntType);
                // Instantiate Object
                var newAnt = (GameObject)Instantiate(antPrefab, hit.position, Random.rotation);
                newAnt.GetComponent<AntAI>().Agent = agent;
                agent.Ant = newAnt.GetComponent<AntAI>();
                newAnt.transform.SetParent(gameObject.transform);
                break;
            }
        }
    }
}