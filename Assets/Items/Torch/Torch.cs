using System.Collections;
using UnityEngine;

namespace Items
{
    public class Torch : MonoBehaviour
    {
        public Light torchLight;
        public float burnDuration = 60f;
        public float lightIntensity = 1f;
        private bool isLit = false;
        private float burnTimer = 0f;

        void Update()
        {
            if (isLit)
            {
                burnTimer += Time.deltaTime;
                if (burnTimer >= burnDuration)
                {
                    Extinguish();
                }
            }
        }

        public void Use()
        {
            if (isLit)
            {
                Extinguish();
            }
            else
            {
                Ignite();
            }
        }

        private void Ignite()
        {
            isLit = true;
            torchLight.enabled = true;
            torchLight.intensity = lightIntensity;
            burnTimer = 0f;
        }

        private void Extinguish()
        {
            isLit = false;
            torchLight.enabled = false;
        }
    }
}

