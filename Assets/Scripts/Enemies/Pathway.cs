using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Enemies
{
    public class Pathway : MonoBehaviour, IEnumerator
    {
        public GameObject[] Waypoints;

        private int position = -1;

        public bool MoveNext()
        {
            position++;
            return (position < Waypoints.Length);
        }

        public void Reset()
        {
            position = -1;
        }

        object IEnumerator.Current
        {
            get
            {
                return Current;
            }
        }

        public GameObject Current
        {
            get
            {
                try
                {
                    return Waypoints[position];
                }
                catch (System.IndexOutOfRangeException)
                {
                    return Waypoints[^1];
                }
            }
        }
    }
}

