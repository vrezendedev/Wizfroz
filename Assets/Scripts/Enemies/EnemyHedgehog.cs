using System.Collections;
using UnityEngine;

namespace Enemies
{
    public class EnemyHedgehog : MonoBehaviour
    {
        [Header("Enemy Hedgehog Characteristics:")]
        public GameObject Projectile;
        public GameObject ProjetileSpot;
        private float timeTillNextShot = 0.5f;
        private float currentTime = 0f;

        void Update()
        {
            if (currentTime == 0f)
            {
                StartCoroutine(NextShot());
            }

            float angle = Time.deltaTime * 90f;
            ProjetileSpot.transform.RotateAround(transform.position, ProjetileSpot.transform.forward, angle);
        }

        private IEnumerator NextShot()
        {
            while (currentTime < timeTillNextShot)
            {
                var shot = Instantiate(Projectile, this.ProjetileSpot.transform.position, ProjetileSpot.transform.rotation);

                currentTime += 0.5f;
                yield return new WaitForSeconds(0.5f);
            }

            currentTime = 0f;
        }
    }
}
