using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Stories
{
    public class SeeAction : EventAction
    {
        GameObject target;
        Character[] characters;
        string reactionMessage;
        bool hasSeen;
        
        public SeeAction(GameObject[] actors, GameObject target, string reactionMessage) : base(actors)
        {
            this.target = target;
            this.reactionMessage = reactionMessage;
        }
        public override void OnEnter()
        {
            hasSeen = false;
            characters = new Character[actors.Length];
            for (int i = 0; i < actors.Length; i ++)
            {
                characters[i] = actors[i].GetComponent<Character>();
            }
        }
        public override void OnUpdate()
        {
            hasSeen = true;
            foreach (Character character in characters)
            {
                if (character != null)
                {
                    if (Vector3.SqrMagnitude(character.transform.position - target.transform.position) > character.sqrDetectionRadius)
                    {
                        hasSeen = false;
                    }
                    
                }
            }
            if (hasSeen)
                status = ActionStatus.Exit;
        }
        public override void OnExit()
        {
            foreach (Character character in characters)
            {
                if (character != null)
                {
                    character.AddDiaryEntry(reactionMessage);
                }
            }
        }
    }
}