using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stories
{
    public class SlerpTransformAction : EventAction
    {
        private Transform target;
        private Transform actor;
        private Vector3 initialPosition;
        private Quaternion initialRotation;
        private Vector3 targetPosition;
        private Quaternion targetRotation;
        private float duration;
        private float factor;

        public SlerpTransformAction(GameObject[] actors, Transform target, float duration) : base(actors)
        {
            this.target = target;
            this.duration = duration;
            actor = actors[0].transform;
        }

        public override void OnEnter() // vérifier que les acteurs existent
        {
            targetPosition = target.position;
            targetRotation = target.rotation;
            initialPosition = actor.position;
            initialRotation = actor.rotation;
            factor = 0;
        }
        public override void OnUpdate()
        {
            float t = Mathf.SmoothStep(0, 1, factor);
            if (t >= 1)
                status = ActionStatus.Exit;
            actor.position = Vector3.Lerp(initialPosition, targetPosition, t);
            actor.rotation = Quaternion.Lerp(initialRotation, targetRotation, t);
            factor += Time.unscaledDeltaTime / duration;
            
        }
        public override void OnExit()
        {
           
        }
    }
}