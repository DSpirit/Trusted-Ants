using UnityEngine;

namespace Assets.Scripts.Ant
{
    // Workaround Unity Error (Particle System is not being stopped correctly and throws AABB and Invalid errors)
    [ExecuteInEditMode]
    public class DisableInactiveParticles : MonoBehaviour
    {
        ParticleSystem.Particle[] unused = new ParticleSystem.Particle[1];

        void Awake()
        {
            GetComponent<ParticleSystemRenderer>().enabled = false;
        }

        void LateUpdate()
        {
            GetComponent<ParticleSystemRenderer>().enabled = GetComponent<ParticleSystem>().GetParticles(unused) > 0;
        }
    }
}
