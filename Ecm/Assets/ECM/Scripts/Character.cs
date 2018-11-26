using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour {
    public string realName;
    public Mood mood;

    public float physicalCondition = 1f; // Will influence speed, activities, ect
    public float studious = 1; // How serious in studies
    public float social = 1; // Qualifies interactions

    public float toiletNeeds = 1;
    public float foodNeeds = 1;
    public float cafeineNeeds = 1;

    public static string[] maleNames = { "abel", "achille", "adam", "adrien", "aimé", "al", "alain", "albert", "alby", "alexandre", "alfred", "alger", "ali", "alonso", "alphonse", "amaury", "anatole", "andré", "antoine", "anton", "archibald", "armand", "arnaud", "art", "arthur", "auguste", "augustin", "augustus", "bart", "bas", "beau", "ben", "benjamin", "bernard", "bertrand", "bile", "bill", "bob", "bras", "brian", "brin", "carlo", "carlos", "césar", "charles", "christ", "christophe", "clément", "constant", "daniel", "dante", "david", "denis", "désiré", "don", "duc", "édouard", "émile", "engin", "ernest", "étienne", "eugène", "eugenio", "félix", "ferdinand", "fernand", "franc", "france", "franck", "franco", "françois", "fred", "frédéric", "gabriel", "gaston", "george", "georges", "gérard", "germain", "gilbert", "gilles", "gosse", "guillaume", "gustave", "guy", "hall", "hector", "henri", "henry", "hermann", "hervé", "hilaire", "honoré", "hubert", "hugo", "innocent", "ira", "jack", "jacques", "james", "jarrett", "javier", "jeannot", "jérémie", "jérôme", "john", "joseph", "juan", "julien", "jure", "juste", "karl", "king", "lambert", "lance", "lasse", "laurent", "léon", "les", "lewis", "louis", "loup", "luc", "lucien", "ludo", "major", "manuel", "marc", "marcel", "marco", "marin", "marino", "marquis", "marquise", "mars", "martin", "mat", "mathias", "maurice", "max", "maxwell", "michel", "milan", "modeste", "monte", "mort", "nathan", "newton", "nicolas", "noble", "olivier", "om", "otto", "paris", "pascal", "patrick", "paul", "pedro", "philippe", "pierre", "platon", "porter", "prince", "raoul", "raphaël", "ravi", "raymond", "re", "régis", "renard", "rené", "richard", "robert", "roc", "roger", "roland", "romain", "roman", "roosevelt", "royal", "royale", "rudolf", "rufus", "samuel", "sans", "sébastien", "serge", "séverin", "simon", "stepan", "stéphane", "tel", "thierry", "thomas", "timothée", "travers", "tue", "urbain", "valéry", "victor", "ville", "vin", "vincent", "vitale", "washington", "werner", "william", "wilson", "winston", "wolf", "xavier", "york" };
    public static string[] femaleNames = { "adélaïde", "adèle", "agathe", "aimée", "alexandrie", "alice", "ami", "amie", "anita", "anna", "anne", "annie", "antoinette", "atalanta", "aude", "aura", "aurore", "avis", "avril", "béatrice", "belle", "berlin", "berthe", "betty", "blanche", "bride", "brigitte", "caprice", "carmen", "carole", "catherine", "cécile", "céleste", "charlotte", "cher", "christine", "ciel", "claire", "clara", "clarisse", "constance", "cristal", "daisy", "demi", "diane", "djamila", "dona", "donna", "dora", "eden", "elle", "elsa", "essence", "fabia", "fleur", "fortune", "francine", "françoise", "frédérique", "gabrielle", "geneviève", "germaine", "gilberte", "gloria", "harmonie", "hélène", "inès", "io", "iris", "irma", "isabelle", "jeanne", "jenny", "jessica", "jeunesse", "johanna", "jolie", "josette", "julie", "juliette", "lalla", "laura", "laure", "leni", "lien", "lili", "lis", "lisa", "lise", "liv", "lois", "lola", "lorraine", "louise", "lourdes", "lucie", "lucienne", "lucile", "lucille", "madeleine", "mai", "malin", "marge", "marguerite", "mari", "maria", "marie", "marine", "martha", "marthe", "mary", "mette", "michèle", "mimi", "miracle", "monique", "monta", "nadine", "nana", "nancy", "natale", "nathalie", "nicole", "olga", "page", "pandora", "patience", "patricia", "paule", "perle", "pris", "prudence", "rachel", "reine", "romaine", "rosa", "rose", "rosette", "rosita", "rue", "salut", "sarah", "sens", "sera", "signe", "sigrid", "solange", "sophie", "su", "suzanne", "sybil", "sylvie", "thérèse", "unique", "ursula", "vanessa", "varvara", "venus", "véronique", "vi", "victoire", "vienne", "violet", "violette", "virginie", "yvonne"};

    public float toiletBuildup = 0;
    public float foodBuildup = 0;
    public float cafeineBuildup = 0;

    public List<Character> friends;

    private void Start()
    {
        gameObject.name = realName;
        toiletBuildup = 0;
        foodBuildup = 0;
        cafeineBuildup = 0;
        //friends = new List<Character>();
        TimeManager.instance.On5MinutesUpdate += UpdateNeeds;
        UpdateNavMeshAgentStats();
    }

    public void RandomizeCharacter()
    {
        string[] nameList = Random.value < .5f ? femaleNames : maleNames;
        realName = nameList[Random.Range(0, nameList.Length - 1)];
        physicalCondition = (1+Random.value)/2;
        studious = Random.value;
        social = Random.value;
        toiletNeeds = 1 + Random.value*2;
        foodNeeds = .5f + Random.value;
        cafeineNeeds = Random.value < .5f ? 1000 : 0.5f + Random.value*2; // Some people don't need cafeine and some are addicts
        UpdateNavMeshAgentStats();
        RandomColor();
    }

    public void UpdateNeeds()
    {
        float buildupSpeed = CharacterManager.instance.needsBuildupSpeed;
        toiletBuildup += buildupSpeed / 48; // 4 hours to reach 1
        foodBuildup += buildupSpeed / 36; // 3 hours to reach 1
        cafeineBuildup += buildupSpeed / 15; // 1h15 to reach 1
    }

    public bool NeedsFood()
    {
        return foodBuildup > foodNeeds;
    }

    public bool NeedsToilet()
    {
        return toiletBuildup > toiletNeeds;
    }

    public bool NeedsCafein()
    {
        return cafeineBuildup > cafeineNeeds;
    }

    public void ResetNeed(int needId)
    {
        switch (needId)
        {
            case 0:
                foodBuildup = 0;
                break;
            case 1:
                toiletBuildup = 0;
                break;
            case 2:
                cafeineBuildup = 0;
                break;
        }
    }

    public void RandomColor()
    {
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        Renderer rend = GetComponentInChildren<Renderer>();

        rend.GetPropertyBlock(propBlock);

        propBlock.SetColor("_Color", new Color(physicalCondition, studious, social));
        rend.SetPropertyBlock(propBlock);
    }

    private void UpdateNavMeshAgentStats()
    {
        NavMeshAgent nav = GetComponent<NavMeshAgent>();
        if (nav != null)
            nav.speed *= physicalCondition;
    }
}


public enum Mood {Calm, Happy, Flirty, Tired, Bored, Depressed, Sad, Angry, Hungry, Thirsty, Neutral}