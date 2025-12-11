using UnityEngine;

namespace ConnectFourMultiplayer.VFX
{
    public class TrailOrbit : MonoBehaviour
    {
        public GameObject centerPivot;
        public float orbitRadius;
        public float angularVelocity;
        public float angle;
        void Update()
        {
            angle -= angularVelocity * Time.deltaTime;

            // keep angle in range
            if (angle < 0f)
                angle += 2f * Mathf.PI;

            float x = Mathf.Cos(angle) * orbitRadius;
            float y = Mathf.Sin(angle) * orbitRadius;

            transform.position = centerPivot.transform.position + new Vector3(x, y, 0f);
        }
    }
}