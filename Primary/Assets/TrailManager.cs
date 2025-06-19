 using System.Collections;
using System.Text;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.IO;
using UnityEngine.Serialization;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Security.Permissions;
using static System.Net.Mime.MediaTypeNames;
using System;

using Microsoft.MixedReality.Toolkit.UI;
using System.Diagnostics;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

public class TrailManager : MonoBehaviour
{
    private string dataFolderPath;           
    private string logFilePath;
    private string filePath;

    public int ButtonPressedglobal;
    public string appstartTime;
    private StreamWriter logStreamWriter;
    private StreamWriter trailStreamWriter;

    public int totalTrails = 10;
    public Material glowingMaterial; 

    private int currentTrailIndex = 0;

    public List<int> userEnteredPattern = new List<int>();

    public List<List<int>> predefinedPatterns = new List<List<int>>();
    private int currentPatternIndex = 0;
    private bool isDisplayingPattern = false;

    public AudioSource correctPatternAudioSource;
    public AudioSource wrongPatternAudioSource;
    public AudioSource endgameAudioSource;

    List<PressableButton> pressableButtons = new List<PressableButton>();

    List<Interactable> interactableButtons = new List<Interactable>();

    private Color originalButtonColor;

    private float trailStartTime;
    private bool inTrail = false;


    private List<bool> isButtonPressed = new List<bool>();
    private List<bool> isButtonTouched = new List<bool>();
    private List<float> buttonPressTimestamps = new List<float>();
    private List<int> pressedButtonNumbers = new List<int>();
    private List<float> buttonReleaseTimestamps = new List<float>();
    private List<int> releasedButtonNumbers = new List<int>();
    private List<float> buttonTouchTimestamps = new List<float>();
    private List<int> touchedButtonNumbers = new List<int>();



    public string patternstartTime;
    public float patternendTime;
    public float lastTimestamp;
    public float lastTimestampindex;

    public float restDuration = 15.0f;
    public float startdelay = 60f;
    public float inputDuration = 15f;

    Color glowColor = Color.red; 
    float glowDuration = 1.0f; 

    private void Start() 
    {
        appstartTime = DateTime.Now.ToString("HH:mm:ss.fff");
        dataFolderPath = UnityEngine.Application.persistentDataPath; 
        filePath = dataFolderPath + "/" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".txt";
        logFilePath = dataFolderPath + "/" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_log.txt";
        UnityEngine.Debug.Log("Filepath" + filePath);

        trailStreamWriter = new StreamWriter(filePath, true); 
        logStreamWriter = new StreamWriter(logFilePath, false); 

        currentPatternIndex = 0;

        GameObject roundbuttons = GameObject.Find("MixedRealitySceneContent/PressExamples/RoundButtons");

        if (roundbuttons != null)
        {
            for (int i = 0; i < roundbuttons.transform.childCount; i++)
            {
                Transform child = roundbuttons.transform.GetChild(i);
                UnityEngine.Debug.Log("Child " + i + ": " + child.name);

                PressableButton pressableButton = child.GetComponent<PressableButton>();

                Interactable interactableButton = child.GetComponent<Interactable>();
                if (interactableButton != null)
                {
                    interactableButtons.Add(interactableButton);

                    int currentButtonIndex = i;

                    interactableButton.OnClick.AddListener(() => OnClickEventHandler(currentButtonIndex));

                    UnityEngine.Debug.Log("Interactable script is found and attached to Button pressed at child" + child.name);
                }
                else
                {
                    UnityEngine.Debug.LogError("Interactable component not found on GameObject: " + child.name);
                }

                if (pressableButton != null)
                {
    
                    pressableButtons.Add(pressableButton);
                    isButtonPressed.Add(false);

                    int currentButtonIndex = i;

                    pressableButton.ButtonPressed.AddListener(() => OnButtonPressed(currentButtonIndex));
                    pressableButton.ButtonReleased.AddListener(() => OnButtonReleased(currentButtonIndex));

                    pressableButton.TouchBegin.AddListener(() => OnTouchBegin(currentButtonIndex));

                    pressableButton.TouchEnd.AddListener(() => OnTouchEnd(currentButtonIndex));

                    UnityEngine.Debug.Log("PressableButton script is found and attached to Button pressed at child" + child.name);
                }
                else
                {
                    UnityEngine.Debug.LogError("PressableButton component not found on GameObject: " + child.name);
                }


                UnityEngine.Component[] components = child.GetComponents<UnityEngine.Component>();
                foreach (var component in components)
                {
                    UnityEngine.Debug.Log("   Component: " + component.GetType().Name);
                }
            }
        }
        else
        {
            UnityEngine.Debug.LogError("RoundButtons GameObject not found in the scene.");
        }

        // Print the positions of each button
        for (int i = 0; i < pressableButtons.Count; i++)
        {
            PressableButton button = pressableButtons[i];
            Vector3 buttonPosition = button.transform.position;
            UnityEngine.Debug.Log("Button " + i + " Position: " + buttonPosition);

            // Find the CylinderContainer child object
            Transform cylinderContainer = button.transform.Find("ButtonContent/CylinderContainer");

            // Find the tube child object
            Transform tube = button.transform.Find("ButtonContent/Tube");
            if (tube != null)
            {
                Renderer tuberenderer = tube.GetComponent<Renderer>();
                if (tuberenderer != null)
                {
                    Material tubematerial = tuberenderer.material;
                    Color tubeColor = tubematerial.color;
                    UnityEngine.Debug.Log("Tube Color: " + tubeColor);
                    //tubematerial.color = Color.magenta;
                }
                else
                {
                    UnityEngine.Debug.LogWarning("Tube has no renderer or material for color.");
                }
                UnityEngine.Debug.Log("Tube found");
            }
            else
            {
                UnityEngine.Debug.Log("Tube not found");
            }

            if (cylinderContainer != null)
            {

                Transform cylinder = cylinderContainer.Find("Cylinder");
                if (cylinder != null)
                {
        
                    Renderer cylinderRenderer = cylinder.GetComponent<Renderer>();
                    if (cylinderRenderer != null)
                    {

                        Material cylinderMaterial = cylinderRenderer.material;
                        Color cylinderColor = cylinderMaterial.color;
                        cylinderMaterial.color = Color.red; 
                        UnityEngine.Debug.Log("Button " + i + " Color: " + cylinderColor);
                    }
                    else
                    {
                        UnityEngine.Debug.LogWarning("Button " + i + " Cylinder has no renderer or material for color.");
                    }
                }
                else
                {
                    UnityEngine.Debug.LogWarning("Button " + i + " Cylinder not found.");
                }
            }
            else
            {
                UnityEngine.Debug.LogWarning("Button " + i + " CylinderContainer not found.");
            }

        }


        InitializePatterns();

        Invoke("StartNewTrail", 60f);
    }
    

    public void StartNewTrail()
    {

            if (currentTrailIndex < 10)
            {
            List<int> currentPattern = predefinedPatterns[currentPatternIndex];


            StartCoroutine(DisplayPattern(currentPattern));


            trailStartTime = Time.time;
            inTrail = true;

            if (buttonPressTimestamps.Count > 0 && buttonPressTimestamps.Count == pressedButtonNumbers.Count)
            {

                logStreamWriter.BaseStream.SetLength(0);
                logStreamWriter.Flush();

                for (int i = 0; i < buttonPressTimestamps.Count; i++)
                {
                    logStreamWriter.WriteLine($"{buttonPressTimestamps[i]},{pressedButtonNumbers[i]}");
                }
                logStreamWriter.Flush();
            }


            UnityEngine.Debug.Log("log Data written to the file.");

        }
        else
        {
            correctPatternAudioSource.Play();
            logStreamWriter.Close();
            trailStreamWriter.Close();
            UnityEngine.Application.Quit();
        }
    }

    private void OnClickEventHandler(int buttonIndex)
    {

        string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        float difference = CalculateTimeDifference(appstartTime, timestamp);

        int buttonNumber = buttonIndex + 1;

        isButtonPressed[buttonIndex] = true;
        buttonPressTimestamps.Add(difference);
        pressedButtonNumbers.Add(buttonIndex);

        ButtonPressedglobal = buttonNumber;

        UnityEngine.Debug.Log("Button Clicked at Timestamp!" + timestamp + "button number" + buttonIndex);
        trailStreamWriter.WriteLine("Button " + (buttonIndex) + " pressed at: " + difference);
        trailStreamWriter.Flush();

    }

    private void InitializePatterns()
    {

        predefinedPatterns.Add(new List<int> { 1, 3, 7, 5 }); 
        predefinedPatterns.Add(new List<int> { 2, 4, 8, 6 }); 
        predefinedPatterns.Add(new List<int> { 0, 6, 3, 1 }); 
        predefinedPatterns.Add(new List<int> { 2, 5, 8, 4 }); 
        predefinedPatterns.Add(new List<int> { 7, 3, 6, 2 }); 
        predefinedPatterns.Add(new List<int> { 1, 5, 8, 4 }); 
        predefinedPatterns.Add(new List<int> { 3, 6, 0, 2 }); 
        predefinedPatterns.Add(new List<int> { 8, 7, 4, 1 }); 
        predefinedPatterns.Add(new List<int> { 1, 2, 3, 0 }); 
        predefinedPatterns.Add(new List<int> { 4, 5, 6, 8 }); 
    }

    private void OnDestroy()
    {
        if (trailStreamWriter != null)
        {
            trailStreamWriter.Close();
        }

        if (logStreamWriter != null)
        {
            logStreamWriter.Close();
        }
    }

   public void Update()
    {
        if (currentPatternIndex == 10)
        {
            correctPatternAudioSource.Play();
        }
    }

    private IEnumerator HighlightButtonColor(PressableButton button, Color targetColor, float highlightDuration)
    {

        Transform cylinderContainer = button.transform.Find("ButtonContent/Tube");

        if (cylinderContainer != null)
        {
            Renderer cylinderRenderer = cylinderContainer.GetComponent<Renderer>();

            if (cylinderRenderer != null)
            {
                Material cylinderMaterial = cylinderRenderer.material;
                Color originalColor = cylinderMaterial.color;


                cylinderMaterial.color = targetColor;


                yield return new WaitForSeconds(highlightDuration);


                cylinderMaterial.color = originalColor;
            }
        }
    }


    private IEnumerator DisplayPattern(List<int> pattern)
    {
        bool state = interactableButtons[0].IsEnabled;
        UnityEngine.Debug.Log("Button 0 is enabled!" + state);
        DisableAllButtons();

        state = interactableButtons[0].IsEnabled;
        UnityEngine.Debug.Log("Button is disabled!" + state);

        string startTime = DateTime.Now.ToString("HH:mm:ss.fff");
        float difference = CalculateTimeDifference(appstartTime, startTime);
        trailStreamWriter.WriteLine("Pattern " + (currentTrailIndex + 1) + " Start Time: " + difference);

        float displayDuration = 1.0f;
        foreach (int buttonIndex in pattern)
        {
     
            StartCoroutine(HighlightButtonColor(pressableButtons[buttonIndex], glowColor, glowDuration));
           
            string buttonPressTime = DateTime.Now.ToString("HH:mm:ss.fff");
            difference = CalculateTimeDifference(appstartTime, buttonPressTime);
            trailStreamWriter.WriteLine("Button " + (buttonIndex) + " shown at: " + difference);

          
            yield return new WaitForSeconds(1f);
        }
        
       
        string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        patternendTime = CalculateTimeDifference(appstartTime, timestamp);
        EnableAllButtons();
        state = interactableButtons[0].IsEnabled;
        UnityEngine.Debug.Log("Button is enabled again!" + state);

        string endTime = DateTime.Now.ToString("HH:mm:ss.fff");
        difference = CalculateTimeDifference(appstartTime, endTime);
        trailStreamWriter.WriteLine("Pattern " + (currentTrailIndex + 1) + " End Time: " + difference);
        UnityEngine.Debug.Log("Pattern " + (currentTrailIndex + 1) + " End Time: " + difference);

        yield return new WaitForSeconds(inputDuration);
        
        

        endTime = DateTime.Now.ToString("HH:mm:ss.fff");
        difference = CalculateTimeDifference(appstartTime, endTime);
        trailStreamWriter.WriteLine("Pattern " + (currentTrailIndex + 1) + " glow pattern starts: " + difference);
        UnityEngine.Debug.Log("Pattern " + (currentTrailIndex + 1) + "glow pattern starts:  " + difference);

        List<int> glowpattern = new List<int> { 0, 1, 2, 4, 6, 7, 8, 4, 0, 1, 2 };
        foreach (int buttonIndex in glowpattern)
        {
            StartCoroutine(HighlightButtonColor(pressableButtons[buttonIndex], Color.black, glowDuration));
          
            yield return new WaitForSeconds(1f);
        }

        yield return new WaitForSeconds(2.0f);
        endTime = DateTime.Now.ToString("HH:mm:ss.fff");
        difference = CalculateTimeDifference(appstartTime, endTime);
        trailStreamWriter.WriteLine("Pattern " + (currentTrailIndex + 1) + " glow pattern ends: " + difference);
        UnityEngine.Debug.Log("Pattern " + (currentTrailIndex + 1) + "glow pattern ends:  " + difference);

        currentTrailIndex++;
        currentPatternIndex++;

        StartNewTrail();


    }


    void OnButtonPressed(int buttonIndex)
    {

        isButtonPressed[buttonIndex] = true;

        float timestamp = Time.time;
        int buttonNumber = buttonIndex + 1;

        buttonPressTimestamps.Add(timestamp);
        pressedButtonNumbers.Add(buttonNumber);
        UnityEngine.Debug.Log(isButtonPressed);

        UnityEngine.Debug.Log("Button " + buttonNumber + " pressed at timestamp " + timestamp);
    }

    void OnButtonReleased(int buttonIndex)
    {

        isButtonPressed[buttonIndex] = false;
    }

    void OnTouchEnd(int buttonIndex)
    {
       
        isButtonTouched[buttonIndex] = false;

        
        float timestamp = Time.time;
        int buttonNumber = buttonIndex + 1;

        buttonReleaseTimestamps.Add(timestamp);
        releasedButtonNumbers.Add(buttonNumber);

        UnityEngine.Debug.Log("Button " + buttonNumber + " released at timestamp " + timestamp);
    }

    void OnTouchBegin(int buttonIndex)
    {
  
        isButtonTouched[buttonIndex] = true;

        float timestamp = Time.time;
        int buttonNumber = buttonIndex + 1;

        buttonTouchTimestamps.Add(timestamp);
        touchedButtonNumbers.Add(buttonNumber);

        UnityEngine.Debug.Log("Button " + buttonNumber + " touched at timestamp " + timestamp);
    }


    void DisableAllButtons()
    {
        foreach (Interactable interactableButton in interactableButtons)
        {
            interactableButton.IsEnabled = false;
        }
    }

    void EnableAllButtons()
    {
        foreach (Interactable interactableButton in interactableButtons)
        {
            interactableButton.IsEnabled = true;
        }
    }

    bool CheckPattern(List<int> pattern, List<int> userPattern)
    {
        bool isPatternCorrect = true;
        for (int k = 0; k < 4; k++)
        {
            if (userPattern[k] != pattern[k])
            {
                isPatternCorrect = false;
                UnityEngine.Debug.Log("Pattern check " + k + " failed: " + userPattern[k] + " != " + pattern[k]);
                break;
            }
        }

        return isPatternCorrect;
    }


    private void OnApplicationQuit()
    {
        if (trailStreamWriter != null)
        {
            trailStreamWriter.Close();
        }
    }
    public static float CalculateTimeDifference(string timestamp1Str, string timestamp2Str)
    {
        
        DateTime timestamp1 = DateTime.ParseExact(timestamp1Str, "HH:mm:ss.fff", CultureInfo.InvariantCulture);
        DateTime timestamp2 = DateTime.ParseExact(timestamp2Str, "HH:mm:ss.fff", CultureInfo.InvariantCulture);

        TimeSpan timeDifference = timestamp2 - timestamp1;
        float timeDifferenceInSeconds = (float)timeDifference.TotalMilliseconds / 1000.0f;

        return timeDifferenceInSeconds;
    }




    private GameObject GetChildWithName(GameObject parent, string name)
    {
        Transform[] children = parent.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.gameObject.name == name)
            {
                return child.gameObject;
            }
        }
        return null;
    }

}