using UnityEngine;

namespace Demo
{
    [RequireComponent(typeof(Rigidbody))]
    public class Player : MonoBehaviour, IGameService
    {
        [SerializeField] private float movementSpeed = 5f;

        [SerializeField] private GameObject canvas;

        private float yaw;
        private float pitch;

        private void Update()
        {
            if (!canvas.activeSelf)
            {
                Vector3 movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0f, Input.GetAxisRaw("Vertical"));
                transform.Translate(movementInput * (movementSpeed * Time.deltaTime));

                yaw += Input.GetAxis("Mouse X") * 5f;
                pitch -= Input.GetAxis("Mouse Y") * 5f;
                transform.eulerAngles = new Vector3(pitch, yaw, 0.0f);
            }
        }
        
        public void Initialize()
        {
            
        }
    }
}
