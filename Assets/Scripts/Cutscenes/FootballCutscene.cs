using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Playables;

public class FootballCutscene : MonoBehaviour
{
    [System.Serializable]
    public class LyricLineWithColor
    {
        public string text;
        public Color speakerColor;
    }

    [Header("UI")]
    [SerializeField] private TMP_Text lyricsDisplay;
    [SerializeField] private Image lyricsBackground;

    [Header("Timeline")]
    [SerializeField] public PlayableDirector director;

    [Header("Crowd Storming")]
    [SerializeField] private GameObject crowdStorming;

    [SerializeField] GameObject crowdLDM;
    [SerializeField] GameObject crowdHDM;
    [SerializeField] GameObject crowdPersonPrefab;

    [Header("HDM Crowd")]
    [SerializeField] int crowdWidth = 40;
    [SerializeField] int crowdDepth = 20;
    [SerializeField] float spacing = 5f;
    [SerializeField] int randomSeed = 7;

    [Tooltip("Low detail fake shader material")]
    [SerializeField] private Material ldmMaterial;

    [Tooltip("GPU instanced procedural crowd material")]
    [SerializeField] private Material notldmMaterial;

    [SerializeField] private Renderer crowdRenderer;

    [Header("Debug")]
    [SerializeField] private bool forceOverride = false;
    [SerializeField] private bool forceHighDetail = false;
    [SerializeField] private bool forceLowDetail = false;

    [Header("Storm Settings")]
    [SerializeField] private float crowdStormDuration = 6f;
    [SerializeField] private Vector3 crowdEndPosition =
        new(0, 1, 0);

    [SerializeField] private float cameraShakeStrength = 0.15f;

    [Header("Scene Transition")]
    [SerializeField] private CanvasGroup fadeCanvas;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private int nextSceneIndex = 58;

    private List<LyricLineWithColor> lyrics;
    private int currentLine = 0;

    private bool lowDetailMode;

    public static double dspStartTime;

    void ParseLyrics()
    {
        lyrics = new List<LyricLineWithColor>();

        foreach (string line in lyricsText)
        {
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            string[] parts = line.Split(':', 4);

            if (parts.Length < 4)
            {
                continue;
            } 

            byte.TryParse(parts[1], out byte r);
            byte.TryParse(parts[2], out byte g);
            byte.TryParse(parts[3], out byte b);

            Color c = new Color32(r, g, b, 128);

            if (r + g + b == 0)
            {
                c = new Color32(0, 0, 0, 0);
            }

            lyrics.Add(
                new LyricLineWithColor
                {
                    text = parts[0].Trim(),
                    speakerColor = c
                }
            );
        }
    }

    [SerializeField] public string[] lyricsText;

    void Start()
    {
        crowdLDM = crowdStorming;
        fadeCanvas.alpha = 0f;
        SetupDetailMode();

        SetupCrowdMaterial();

        lyricsText = BuildDialogue();

        ParseLyrics();

        StartCoroutine(StartCutsceneDSP());
    }

    void SetupDetailMode()
    {
        int useEffect =
            PlayerPrefs.GetInt("useVisualEffects", 0);

        lowDetailMode =
            useEffect == 0;

        if (forceOverride)
        {
            if (forceHighDetail)
            {
                lowDetailMode = false;
            }

            if (forceLowDetail)
            {
                lowDetailMode = true;
            }
        }
    }

    void SetupCrowdMaterial()
    {
        crowdLDM.SetActive(false);
        crowdHDM.SetActive(false);

        if (lowDetailMode)
        {
            crowdLDM.SetActive(true);

            Renderer r =
                crowdLDM.GetComponent<Renderer>();

            r.material = ldmMaterial;
        }
        else
        {
            crowdHDM.SetActive(true);

            GenerateHighDetailCrowd();
        }
    }

    string[] BuildDialogue()
    {
        List<string> lines =
            new()
            {
                ":0:0:0",
                "We're here at Pen You Never City in a close game against Cookie Cutter...:255:0:0",
                "PYNC currently with the ball, trailing 35-37.:0:0:255",
                "It's fourth and 26 from the 50 yard line...:255:0:0",
                "Only two seconds left on the clock...:0:0:255",
                "PYNC in field goal formation...:0:0:255",
                "With it being a 67 yard field goal, it looks like it's all over for PYNC...:255:0:0",
                ":0:0:0",
                "Jang with the kick, it's up...:255:0:0",
                "It looks good, it looks VERY good...:255:0:0",
                "IT'S GOOD! PYNC WINS 38-37!:255:0:0",
                "Fire whoever runs the jumbotron camera, that was terrible!:0:0:255",
                "THE STUDENTS ARE RUSHING THE FIELD!:255:0:0"
            };

        if (lowDetailMode)
        {
            lines.Add(
                "This crowd is really looking like a net of enthusiastic students!:255:0:0");

            lines.Add(
                "This is what happens when you don't have an artist...:255:0:0");

            lines.Add(
                "Hey, this is the wrong game for abstract circles!:0:0:255");

            lines.Add(
                "That should be Love Sees Differences!:0:0:255");
        }
        else
        {
            lines.Add(
                "If you're not in low detail mode, the crowd is now storming the field!:255:0:0");

            lines.Add(
                "A bunch of of procedurally generated students now rendering at once.:0:0:255");

            lines.Add(
                "Aren't we all procedurally generated to some extent?:255:0:0");

            lines.Add(
                "I have no idea why we're breaking the fourth wall...:0:0:255");
        }

        lines.Add(":0:0:0");

        lines.Add(
            "And now, for a special presentation by the PYNC Game Dev program!:255:0:0");

        lines.Add(
            "ROLL THE TRACK!:255:0:0");

        return lines.ToArray();
    }

    IEnumerator StartCutsceneDSP()
    {
        yield return null;

        dspStartTime =
            AudioSettings.dspTime + 0.2;

        director.time = 0;
        director.initialTime = 0;

        director.Play();

        while (AudioSettings.dspTime < dspStartTime)
        {
            yield return null;
        }

        director.playableGraph
            .GetRootPlayable(0)
            .SetSpeed(1);
    }

    void Update()
    {
        if (currentLine >= lyrics.Count)
        {
            return;
        }
        
        lyricsDisplay.text =
            lyrics[currentLine].text;

        lyricsBackground.color =
            lyrics[currentLine].speakerColor;
    }

    public void increaseCurrentLine()
    {
        currentLine++;
    }

    public void DoCrowdStorm()
    {
        StartCoroutine(CrowdStormRoutine());
    }

    void GenerateHighDetailCrowd()
    {
        Random.InitState(randomSeed);

        for (int x = 0; x < crowdWidth; x++)
        {
            for (int z = 0; z < crowdDepth; z++)
            {
                GameObject p =
                    Instantiate(
                        crowdPersonPrefab,
                        crowdHDM.transform
                    );

                float jitterX =
                    Random.Range(-4.2f, 4.2f);

                float jitterZ =
                    Random.Range(-4.2f, 4.2f);

                p.transform.localPosition =
                    new Vector3(
                        (x - crowdWidth / 2f) * spacing + jitterX,
                        6.7f,
                        (z - crowdDepth / 2f) * spacing + jitterZ
                    );

                p.transform.localScale =
                    Vector3.one *
                    Random.Range(6.7f, 16.7f);

                Renderer r =
                    p.GetComponent<Renderer>();

                Material mat =
                    new(notldmMaterial);

                mat.SetColor(
                    "_ShirtColor",
                    Random.ColorHSV(
                        0f, 1f,
                        0.5f, 1f,
                        0.5f, 1f
                    )
                );

                mat.SetFloat(
                    "_HeightOffset",
                    Random.Range(0f, 1f)
                );

                r.material = mat;
            }
        }
    }

    IEnumerator CrowdStormRoutine()
    {
        crowdStorming.SetActive(true);

        Vector3 initialPos =
            crowdStorming.transform.position;

        float elapsed = 0;

        Camera cam =
            Camera.main;

        Vector3 originalCamPos =
            cam.transform.position;

        while (elapsed < crowdStormDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                elapsed / crowdStormDuration;

            if (lowDetailMode)
            {
                crowdLDM.transform.position =
                    Vector3.Lerp(
                        initialPos,
                        crowdEndPosition,
                        t
                    );
            }
            else
            {
                crowdHDM.transform.position =
                    Vector3.Lerp(
                        initialPos,
                        crowdEndPosition,
                        t
                    );
            }

            // Slight camera shake
            if (cam != null)
            {
                Vector3 shake =
                    Random.insideUnitSphere
                    * cameraShakeStrength
                    * (1 - t);

                cam.transform.position =
                    originalCamPos + shake;
            }

            yield return null;
        }

        if (cam != null)
        {
            cam.transform.position =
                originalCamPos;
        }
    }

    public void BeginMusicVideoTransition()
    {
        StartCoroutine(
            TransitionToDanceCutscene()
        );
    }

    IEnumerator TransitionToDanceCutscene()
    {
        float elapsed = 0;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                elapsed / fadeDuration;

            if (fadeCanvas != null)
            {
                fadeCanvas.alpha = t;
            }

            yield return null;
        }

        SceneManager.LoadScene(nextSceneIndex);
    }

    public void goToDanceCutscene()
    {
        BeginMusicVideoTransition();
    }
}