using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class EnemySpaceship : MonoBehaviour
    {
        public Pathway pathway;
        public float speed;
        public float rotateSpeed;
        public float sightRadius;
        public GameObject SightCircle;

        private Vector3 currentTargetWaypoint;
        private CircleCollider2D circleCollider;

        private void Awake()
        {
            TryGetComponent<CircleCollider2D>(out circleCollider);
            circleCollider.radius = sightRadius;
            SightCircle.transform.localScale = Vector3.one * sightRadius * 2;
        }

        private void Start()
        {
            pathway.MoveNext();
            currentTargetWaypoint = pathway.Current.transform.position;
        }

        private void Update()
        {
            FollowPath();
        }

        private void FollowPath()
        {
            if (transform.position == currentTargetWaypoint)
            {
                if (!pathway.MoveNext()) pathway.Reset();
                currentTargetWaypoint = pathway.Current.transform.position;
            }

            transform.position = Vector3.MoveTowards(transform.position, currentTargetWaypoint, speed * Time.deltaTime);

            Vector3 direction = (currentTargetWaypoint - transform.position);
            direction.z = 0f;
            Quaternion targetRotation = Quaternion.FromToRotation(Vector3.up, direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotateSpeed);
        }

    }
}

