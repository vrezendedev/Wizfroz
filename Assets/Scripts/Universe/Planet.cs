using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Random = UnityEngine.Random;

namespace Universe
{
    public class Planet : MonoBehaviour
    {
        [Header("Planet Behaviour Characteristics:")]
        public bool ShouldRandomizeData;
        public int RotateDirection;
        public float RotateVelocity;
        public float PlanetSize;
        private float MinSize = 0.1f;
        public int ScaleTime;
        public int NumberOfObstacles = 0;
        [SerializeField] private PlanetType type;
        public Obstacles ObstaclesPrefab;
        public GameObject FlagPrefab;
        public bool IsSlowed = false;
        public bool IsWinPlanet = false;

        [Space(20)]

        [Header("Planet Skins:")]
        public List<Sprite> PlanetSkins;

        private SpriteRenderer spriteRenderer;
        private Frog frog;

        public enum PlanetType
        {
            Water = 0,
            Earth = 1,
            Fire = 2,
            Air = 3
        }

        private void Awake()
        {
            this.TryGetComponent<SpriteRenderer>(out spriteRenderer);
            SetPlanet();
        }

        private void Start()
        {
            InstantiateObstacles();
        }

        private void Update()
        {
            Rotate();

            if (ShouldDestroy())
            {
                Destroy(gameObject);
                if (frog) frog.Die("Shrinked to space abism...");
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            other.gameObject.TryGetComponent<Frog>(out frog);

            if (frog != null)
            {
                if (frog.Won == false)
                {
                    StopAllCoroutines();
                    StartCoroutine(ChangeScaleOverTime(Vector2.zero, ScaleTime));
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            other.gameObject.TryGetComponent<Frog>(out frog);

            if (frog != null)
            {
                if (!ShouldDestroy()) frog = null;
                StopAllCoroutines();
                StartCoroutine(ChangeScaleOverTime(Vector2.one * PlanetSize, ScaleTime));
            }
        }

        private void SetPlanet()
        {
            if (ShouldRandomizeData == true)
            {
                int _type = Random.Range(0, PlanetSkins.Count);
                type = (PlanetType)_type;
                PlanetSize = Random.Range(1f, 3.5f);
                RotateDirection = (int)Mathf.Sign(Random.Range(-1.0f, 1.0f));
                RotateVelocity = Random.Range(90f, 180f);
                ScaleTime = Random.Range(3, 11);
                NumberOfObstacles = Random.Range(2, 4);
            }

            spriteRenderer.sprite = PlanetSkins[(int)type];
            transform.localScale = Vector3.one * PlanetSize;
        }

        private void Rotate()
        {
            float angle = Time.deltaTime * RotateVelocity * RotateDirection;
            transform.RotateAround(transform.position, transform.forward, angle);
        }

        private bool ShouldDestroy()
        {
            return transform.localScale.x <= MinSize && transform.localScale.y <= MinSize;
        }

        private IEnumerator ChangeScaleOverTime(Vector3 scale, float time)
        {
            float progress = 0;
            float rate = 1 / time;

            Vector3 fromScale = transform.localScale;
            Vector3 toScale = scale;

            while (progress < 1)
            {
                progress += Time.deltaTime * rate;
                transform.localScale = Vector3.Lerp(fromScale, toScale, progress);
                yield return null;
            }
        }

        void InstantiateObstacles()
        {
            Vector2 center = this.gameObject.transform.position;

            if (this.IsWinPlanet == false)
            {
                for (int i = 0; i < NumberOfObstacles; i++)
                {
                    var newObstacle = Instantiate(ObstaclesPrefab);
                    float angle = i * Mathf.PI * 2f / NumberOfObstacles;
                    float x = Mathf.Cos(angle) * PlanetSize * 0.7f;
                    float y = Mathf.Sin(angle) * PlanetSize * 0.7f;

                    Vector2 point = new Vector2(x, y) + center;

                    newObstacle.transform.position = new Vector3(point.x, point.y, 1);
                    newObstacle.transform.parent = this.gameObject.transform;

                    float rotationAngle = angle * Mathf.Rad2Deg - 90f;
                    newObstacle.transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);

                    newObstacle.SetSprite(type);
                }
            }
            else
            {
                var flag = Instantiate(FlagPrefab);

                float angle = Mathf.PI * 2f;
                float x = Mathf.Cos(angle) * PlanetSize * 0.9f;
                float y = Mathf.Sin(angle) * PlanetSize * 0.9f;

                Vector2 point = new Vector2(x, y) + center;

                flag.transform.position = new Vector3(point.x, point.y, 1);
                flag.transform.parent = this.gameObject.transform;

                float rotationAngle = angle * Mathf.Rad2Deg - 90f;
                flag.transform.rotation = Quaternion.Euler(0f, 0f, rotationAngle);
            }

        }
    }
}