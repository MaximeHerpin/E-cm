﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

namespace Stories
{

    [System.Serializable]
    public class JsonStory // on dit quelle est la forme d'une histoire en JSon
    {
        public EventEntry[] Events; // elle contient plusieurs évènements, c'est une liste d'évènements

        [System.Serializable]
        public struct EventEntry // on dit quelle est la structure d'un évènement
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
        public static string pathToJsonFolder; // adresse du dossier contenant les fichiers JSon des histoires

        public static Story[] GetAllStories()
        {
            DirectoryInfo dir = new DirectoryInfo(pathToJsonFolder); // on va à l'adresse requise
            FileInfo[] info = dir.GetFiles("*.json"); // liste des n JSon
            Story[] stories = new Story[info.Length]; //tableau prêt à recevoir les n histoires
            int i = 0;
            foreach (FileInfo f in info)
            {
                stories[i] = JsonToStory(f.FullName);
                i++;
            }
            return stories;
        }

        public static Story JsonToStory(string pathToJson) // fonction retournant une classe Story à partir d'un fichier Json
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
        
        private static GameObject[] GetObjectsFromNames(string[] names) // retrouve un GameObject à partir de son nom (écrit en chaîne de caractères)
        {
            GameObject[] result = new GameObject[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                result[i] = GameObject.Find(names[i]);
            }
            return result;
        }

        private static EventAction GetAction(string actionName, GameObject[] actors, string[] parameters) // cas par cas pour les actions - elles ont différents types de paramètres
        {
            EventAction action = null;
            float duration;
            switch (actionName)
            {
                case "Explode":
                    action = new ExplodeAction(actors);
                    break;
                case "Move":
                    GameObject destination = GameObject.Find(parameters[0]);
                    action = new MoveAction(actors, destination);
                    break;
                case "SetTimeSpeed":
                    int timeSpeed = 1;
                    if (!Int32.TryParse(parameters[0], out timeSpeed))
                        Debug.LogError(string.Format("parameter can't be parsed into int for action {0}", actionName));
                    action = new SetTimeSpeedAction(timeSpeed);
                    break;
                case "SlerpTransform":
                    Transform target = GameObject.Find(parameters[0]).transform;
                    duration = float.Parse(parameters[1], CultureInfo.InvariantCulture.NumberFormat);
                    action = new SlerpTransformAction(actors, target, duration);
                    break;
                case "Wait":
                    duration = float.Parse(parameters[0], CultureInfo.InvariantCulture.NumberFormat);
                    action = new WaitAction(duration);
                    break;

                default:
                    Debug.LogError(string.Format("Action {0} does not exist", actionName));
                    break;
            }
            return action;
        }

    }

    
}