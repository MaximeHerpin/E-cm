using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Character : MonoBehaviour {
    [HideInInspector]
    public bool isSelected = false;
    [HideInInspector]
    public bool isHighlighted = false;
    public Canvas CharacterUi;
    public DiaryController spawnedUi;
    private Queue<DiaryEntry> diary;

    public string realName;
    public CharacterJob job = CharacterJob.Student;
    public Mood mood;

    public float physicalCondition = 1f; // Will influence speed, activities, ect
    public float studious = 1; // How serious in studies
    public float social = 1; // Qualifies interactions
    public float sqrDetectionRadius = 36f;

    public float toiletNeeds = 1;
    public float foodNeeds = 1;
    public float cafeineNeeds = 1;

    public float toiletBuildup = 0;
    public float foodBuildup = 0;
    public float cafeineBuildup = 0;

    private Color color = Color.gray;

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
        diary = new Queue<DiaryEntry>();
        sqrDetectionRadius = Mathf.Pow(GetComponent<DetectionComponent>().detectionRadius, 2);

    }

    public void RandomizeCharacter(string name, CharacterJob job = CharacterJob.Student)
    {
        realName = name;
        gameObject.name = realName;
        physicalCondition = (1+Random.value)/2;
        studious = Random.value;
        social = Random.value;
        toiletNeeds = 1 + Random.value*2;
        foodNeeds = .5f + Random.value;
        cafeineNeeds = Random.value < .5f ? 1000 : 0.5f + Random.value*2; // Some people don't need cafeine and some are addicts
        UpdateNavMeshAgentStats();
        RandomColor();
        this.job = job;
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
        color = new Color(physicalCondition, studious, social);
        propBlock.SetColor("_Color", color);
        rend.SetPropertyBlock(propBlock);
    }

    private void UpdateNavMeshAgentStats()
    {
        NavMeshAgent nav = GetComponent<NavMeshAgent>();
        if (nav != null)
            nav.speed *= physicalCondition;
    }

    public void SpawnCharacterUi()
    {
        if (spawnedUi != null)
            return;

        Vector3 pos = Camera.main.WorldToScreenPoint(transform.position) - Vector3.up * 100;
        spawnedUi = Instantiate(CharacterUi).GetComponent<DiaryController>();
        spawnedUi.transform.GetChild(0).position = pos;
        spawnedUi.AddEntries(diary);
        spawnedUi.UpdateNameAndStatus(realName, job.ToString());
    
    }

    public void AddDiaryEntry(string description)
    {
        DiaryEntry entry = new DiaryEntry(TimeManager.instance.timeOfDay, description);
        diary.Enqueue(entry);
        if (spawnedUi != null)
            spawnedUi.AddEntry(entry);
    }

    public void Select()
    {
        isSelected = true;
        SpawnCharacterUi();
    }

    public void Deselect()
    {
        isSelected = false;
    }

    void OnMouseEnter()
    {
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        Renderer rend = GetComponentInChildren<Renderer>();
        rend.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", color * 0.5f);
        rend.SetPropertyBlock(propBlock);
        isHighlighted = true;
    }

    private void OnMouseExit()
    {
        MaterialPropertyBlock propBlock = new MaterialPropertyBlock();
        Renderer rend = GetComponentInChildren<Renderer>();
        rend.GetPropertyBlock(propBlock);
        propBlock.SetColor("_Color", color);
        rend.SetPropertyBlock(propBlock);
        isHighlighted = false;
    }

}


public enum Mood {Calm, Happy, Flirty, Tired, Bored, Depressed, Sad, Angry, Hungry, Thirsty, Neutral}

public enum CharacterJob { Student, Professor, Administration, Worker}