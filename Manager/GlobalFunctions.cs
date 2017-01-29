using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Assets.Scripts.Ant;
using Assets.Scripts.Ant.AgentTypes;
using Assets.Scripts.Norms;
using Assets.Scripts.Player;
using UnityEngine;
using Action = Assets.Scripts.Norms.Action;
using Random = UnityEngine.Random;

namespace Assets.Scripts.Manager
{
    public class GlobalFunctions : MonoBehaviour
    {
        public static float StandardIncentive = 1;
        public static float StandardSanction = -1;
        public Vector3 RandomPointOnTerrain()
        {
            Terrain terrain = Terrain.activeTerrain;
            int terrainWidth = (int)terrain.terrainData.size.x;
            // terrain size z
            int terrainLength = (int)terrain.terrainData.size.z;
            // terrain x position
            int terrainPosX = (int)terrain.transform.position.x;
            // terrain z position
            int terrainPosZ = (int)terrain.transform.position.z;
            // generate random x position
            int posx = Random.Range(terrainPosX, terrainPosX + terrainWidth);
            // generate random z position
            int posz = Random.Range(terrainPosZ, terrainPosZ + terrainLength);
            // get the terrain height at the random position
            float posy = Terrain.activeTerrain.SampleHeight(new Vector3(posx, 0, posz));
            // create new gameObject on random position
            return new Vector3(posx, posy, posz);
        }

        public static void CheckNorm(Agent submitter, Agent worker, Tag onContract, Action onAction)
        {
            foreach (var norm in EnvironmentManager.Instance.Settings.Norms.Where(n => n.IsActive && n.PertinenceCondition == onContract))
            {
                bool actionApplied = norm.Content == onAction;
                Agent evaluator = norm.Evaluator == Role.Submitter ? submitter : worker;
                Agent target = norm.Target == Role.Submitter ? submitter : worker;
                foreach (var policy in norm.Policies)
                {
                    bool keptPolicy = GlobalFunctions.KeptPolicy(policy, evaluator.Id);
                    if (keptPolicy && actionApplied)
                    {
                        if (keptPolicy && target.AntType == AntType.AdaptiveAnt && submitter.AntType == AntType.CunningAnt)
                        {
                            if (policy.RewardType == RewardType.Sanction)
                            {
                                Debug.Log("");
                            }
                        }
                        EnvironmentManager.Instance.GameManager.AddTask(target, policy);
                    }
                }
            }
        }

        // Random point on the navmesh
        public static Vector3 RandomPointOnNavmesh(float radius)
        {
            Vector3 randomDirection = Random.insideUnitSphere * radius;
            GameObject origin = GameObject.Find("Hive");
            randomDirection += origin.transform.position;
            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, radius, 1);
            return hit.position;
        }

        public static Dictionary<AntType, Color> AntTypeColorMapping = new Dictionary<AntType, Color>()
        {
            { AntType.AltruisticAnt, Color.green },
            { AntType.AdaptiveAnt, Color.blue },
            { AntType.CunningAnt, Color.red },
            { AntType.EgoistAnt, new Color(1, 0, 1) }, // Purple 
            { AntType.FreeriderAnt, new Color(1, 0.6f, 0) }, // Orange
            { AntType.SloppyAnt, Color.yellow }
        };

        public static Dictionary<AntType, FoodType> AntTypeFoodMapping = new Dictionary<AntType, FoodType>()
        {
            { AntType.AltruisticAnt, FoodType.IceCream },
            { AntType.AdaptiveAnt, FoodType.Cake },
            { AntType.CunningAnt, FoodType.Hamburger },
            { AntType.EgoistAnt, FoodType.Donut }, // Purple 
            { AntType.FreeriderAnt, FoodType.Waffle }, // Orange
            { AntType.SloppyAnt, FoodType.Muffin }
        };

        public static string GetTime()
        {
            var t = TimeSpan.FromSeconds(Time.time);
            return string.Format("{00:00}:{01:00}", t.TotalMinutes, t.Seconds);
        }

        public static void DrawLine(Vector3 start, Vector3 end, Color color, float duration = 0.2f)
        {
            GameObject myLine = new GameObject();
            myLine.transform.position = start;
            myLine.AddComponent<LineRenderer>();
            LineRenderer lr = myLine.GetComponent<LineRenderer>();
            lr.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
            lr.SetColors(color, color);
            lr.SetWidth(0.1f, 0.1f);
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
            GameObject.Destroy(myLine, duration);
        }

        public static Vector3 PointOnNavmesh(Vector3 point)
        {
            NavMeshHit hit;
            NavMesh.SamplePosition(point, out hit, 10.0f, 1);
            return hit.position;
        }

        // Group assignment

        public static Groups ConsumerGroup(Groups g)
        {
            switch (g)
            {
                case Groups.Blue:
                    return Groups.Turquoise;
                case Groups.Green:
                    return Groups.Blue;
                case Groups.Purple:
                    return Groups.Green;
                case Groups.Turquoise:
                    return Groups.Purple;
            }
            Debug.Log("Undefined group for consumer");
            return Groups.Blue;
        }

        public static Groups VerificatorGroup(Groups g)
        {
            switch (g)
            {
                case Groups.Blue:
                    return Groups.Green;
                case Groups.Green:
                    return Groups.Purple;
                case Groups.Purple:
                    return Groups.Turquoise;
                case Groups.Turquoise:
                    return Groups.Blue;
            }
            Debug.Log("Undefined group for verfification");
            return Groups.Blue;
        }

        public static Color GroupColorMapping(Groups g)
        {
            switch (g)
            {
                case Groups.Green:
                    return Color.green;
                    
                case Groups.Blue:
                    return Color.blue;
                    
                case Groups.Turquoise:
                    return new Color(0, 0.8f, 0.82f);
                    
                case Groups.Purple:
                    return new Color(0.5f, 0, 0.5f);
                    
                default:
                    return Color.white;
            }
        }

        public static Func<float, float, bool> Smaller = (a, b) => a < b;
        public static Func<float, float, bool> SmallerEqual = (a, b) => a <= b;
        public static Func<float, float, bool> Equal = (a, b) => Math.Abs(a - b) < 0.01;
        public static Func<float, float, bool> BiggerEqual = (a, b) => a >= b;
        public static Func<float, float, bool> Bigger = (a, b) => a > b;

        public static Func<float, float, bool> GetComparisonOperator(Policy policy)
        {
            Func<float, float, bool> f;

            // Define Operator
            switch (policy.ComparisonOperator)
            {
                case Operator.Smaller:
                    f = Smaller;
                    break;
                case Operator.SmallerOrEqual:
                    f = SmallerEqual;
                    break;
                case Operator.Equals:
                    f = Equal;
                    break;
                case Operator.BiggerOrEqual:
                    f = BiggerEqual;
                    break;
                case Operator.Bigger:
                    f = Bigger;
                    break;
                default:
                    f = Smaller;
                    break;
            }
            return f;
        }

        public static bool KeptPolicy(Policy policy, Guid targetId)
        {
            var creditor = EnvironmentManager.Instance.Ants.Find(n => n.Agent.Id == targetId).Agent;
            var f = GetComparisonOperator(policy);
            bool keptPolicy;
            // Check Values
            switch (policy.Option)
            {
                case Option.TrustRating:
                    keptPolicy = f(creditor.TrustRating, policy.Value);
                    break;
                case Option.Consistency:
                    keptPolicy = f(creditor.Consistency, policy.Value);
                    break;
                case Option.Speedup:
                    keptPolicy = f(EnvironmentManager.Instance.Observer.Speedup[creditor.AntType].Average(), policy.Value);
                    break;
                case Option.Workload:
                    keptPolicy = f(creditor.GetValueFromLevel(EnvironmentManager.Instance.Observer.WorkloadTotal()), policy.Value);
                    break;
                default:
                    keptPolicy = f(creditor.TrustRating, policy.Value);
                    break;
            }
            return keptPolicy;
        }

        public static string CreateDirectory(string category)
        {
            string path = Path.Combine(@"C:\Users\wende\Documents", "Statistics");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            string dayPath = Path.Combine(path, DateTime.Now.ToString("M"));
            if (!Directory.Exists(dayPath))
                Directory.CreateDirectory(dayPath);
            path = Path.Combine(dayPath, category);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
    }
}
