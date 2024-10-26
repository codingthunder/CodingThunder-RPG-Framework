using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodingThunder.RPGUtilities.Mechanics
{
    public class SimpleCameraFollow : GameStateManaged
    {
        public string targetName;

        private Transform target;

        // Start is called before the first frame update
        protected override void OnStart()
        {
            if (!string.IsNullOrEmpty(targetName))
            {
                target = GameObject.Find(targetName).transform;
            }
        }

        // Update is called once per frame
        protected override void OnUpdate()
        {
            if (string.IsNullOrEmpty(targetName))
            {
                return;
            }

            if (target == null || target.gameObject.name !=  targetName)
            {
                target = GameObject.Find(targetName).transform;
            }

            var targetPosition = target.position;
            targetPosition.z = transform.position.z;

            transform.position = targetPosition;
        }
    }
}
