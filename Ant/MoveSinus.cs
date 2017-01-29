using UnityEngine;

namespace Assets.Scripts.Ant
{
    public class MoveSinus : MonoBehaviour
    {
        private NavMeshAgent _nav;
        private Animator _ani;
        void Start()
        {
            _nav = GetComponent<NavMeshAgent>();
            _ani = GetComponent<Animator>();
        }

        void Update()
        {
            if (_ani.GetCurrentAnimatorStateInfo(0).IsName("Exploring"))
            {
                // Update degrees
                float degreesPerSecond = 360.0f / m_period;
                m_degrees = Mathf.Repeat(m_degrees + (Time.deltaTime * degreesPerSecond), 360.0f);
                float radians = m_degrees * Mathf.Deg2Rad;

                // Offset by sin wave
                Vector3 offset = new Vector3(0, 0, m_amplitude * Mathf.Sin(radians));
                _nav.Move(offset);
            }
            
        }

        float m_degrees;

        [SerializeField]
        float m_speed = 5;

        [SerializeField]
        float m_amplitude = .5f;

        [SerializeField]
        float m_period = .5f;
    }

}
