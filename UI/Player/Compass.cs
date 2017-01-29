using UnityEngine;
using System.Collections;
using System.Linq;
using Assets.Scripts.Manager;
using Assets.Scripts.Player;

public class Compass : MonoBehaviour
{
    public GameManager TaskManager;
    public float RotationX = 90;
    public float RotationY = 0;
    public float RotationZ = 90;
    public Transform Target;
    public float Distance = 0;

    // Use this for initialization
    void Start()
    {
        transform.rotation.Set(RotationX, 0,0,0);
    }

    // Update is called once per frame
    void Update()
    {
        bool isActive = false;
        if (TaskManager.TaskPanel.ActiveTasks.Any())
        {
            Target = TaskManager.TaskPanel.ActiveTasks.First().Target.Ant.transform;
            Distance = Vector3.Distance(Target.transform.position, transform.position);
            var lookPos = Target.position - transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime);
            isActive = true;
        }

        // Trigger Enemies
        foreach (var task in TaskManager.TaskPanel.ActiveTasks.Where(n => !n.Finished))
        {
            Target = task.Target.Ant.transform;
            Distance = Vector3.Distance(Target.transform.position, transform.position);
            bool crossedTarget = Distance < 100 && Target;
            SetArrowActive(crossedTarget);
            if (crossedTarget)
            {
                isActive = false;
                task.Target.Ant.Ani.SetTrigger("GotAttacked");
            }
        }

        //foreach (var finishedTask in EnvironmentManager.Instance.Player.Tasks.GetComponentsInChildren<Task>(true))
        //{
        //    Target = finishedTask.Target.Ant.transform;
        //    SetArrowActive(false);
        //}

        transform.parent.GetComponent<Camera>().enabled = isActive;
    }

    public void SetArrowActive(bool setActive)
    {
        var pointer = Target.GetComponentInChildren<Pointer>(true);
        pointer.gameObject.SetActive(setActive);
        pointer.Player = gameObject;
    }
}
