using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
            JsonStory Jstory = null;
            string dataAsJson = File.ReadAllText(pathToJson);
            try
            {
                Jstory = JsonUtility.FromJson<JsonStory>(dataAsJson);
            }
            catch (ArgumentException)
            {
                Debug.LogError(string.Format("Invalid Json file :{0}", Path.GetFileName(pathToJson)));
                return null;
            }

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
                if (result[i] == null)
                    Debug.LogError(string.Format("GameObject {0} was not found, check spelling", names[i]));
            }
            return result;
        }

        private static EventAction GetAction(string actionName, GameObject[] actors, string[] parameters)
        {
            EventAction action = null;
            float duration;
            GameObject target;
            switch (actionName)
            {
                case "Explode":
                    action = new ExplodeAction(actors);
                    break;
                case "Move":
                    GameObject destination = GameObject.Find(parameters[0]);
                    if (destination == null)
                        Debug.LogError(string.Format("can't move to location, {0} is not Found", parameters[0]));
                    action = new MoveAction(actors, destination);
                    break;
                case "Interact":
                    target = GameObject.Find(parameters[0]);
                    if (target == null)
                        Debug.LogError(string.Format("can't interact with object, {0} is not Found", parameters[0]));
                    action = new InteractAction(actors, target);
                    break;
                case "SetTimeSpeed":
                    int timeSpeed = 1;
                    if (!Int32.TryParse(parameters[0], out timeSpeed))
                        Debug.LogError(string.Format("parameter can't be parsed into int for action {0}", actionName));
                    action = new SetTimeSpeedAction(timeSpeed);
                    break;
                case "SlerpTransform":
                    Transform targetTransform = GameObject.Find(parameters[0]).transform;
                    duration = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
                    action = new SlerpTransformAction(actors, targetTransform, duration);
                    break;
                case "Wait":
                    duration = float.Parse(parameters[0], CultureInfo.InvariantCulture.NumberFormat);
                    action = new WaitAction(duration);
                    break;
                case "LogEntry":
                    string description = parameters[0];
                    action = new DiaryEntryAction(actors, description);
                    break;
                case "Speak":
                    action = new SpeakAction(actors);
                    break;
                case "See":
                    target = GameObject.Find(parameters[0]);
                    string reactionMessage = parameters[1];
                    action = new SeeAction(actors, target, reactionMessage);
                    break;
                case "DefaultBehaviour":
                    action = new DefaultBehaviourAction(actors);
                    break;
                case "Die":
                    action = new DieAction(actors);
                    break;
                default:
                    Debug.LogError(string.Format("Action {0} does not exist", actionName));
                    break;
            }
            return action;
        }

    }
}

public static class JsonNamesConverter
{
    public static JsonNames ConvertNames(string path)
    {
        JsonNames names = null;
        string dataAsJson = File.ReadAllText(path);
        try
        {
            names = JsonUtility.FromJson<JsonNames>(dataAsJson);
        }
        catch (ArgumentException)
        {
            Debug.LogError(string.Format("Invalid Json file :{0}", Path.GetFileName(path)));
        }
        return names;
    }
}

public class JsonNames
{
    public string[] males;
    public string[] females;
    private int malesIndex = -1;
    private int femalesIndex = -1;

    public string GetNextMale()
    {
        malesIndex++;
        return males[malesIndex];
    }
    public string GetNextFemale()
    {
        femalesIndex++;
        return females[femalesIndex];
    }
}