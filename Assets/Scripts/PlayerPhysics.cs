using UnityEngine;

namespace TwoRuns
{
    public class PlayerPhysics : MonoBehaviour
    {
        [SerializeField] private Rigidbody p1ControlRigidbody;
        [SerializeField] private float rotateSpeed = 0.3f;


        public void MoveLeft()
        {
            p1ControlRigidbody.angularVelocity += new Vector3(0, 0, -rotateSpeed);
        }
        public void MoveRight()
        {
            p1ControlRigidbody.angularVelocity += new Vector3(0, 0, rotateSpeed);
        }

        private void Start()
        {
        }
        private void Update()
        {
        }
        private void OnCollisionEnter(Collision collision)
        {
        }
    }
}
