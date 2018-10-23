using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGeneration : MonoBehaviour
{
    /// <summary>
    /// Manages the modular map generation as the game plays
    /// Initially sorts all map sections into different list
    /// Sections are then pooled ahead of the player in random orders in random rotations
    /// Sections are sorted into different groups, different groups are used the further the player progresses
    /// </summary>

    public static MapGeneration mapInstance;

    [SerializeField]
    Transform firstGroupParent, secondGroupParent, thirdGroupParent, fourthGroupParent, emptyParent, activeParent, bossSectionParent;

    private GameObject currentSection, lastSection;
    private Transform playerTransform, sectionPickups;
    private List<GameObject> activeSections, firstGroupSections, secondGroupSections, thirdGroupSections, fourthGroupSections, emptySections, bossSpawnSection;

    private float spawnZ = 0.0f;
    private float sectionLength;    //100f; ~ length of tube section with asset on the end. 96f; ~ this is the length for tube sections without the additional asset on the end.
    private int[] sectionRotations = { 0, 45, 90, 135, 180, 225, 270, 315 }; //Array of the different roations the section can be placed.                        
    private int numTubesOnScreen = 6; //Script will only pool a number of sections in front of the player up to this value.


    void Awake()
    {
        mapInstance = this;
    }


    void Start()
    {
        playerTransform = Player.playerInstance.gameObject.transform;
        lastSection = null;
        currentSection = null;
        sectionLength = 96.0f;

        activeSections = new List<GameObject>();

        //Sets up the lists of section gameobjects to be used in the generation.
        //Any new section groups will need a list to be created for them and for the sections to be added to that list from the transform of the parent object within the editor.
        emptySections = new List<GameObject>();
        foreach (Transform section in emptyParent)
        {
            emptySections.Add(section.gameObject);
        }

        firstGroupSections = new List<GameObject>();
        foreach (Transform section in firstGroupParent)
        {
            firstGroupSections.Add(section.gameObject);
        }

        secondGroupSections = new List<GameObject>();
        foreach (Transform section in secondGroupParent)
        {
            secondGroupSections.Add(section.gameObject);
        }

        thirdGroupSections = new List<GameObject>();
        foreach (Transform section in thirdGroupParent)
        {
            thirdGroupSections.Add(section.gameObject);
        }

        fourthGroupSections = new List<GameObject>();
        foreach (Transform section in fourthGroupParent)
        {
            fourthGroupSections.Add(section.gameObject);
        }

        bossSpawnSection = new List<GameObject>();
        bossSpawnSection.Add(bossSectionParent.GetChild(0).gameObject);

        //Generate the initial bunch of sections, giving the number of sections to be generated and which groups they are being pulled from.
        //Can use sections from any groups but the total number should equal the 'numTubesOnScreen' value - any that go over this limit will not be spawned.
        PullSection(3, "EmptySection");
        PullSection(3, "FirstGroupSection");
    }

    /// <summary>
    /// Method used to move sections in front of the player given the number of sections to pull and which groups to pull from as parameters.
    /// Ideally there should always be x sections ahead of the player, where x is equal to 'numTubesOnScreen'.
    /// </summary>
    /// <param name="count"></param>
    /// <param name="sectionGroup"></param>
    private void PullSection(int count, string sectionGroup)
    {
        //Loops by the number of sections to pull
        for (int i = 0; i < count; i++)
        {
            //Will only pull if the number of active sections (sections in front of the player) is less than the max number of sections allowed.
            if (activeSections.Count < numTubesOnScreen)
            {
                GameObject pulledSection;

                //Switch statement based on the group to be pulled from.
                switch (sectionGroup)
                {
                    //With each case; a temporary gameobject is set up and used to pull a random section object from the chosen group.
                    //That section is then moved in front of the section before it based of the tube length.
                    //The pulled section is then removed from its previous group list and added to the list of active sections - this is to ensure that the section the player is already in or one ahead of the player isnt then moved.
                    //The transform of the object within the editor is also changed to be a child of the active sections object - this makes it easier to see which sections are in use within the editor.
                    //Before the pulled section is moved into place, all pickup objects are set to active, otherwise they would not appear again once picked up.
                    //Default case set up as safety net, will pull an empty section to ensure that the level doesnt just end.
                    //If default case is called then check for typos between passed sectionGroup string and case strings.
                    case "EmptySection":
                        pulledSection = emptySections[0];

                        sectionLength = 96.0f;

                        pulledSection.transform.SetParent(activeParent);
                        pulledSection.transform.position = Vector3.forward * spawnZ;
                        spawnZ += sectionLength;

                        activeSections.Add(pulledSection);
                        emptySections.Remove(pulledSection);
                        break;
                    case "FirstGroupSection":
                        pulledSection = firstGroupSections[Random.Range(0, firstGroupSections.Count)];
                        sectionPickups = pulledSection.transform.GetChild(0);

                        foreach (Transform sectionObject in sectionPickups)
                        {
                            sectionObject.gameObject.SetActive(true);
                        }

                        sectionLength = 96.0f;

                        pulledSection.transform.SetParent(activeParent);
                        pulledSection.transform.position = Vector3.forward * spawnZ;
                        pulledSection.transform.eulerAngles = new Vector3(0, 0, sectionRotations[Random.Range(0, sectionRotations.Length)]);
                        spawnZ += sectionLength;

                        activeSections.Add(pulledSection);
                        firstGroupSections.Remove(pulledSection);
                        break;
                    case "SecondGroupSection":
                        pulledSection = secondGroupSections[Random.Range(0, secondGroupSections.Count)];
                        sectionPickups = pulledSection.transform.GetChild(0);

                        foreach (Transform sectionObject in sectionPickups)
                        {
                            sectionObject.gameObject.SetActive(true);
                        }

                        sectionLength = 100.0f;

                        pulledSection.transform.SetParent(activeParent);
                        pulledSection.transform.position = Vector3.forward * spawnZ;
                        pulledSection.transform.eulerAngles = new Vector3(0, 0, sectionRotations[Random.Range(0, sectionRotations.Length)]);
                        spawnZ += sectionLength;

                        activeSections.Add(pulledSection);
                        secondGroupSections.Remove(pulledSection);
                        break;
                    case "ThirdGroupSection":
                        pulledSection = thirdGroupSections[Random.Range(0, thirdGroupSections.Count)];
                        sectionPickups = pulledSection.transform.GetChild(0);

                        foreach (Transform sectionObject in sectionPickups)
                        {
                            sectionObject.gameObject.SetActive(true);
                        }

                        sectionLength = 192.0f;

                        pulledSection.transform.SetParent(activeParent);
                        pulledSection.transform.position = Vector3.forward * spawnZ;
                        pulledSection.transform.eulerAngles = new Vector3(0, 0, sectionRotations[Random.Range(0, sectionRotations.Length)]);
                        spawnZ += sectionLength;

                        activeSections.Add(pulledSection);
                        thirdGroupSections.Remove(pulledSection);
                        break;
                    case "FourthGroupSection":
                        pulledSection = thirdGroupSections[Random.Range(0, fourthGroupSections.Count)];
                        sectionPickups = pulledSection.transform.GetChild(0);

                        foreach (Transform sectionObject in sectionPickups)
                        {
                            sectionObject.gameObject.SetActive(true);
                        }

                        sectionLength = 384.0f;

                        pulledSection.transform.SetParent(activeParent);
                        pulledSection.transform.position = Vector3.forward * spawnZ;
                        pulledSection.transform.eulerAngles = new Vector3(0, 0, sectionRotations[Random.Range(0, sectionRotations.Length)]);
                        spawnZ += sectionLength;

                        activeSections.Add(pulledSection);
                        fourthGroupSections.Remove(pulledSection);
                        break;
                    case "BossSpawnSection":
                        pulledSection = bossSpawnSection[0];

                        sectionLength = 192.0f;

                        pulledSection.transform.SetParent(activeParent);
                        pulledSection.transform.position = Vector3.forward * spawnZ;
                        spawnZ += sectionLength;

                        activeSections.Add(pulledSection);
                        bossSpawnSection.Remove(pulledSection);
                        break;
                    default:
                        pulledSection = emptySections[0];

                        sectionLength = 96.0f;

                        pulledSection.transform.SetParent(activeParent);
                        pulledSection.transform.position = Vector3.forward * spawnZ;
                        spawnZ += sectionLength;

                        activeSections.Add(pulledSection);
                        emptySections.Remove(pulledSection);
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Called from the player script when the player enters the trigger for a section start.
    /// When used will remove the previous section from list of active sections and add it back into its group list depending on the sections tag.
    /// The removed section can then be used again within the 'PullSection' method, if the section is not pulled again it will remain in its previous position.
    /// </summary>
    public void RemoveSection()
    {
        if (lastSection != null)
        {
            switch (lastSection.tag)
            {
                case "EmptySection":
                    lastSection.transform.SetParent(emptyParent);
                    emptySections.Add(lastSection);
                    activeSections.Remove(lastSection);
                    break;
                case "FirstGroupSection":
                    lastSection.transform.SetParent(firstGroupParent);
                    firstGroupSections.Add(lastSection);
                    activeSections.Remove(lastSection);
                    break;
                case "SecondGroupSection":
                    lastSection.transform.SetParent(secondGroupParent);
                    secondGroupSections.Add(lastSection);
                    activeSections.Remove(lastSection);
                    break;
                case "ThirdGroupSection":
                    lastSection.transform.SetParent(thirdGroupParent);
                    thirdGroupSections.Add(lastSection);
                    activeSections.Remove(lastSection);
                    break;
                case "FourthGroupSection":
                    lastSection.transform.SetParent(fourthGroupParent);
                    fourthGroupSections.Add(lastSection);
                    activeSections.Remove(lastSection);
                    break;
                case "BossSpawnSection":
                    lastSection.transform.SetParent(bossSectionParent);
                    bossSpawnSection.Add(lastSection);
                    activeSections.Remove(lastSection);
                    break;
                default:
                    break;
            }
        }

        //Once a section is removed a new one is then pulled to replace it ahead of the player.
        //Uses the Distance value from the player script to determine which group(s) of sections should be used.
        //If multiple groups can be used within a distance range, then use if statments and a random number range to determine the probability of each group being used.
        //Can use this to set when more difficult sections will start to appear based on player progression.
        //Can also use this to pull individual sections such as enemy spawns or boss sections.
        if (Boss.bossInstance.Alive)
        {
            PullSection(1, "EmptySection");
        }
        else if (Player.playerInstance.Distance > 50.0f && Player.playerInstance.Distance <= 1500.0f)
        {
            PullSection(1, "FirstGroupSection");
        }
        else if (Player.playerInstance.Distance > 1500.0f && Player.playerInstance.Distance <= 4000.0f)
        {
            int randInt = Random.Range(0, 10);

            if (randInt <= 7)
            {
                PullSection(1, "FirstGroupSection");
            }
            else
            {
                PullSection(1, "SecondGroupSection");
            }
        }
        else if (Player.playerInstance.Distance > 4000.0f && Player.playerInstance.Distance <= 7000.0f)
        {
            int randInt = Random.Range(0, 10);

            if (randInt <= 3)
            {
                PullSection(1, "FirstGroupSection");
            }
            else
            {
                PullSection(1, "SecondGroupSection");
            }
        }
        else if (Player.playerInstance.Distance > 7000.0f && Player.playerInstance.Distance <= 10000.0f)
        {

            PullSection(1, "SecondGroupSection");
        }

        else if (Player.playerInstance.Distance > 10000.0f && Player.playerInstance.Distance < 13000.0f)
        {
            PullSection(1, "ThirdGroupSection");
        }
        else if (Player.playerInstance.Distance > 13000.0f && !Boss.bossInstance.Defeated)
        {
            PullSection(1, "BossSpawnSection");
            Boss.bossInstance.Alive = true;
        }
        else if (Player.playerInstance.Distance > 15000.0f && Boss.bossInstance.Defeated)
        {
            int randInt = Random.Range(0, 10);

            if (randInt <= 4)
            {
                PullSection(1, "EmptySection");
            }
            else
            {
                PullSection(1, "ThirdGroupSection");
            }
        }

    }

    #region publicAccessors
    public GameObject CurrentSection
    {
        get
        {
            return currentSection;
        }

        set
        {
            currentSection = value;
        }
    }


    public GameObject LastSection
    {
        get
        {
            return lastSection;
        }

        set
        {
            lastSection = value;
        }
    }
    #endregion
}
