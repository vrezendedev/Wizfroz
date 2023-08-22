using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class EnemyHedgehogProjectile : MonoBehaviour
    {
        [Header("Enemy Hedgehog Shot Characteristics:")]
        [SerializeField] private float speed = 2f;

        private SpriteRenderer spriteRenderer;

        [SerializeField] private float lifetime = 0.2f;
        private float timePassed = 0f;

        void Awake()
        {
            TryGetComponent<SpriteRenderer>(out spriteRenderer);
        }

        void Start()
        {
            StartCoroutine(Disappear());
        }

        void Update()
        {
            transform.position += transform.up * Time.deltaTime * speed;
        }

        public IEnumerator Disappear()
        {
            while (timePassed < lifetime)
            {
                float alpha = Mathf.Lerp(1f, 0f, (timePassed / lifetime));
                var color = this.spriteRenderer.material.color;
                this.spriteRenderer.material.color = new Color(color.r, color.g, color.b, alpha);
                timePassed += Time.deltaTime;
                yield return new WaitForSeconds(0.01f);
            }
            Destroy(this.gameObject);
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Player")
            {
                StopAllCoroutines();
                Destroy(this.gameObject);
            }
        }
    }
}
