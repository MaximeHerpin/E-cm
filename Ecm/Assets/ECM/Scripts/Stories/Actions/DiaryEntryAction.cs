using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stories
{
    public class DiaryEntryAction : EventAction
    {
        private string description;

        public DiaryEntryAction(GameObject[] actors, string description) : base(actors)
        {
            this.description = description;
        }

        public override void OnEnter()
        {
            foreach (GameObject actor in actors)
            {
                Character character = actor.GetComponent<Character>();
                if (character != null)
                {
                    character.AddDiaryEntry(description);
                }
                else
                {
                    Debug.LogError(string.Format("{0} was asked to write a diary entry but has no Character Component attached", actor.name));
                }
            }            
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