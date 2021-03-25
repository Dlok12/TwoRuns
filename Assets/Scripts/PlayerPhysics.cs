using UnityEngine;

namespace TwoRuns
{
    public class PlayerPhysics : MonoBehaviour
    {
        [SerializeField] private Level level;
        [SerializeField] private Rigidbody p1ControlRigidbody;
        [SerializeField] private Rigidbody p2ControlRigidbody;
        [SerializeField] private Transform p1CameraTransform;
        [SerializeField] private Renderer p1ModelRenderer;
        [SerializeField] private float rotateSpeed = 0.3f;

        const string PLAYER_TAG = "Player";

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
        private void Respawn()
        {
            Player.Died = true;
            Player.ResetScores();

            Vector3 p = gameObject.transform.position;

            StartCoroutine(ProcedureAnimations.Shake(p1CameraTransform));
            StartCoroutine(ProcedureAnimations.Stay(transform, afterAnimation: () =>
            {
                Player.Died = false;
            }));
        }
        private void VelocitiesExchange()
        {
            float velP1 = p1ControlRigidbody.angularVelocity.z;
            float velP2 = p2ControlRigidbody.angularVelocity.z;

            p1ControlRigidbody.angularVelocity += new Vector3(0, 0, -velP1 + velP2);
            p2ControlRigidbody.angularVelocity += new Vector3(0, 0, velP1 - velP2);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!Player.Died)
            {
                Debug.Log("trig");
                if (other.transform.CompareTag(PLAYER_TAG))
                {
                    VelocitiesExchange();
                }
                else
                {
                    Respawn();
                }
            }
        }
    }
}
