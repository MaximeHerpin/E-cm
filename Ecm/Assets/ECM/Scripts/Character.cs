using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour {
    public bool randomizeCharacterAtStart = true;
    public bool isChromozomeXX = true;
    public string realName;
    public float physicalCondition = 1f; // Will influence speed, activities, ect
    public Mood mood;

    public float studious = 1; // How serious in studies
    public float social = 1; // Qualifies interactions

    public static string[] maleNames = { "abel", "achille", "adam", "adrien", "aimé", "al", "alain", "albert", "alby", "alexandre", "alfred", "alger", "ali", "alonso", "alphonse", "amaury", "anatole", "andré", "antoine", "anton", "archibald", "armand", "arnaud", "art", "arthur", "auguste", "augustin", "augustus", "bart", "bas", "beau", "ben", "benjamin", "bernard", "bertrand", "bile", "bill", "bob", "bras", "brian", "brin", "carlo", "carlos", "césar", "charles", "christ", "christophe", "clément", "constant", "daniel", "dante", "david", "denis", "désiré", "don", "duc", "édouard", "émile", "engin", "ernest", "étienne", "eugène", "eugenio", "félix", "ferdinand", "fernand", "franc", "france", "franck", "franco", "françois", "fred", "frédéric", "gabriel", "gaston", "george", "georges", "gérard", "germain", "gilbert", "gilles", "gosse", "guillaume", "gustave", "guy", "hall", "hector", "henri", "henry", "hermann", "hervé", "hilaire", "honoré", "hubert", "hugo", "innocent", "ira", "jack", "jacques", "james", "jarrett", "javier", "jeannot", "jérémie", "jérôme", "john", "joseph", "juan", "julien", "jure", "juste", "karl", "king", "lambert", "lance", "lasse", "laurent", "léon", "les", "lewis", "louis", "loup", "luc", "lucien", "ludo", "major", "manuel", "marc", "marcel", "marco", "marin", "marino", "marquis", "marquise", "mars", "martin", "mat", "mathias", "maurice", "max", "maxwell", "michel", "milan", "modeste", "monte", "mort", "nathan", "newton", "nicolas", "noble", "olivier", "om", "otto", "paris", "pascal", "patrick", "paul", "pedro", "philippe", "pierre", "platon", "porter", "prince", "raoul", "raphaël", "ravi", "raymond", "re", "régis", "renard", "rené", "richard", "robert", "roc", "roger", "roland", "romain", "roman", "roosevelt", "royal", "royale", "rudolf", "rufus", "samuel", "sans", "sébastien", "serge", "séverin", "simon", "stepan", "stéphane", "tel", "thierry", "thomas", "timothée", "travers", "tue", "urbain", "valéry", "victor", "ville", "vin", "vincent", "vitale", "washington", "werner", "william", "wilson", "winston", "wolf", "xavier", "york" };
    public static string[] femaleNames = { "adélaïde", "adèle", "agathe", "aimée", "alexandrie", "alice", "ami", "amie", "anita", "anna", "anne", "annie", "antoinette", "atalanta", "aude", "aura", "aurore", "avis", "avril", "béatrice", "belle", "berlin", "berthe", "betty", "blanche", "bride", "brigitte", "caprice", "carmen", "carole", "catherine", "cécile", "céleste", "charlotte", "cher", "christine", "ciel", "claire", "clara", "clarisse", "constance", "cristal", "daisy", "demi", "diane", "djamila", "dona", "donna", "dora", "eden", "elle", "elsa", "essence", "fabia", "fleur", "fortune", "francine", "françoise", "frédérique", "gabrielle", "geneviève", "germaine", "gilberte", "gloria", "harmonie", "hélène", "inès", "io", "iris", "irma", "isabelle", "jeanne", "jenny", "jessica", "jeunesse", "johanna", "jolie", "josette", "julie", "juliette", "lalla", "laura", "laure", "leni", "lien", "lili", "lis", "lisa", "lise", "liv", "lois", "lola", "lorraine", "louise", "lourdes", "lucie", "lucienne", "lucile", "lucille", "madeleine", "mai", "malin", "marge", "marguerite", "mari", "maria", "marie", "marine", "martha", "marthe", "mary", "mette", "michèle", "mimi", "miracle", "monique", "monta", "nadine", "nana", "nancy", "natale", "nathalie", "nicole", "olga", "page", "pandora", "patience", "patricia", "paule", "perle", "pris", "prudence", "rachel", "reine", "romaine", "rosa", "rose", "rosette", "rosita", "rue", "salut", "sarah", "sens", "sera", "signe", "sigrid", "solange", "sophie", "su", "suzanne", "sybil", "sylvie", "thérèse", "unique", "ursula", "vanessa", "varvara", "venus", "véronique", "vi", "victoire", "vienne", "violet", "violette", "virginie", "yvonne"};

    private void Start()
    {
        if (randomizeCharacterAtStart)
            RandomizeCharacter();

        gameObject.name = realName;
    }

    private void RandomizeCharacter()
    {
        isChromozomeXX = Random.value < .5f;
        string[] nameList = isChromozomeXX ? femaleNames : maleNames;
        realName = nameList[Random.Range(0, nameList.Length - 1)];
        physicalCondition = Random.value;
    }
}


public enum Mood {Calm, Happy, Flirty, Tired, Bored, Depressed, Sad, Angry, Hungry, Thirsty, Neutral}