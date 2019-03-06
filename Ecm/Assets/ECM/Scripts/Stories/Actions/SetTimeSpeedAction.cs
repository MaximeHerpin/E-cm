using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stories
{
    public class SetTimeSpeedAction : EventAction
    {
        private float timeScale;
        public SetTimeSpeedAction(float timeScale)
        {
            this.timeScale = timeScale;
        }

        public override void OnEnter()
        {
            TimeManager.instance.SetTimeScale(timeScale);
        }
        public override void OnUpdate()
        {
            status = ActionStatus.Exit;
        }
        public override void OnExit()
        {
            
        }
    }
}
