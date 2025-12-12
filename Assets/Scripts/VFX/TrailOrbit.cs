using UnityEngine;

namespace ConnectFourMultiplayer.VFX
{
    public class TrailOrbit : MonoBehaviour
    {
        [HideInInspector] public GameObject centerPivot;
        [HideInInspector] public float angle;

        public float orbitSpeed;
        public float orbitRadius;

        void Update()
        {
            angle -= orbitSpeed * Time.deltaTime;

            float x = Mathf.Cos(angle) * orbitRadius;
            float y = Mathf.Sin(angle) * orbitRadius;

            transform.position = centerPivot.transform.position + new Vector3(x, y, 0f);
        }
    }
}