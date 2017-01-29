using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Manager;
using Assets.Scripts.Norms;
using Assets.Scripts.Player;
using UnityEngine.UI;

public class TaskPanel : MonoBehaviour
{
    public List<Texture> Textures = new List<Texture>();
    public GameManager GameManager;
    public List<Task> ActiveTasks;
    public int TaskHeight = 50;

	// Use this for initialization
	void Start ()
	{
	    GameManager = EnvironmentManager.Instance.GameManager;
	}
	
	// Update is called once per frame
	void Update ()
	{
        CheckTasks();
	    Reorder();

	}

    private void Reorder()
    {
        int i = 1;
        foreach (Task task in ActiveTasks)
        {
            var t = task.GetComponent<Task>();
            if (!t.Finished && t.Activated)
            {
                var pos = new Vector3(0,0);
                pos.y = i * TaskHeight;
                task.transform.localPosition = pos;
                i++;
            }
        }
    }

    public void CheckTasks()
    {
        foreach (var task in ActiveTasks)
        {
            SetTask(task, !task.Finished);
        }
        ActiveTasks = ActiveTasks.Where(n => !n.Finished).ToList();
    }

    public void SetTask(Task task, bool active)
    {
        task.gameObject.SetActive(active);
    }
}
