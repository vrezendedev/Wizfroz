using System.Collections;

using UnityEngine;

using Universe;

namespace Spells
{
    public class SlowDownPlanetSpell : Spell
    {
        public float SlowedDuration = 0f;
        private float PlanetInitialRotationSpeed = 0f;
        private int PlanetInitialScaleTime = 0;
        private float countdown;

        public SlowDownPlanetSpell() : base(SpellTypes.SlowDownPlanetSpell) { }

        public override void Action(GameObject target)
        {
            target.TryGetComponent<Planet>(out var planet);

            PlanetInitialRotationSpeed = planet.RotateVelocity;
            PlanetInitialScaleTime = planet.ScaleTime;

            planet.RotateVelocity = PlanetInitialRotationSpeed - (0.5f * PlanetInitialRotationSpeed);
            planet.ScaleTime = PlanetInitialScaleTime + Mathf.CeilToInt(0.5f * (float)PlanetInitialScaleTime);
            planet.IsSlowed = true;

            StartCoroutine(CountdownSlowedDown(planet));
        }

        IEnumerator CountdownSlowedDown(Planet planet)
        {
            while (countdown < SlowedDuration)
            {
                countdown++;
                yield return new WaitForSeconds(1f);
            }

            planet.RotateVelocity = PlanetInitialRotationSpeed;
            planet.ScaleTime = PlanetInitialScaleTime;
            planet.IsSlowed = false;
        }
    }
}

