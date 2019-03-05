using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Stories
{

    [System.Serializable]
    public class JsonStory
    {
        public EventEntry[] Events;

        [System.Serializable]
        public struct EventEntry
        {
            public string[] Actors;
            public string Action;
            public string[] Parameters;
            public int[] TimeMin;
            public int[] TimeMax;
            public string Conditions;
            public string[] Consequences;
        }
    }


    public static class JsonConverter
    {
        public static string pathToJsonFolder;

        public static Story[] GetAllStories()
        {
            DirectoryInfo dir = new DirectoryInfo(pathToJsonFolder);
            FileInfo[] info = dir.GetFiles("*.json");
            Story[] stories = new Story[info.Length];
            int i = 0;
            foreach (FileInfo f in info)
            {
                stories[i] = JsonToStory(f.FullName);
                i++;
            }
            return stories;
        }

        public static Story JsonToStory(string pathToJson)
        {
            string dataAsJson = File.ReadAllText(pathToJson);
            JsonStory Jstory = JsonUtility.FromJson<JsonStory>(dataAsJson);

            Story story = new Story(Jstory.Events.Length);
            for (int i=0; i<Jstory.Events.Length; i++)
            {
                JsonStory.EventEntry Jevent = Jstory.Events[i];
                TimeOfDay timeMin = new TimeOfDay(Jevent.TimeMin[0], Jevent.TimeMin[1]);
                TimeOfDay timeMax = new TimeOfDay(Jevent.TimeMax[0], Jevent.TimeMax[1]);
                string conditions = Jevent.Conditions;
                string[] consequences = Jevent.Consequences;
                GameObject[] actors = GetObjectsFromNames(Jevent.Actors);
                EventAction action = GetAction(Jevent.Action, actors, Jevent.Parameters);

                StoryEvent storyEvent = new StoryEvent(timeMin, timeMax, action, conditions, consequences);
                story.events[i] = storyEvent;
            }

            return story;
        }
        
        private static GameObject[] GetObjectsFromNames(string[] names)
        {
            GameObject[] result = new GameObject[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                result[i] = GameObject.Find(names[i]);
            }
            return result;
        }

        private static EventAction GetAction(string actionName, GameObject[] actors, string[] parameters)
        {
            EventAction action = null;
            switch (actionName)
            {
                case "Explode":
                    action = new Explode(actors);
                    break;
                case "Move":
                    GameObject destination = GameObject.Find(parameters[0]);
                    action = new Move(actors, destination);
                    break;
                default:
                    Debug.LogError(string.Format("Action {0} does not exist", actionName));
                    break;
            }
            return action;
        }

    }

    
}