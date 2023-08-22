using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Universe
{
    public class Obstacles : MonoBehaviour
    {
        [Header("Obstacles Skins by Planet Type:")]
        public List<Sprite> PlanetWaterObstacles;
        public List<Sprite> PlanetEarthObstacles;
        public List<Sprite> PlanetFireObstacles;
        public List<Sprite> PlanetAirObstacles;

        [Space(20)]
        private SpriteRenderer spriteRenderer;

        void Awake()
        {
            this.TryGetComponent<SpriteRenderer>(out spriteRenderer);
        }

        public void SetSprite(Planet.PlanetType type)
        {
            switch (type)
            {
                case Planet.PlanetType.Water:
                    spriteRenderer.sprite = PlanetWaterObstacles[Random.Range(0, PlanetWaterObstacles.Count)];
                    break;
                case Planet.PlanetType.Earth:
                    spriteRenderer.sprite = PlanetEarthObstacles[Random.Range(0, PlanetEarthObstacles.Count)];
                    break;
                case Planet.PlanetType.Fire:
                    spriteRenderer.sprite = PlanetFireObstacles[Random.Range(0, PlanetFireObstacles.Count)];
                    break;
                case Planet.PlanetType.Air:
                    spriteRenderer.sprite = PlanetAirObstacles[Random.Range(0, PlanetAirObstacles.Count)];
                    break;
            }
        }
    }
}

