using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Stories
{
    public class SpeakAction : EventAction {

        public SpeakAction(GameObject[] actors) : base(actors)
        {
        }
        public override void OnEnter() // vérifier que les acteurs existent
        {
            base.OnEnter();
            string[] interlocutors = new string[actors.Length - 1];
            for (int i=0; i<actors.Length; i++)
            {
                Character character = actors[i].GetComponent<Character>();
                if (character != null)
                {
                    int index = 0;
                    for (int j=0; j<actors.Length; j++)
                    {
                        if (j != i)
                        {
                            interlocutors[index] = actors[j].name;
                            index++;
                        }
                    }
                    string message = string.Format("Spoke with {0}", string.Join(",", interlocutors));
                    character.AddDiaryEntry(message);
                }
            }
        }
        public override void OnUpdate()
        {
            status = ActionStatus.Exit;
        }
    }

    public class Dialogue
    {

    }
}