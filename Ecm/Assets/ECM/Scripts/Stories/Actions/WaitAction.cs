using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stories
{
    public class WaitAction : EventAction
    {
        private float duration;
        private float timeElapsed;

        public WaitAction(float duration)
        {
            this.duration = duration;
        }

        public override void OnEnter()
        {
            timeElapsed = 0;
        }
        public override void OnUpdate()
        {
            timeElapsed += Time.unscaledDeltaTime;
            if (timeElapsed > duration)
                status = ActionStatus.Exit;

        }
        public override void OnExit()
        {

        }
    }
}