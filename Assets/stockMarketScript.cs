using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using stockMarket;

public class stockMarketScript : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMAudio Audio;

    //buttons
    public KMSelectable cycleLeftButton;
    public KMSelectable cycleRightButton;
    public KMSelectable investButton;

    //companies
    public String[] companyOptions;
    public String[] companyOptionsABV;
    public String[] companyName;
    public TextMesh[] companyNameDisplay;
    private List<int> selectedCompanies = new List<int>();
    int increaser = 0;

    //companyValues
    private float[] companyStartValue = new float[4];
    private float companyStartValueEdit = 0;
    private float[] company1Flux = new float[4];
    private float[] company2Flux = new float[4];
    private float[] company3Flux = new float[4];
    private float[] company4Flux = new float[4];
    private string[] company1FluxString = new string[4];
    private string[] company2FluxString = new string[4];
    private string[] company3FluxString = new string[4];
    private string[] company4FluxString = new string[4];
    private float[] company1Values = new float[4];
    private float[] company2Values = new float[4];
    private float[] company3Values = new float[4];
    private float[] company4Values = new float[4];
    private bool editAdded = false;

    //startValues
    public float[] startValues;
    public Color[] colorOptions;

    //display
    public TextMesh displayedCompany;
    private int displayIndex;

    //figuresGeneral
    public TextMesh[] displayedFigures;
    public String blank = "";
    public Color[] textColors;
    public GameObject[] upDown;

    //figures1
    private GameObject arrow1;
    int quarter1 = 0;
    int figureLength1 = 0;
    int character1 = 0;
    Coroutine pricesCoroutine1;

    //figures2
    private GameObject arrow2;
    int quarter2 = 0;
    int figureLength2 = 0;
    int character2 = 0;
    Coroutine pricesCoroutine2;

    //figures3
    private GameObject arrow3;
    int quarter3 = 0;
    int figureLength3 = 0;
    int character3 = 0;
    Coroutine pricesCoroutine3;

    //figures4
    private GameObject arrow4;
    int quarter4 = 0;
    int figureLength4 = 0;
    int character4 = 0;
    Coroutine pricesCoroutine4;

    //logic
    private List<float> quarterlyCompanyValues = new List<float>();
    private int[] companyPoints = new int[4];
    private int[] fluxPoints1 = new int[4];
    private int[] fluxPoints2 = new int[4];
    private int[] fluxPoints3 = new int[4];
    private int[] fluxPoints4 = new int[4];
    private int[] totalFluxPoints = new int[4];
    private int[] totalBaseValue = new int[4];
    private string correctAnswer = "";

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved = false;

    void Awake()
    {
        moduleId = moduleIdCounter++;
        cycleLeftButton.OnInteract += delegate () { onCycleLeftButton(); return false; };
        cycleRightButton.OnInteract += delegate () { onCycleRightButton(); return false; };
        investButton.OnInteract += delegate () { onInvestButton(); return false; };
    }

    void clearAll()
    {
        StopCoroutine(pricesCoroutine1);
        StopCoroutine(pricesCoroutine2);
        StopCoroutine(pricesCoroutine3);
        StopCoroutine(pricesCoroutine4);
        quarter1 = 0;
        figureLength1 = 0;
        character1 = 0;
        quarter2 = 0;
        figureLength2 = 0;
        character2 = 0;
        quarter3 = 0;
        figureLength3 = 0;
        character3 = 0;
        quarter4 = 0;
        figureLength4 = 0;
        character4 = 0;
        companyPoints[0] = 0;
        companyPoints[1] = 0;
        companyPoints[2] = 0;
        companyPoints[3] = 0;
        fluxPoints1[0] = 0;
        fluxPoints1[1] = 0;
        fluxPoints1[2] = 0;
        fluxPoints1[3] = 0;
        fluxPoints2[0] = 0;
        fluxPoints2[1] = 0;
        fluxPoints2[2] = 0;
        fluxPoints2[3] = 0;
        fluxPoints3[0] = 0;
        fluxPoints3[1] = 0;
        fluxPoints3[2] = 0;
        fluxPoints3[3] = 0;
        fluxPoints4[0] = 0;
        fluxPoints4[1] = 0;
        fluxPoints4[2] = 0;
        fluxPoints4[3] = 0;
        totalFluxPoints[0] = 0;
        totalFluxPoints[1] = 0;
        totalFluxPoints[2] = 0;
        totalFluxPoints[3] = 0;
        totalBaseValue[0] = 0;
        totalBaseValue[1] = 0;
        totalBaseValue[2] = 0;
        totalBaseValue[3] = 0;
        Start();
    }

    void Start()
    {
        foreach(GameObject upDown in upDown)
        {
            upDown.SetActive(false);
        }
        foreach(TextMesh price in displayedFigures)
        {
            price.text = "";
        }
        for (var i = 0; i < company4Flux.Length; i++)
        {
            companyStartValue[i] = 0;
        }
        displayIndex = UnityEngine.Random.Range(0,12);
        displayedCompany.text = companyOptions[displayIndex];
        pickCompanies();
        pickStartValues();
        pickFlux();
        logic();
        pricesCoroutine1 = StartCoroutine(prices1());
        pricesCoroutine2 = StartCoroutine(prices2());
        pricesCoroutine3 = StartCoroutine(prices3());
        pricesCoroutine4 = StartCoroutine(prices4());
        Debug.LogFormat("[The Stock Market #{0}] Company 1 is {1}. Start value: {2}. Q1 value: {3}. Q2 value: {4}. Q3 value: {5}. Q4 value: {6}.", moduleId, companyName[0], companyStartValue[0], company1Values[0].ToString(), company1Values[1].ToString(), company1Values[2].ToString(), company1Values[3].ToString());
        Debug.LogFormat("[The Stock Market #{0}] Fluctuations of {1}. Q1: {2}. Q2: {3}. Q3: {4}. Q4: {5}.", moduleId, companyName[0], company1FluxString[0], company1FluxString[1], company1FluxString[2], company1FluxString[3]);
        Debug.LogFormat("[The Stock Market #{0}] Base points of {1}: {2}.", moduleId, companyName[0], totalBaseValue[0]);
        Debug.LogFormat("[The Stock Market #{0}] Fluctuation points of {1}: {2}.", moduleId, companyName[0], totalFluxPoints[0]);

        Debug.LogFormat("[The Stock Market #{0}] Company 2 is {1}. Start value: {2}. Q1 value: {3}. Q2 value: {4}. Q3 value: {5}. Q4 value: {6}.", moduleId, companyName[1], companyStartValue[1], company2Values[0].ToString(), company2Values[1].ToString(), company2Values[2].ToString(), company2Values[3].ToString());
        Debug.LogFormat("[The Stock Market #{0}] Fluctuations of {1}. Q1: {2}. Q2: {3}. Q3: {4}. Q4: {5}.", moduleId, companyName[1], company2FluxString[0], company2FluxString[1], company2FluxString[2], company2FluxString[3]);
        Debug.LogFormat("[The Stock Market #{0}] Base points of {1}: {2}.", moduleId, companyName[1], totalBaseValue[1]);
        Debug.LogFormat("[The Stock Market #{0}] Total fluctuation points of {1}: {2}.", moduleId, companyName[1], totalFluxPoints[1]);

        Debug.LogFormat("[The Stock Market #{0}] Company 3 is {1}. Start value: {2}. Q1 value: {3}. Q2 value: {4}. Q3 value: {5}. Q4 value: {6}.", moduleId, companyName[2], companyStartValue[2], company3Values[0].ToString(), company3Values[1].ToString(), company3Values[2].ToString(), company3Values[3].ToString());
        Debug.LogFormat("[The Stock Market #{0}] Fluctuations of {1}. Q1: {2}. Q2: {3}. Q3: {4}. Q4: {5}.", moduleId, companyName[2], company3FluxString[0], company3FluxString[1], company3FluxString[2], company3FluxString[3]);
        Debug.LogFormat("[The Stock Market #{0}] Base points of {1}: {2}.", moduleId, companyName[2], totalBaseValue[2]);
      Debug.LogFormat("[The Stock Market #{0}] Total fluctuation points of {1}: {2}.", moduleId, companyName[2], totalFluxPoints[2]);

        Debug.LogFormat("[The Stock Market #{0}] Company 4 is {1}. Start value: {2}. Q1 value: {3}. Q2 value: {4}. Q3 value: {5}. Q4 value: {6}.", moduleId, companyName[3], companyStartValue[3], company4Values[0].ToString(), company4Values[1].ToString(), company4Values[2].ToString(), company4Values[3].ToString());
        Debug.LogFormat("[The Stock Market #{0}] Fluctuations of {1}. Q1: {2}. Q2: {3}. Q3: {4}. Q4: {5}.", moduleId, companyName[3], company4FluxString[0], company4FluxString[1], company4FluxString[2], company4FluxString[3]);
        Debug.LogFormat("[The Stock Market #{0}] Base points of {1}: {2}.", moduleId, companyName[3], totalBaseValue[3]);
        Debug.LogFormat("[The Stock Market #{0}] Total fluctuation points of {1}: {2}.", moduleId, companyName[3], totalFluxPoints[3]);

        Debug.LogFormat("[The Stock Market #{0}] {1} points are {2}. {3} points are {4}. {5} points are {6}. {7} points are {8}.", moduleId, companyName[0], companyPoints[0], companyName[1], companyPoints[1], companyName[2], companyPoints[2], companyName[3], companyPoints[3]);
        Debug.LogFormat("[The Stock Market #{0}] The correct investment is {1}.", moduleId, correctAnswer);

    }

    public void onCycleLeftButton()
    {
        if(moduleSolved)
        {
            return;
        }
        GetComponent<KMSelectable>().AddInteractionPunch(0.5f);
        Audio.PlaySoundAtTransform("keyStroke", transform);
        displayIndex = (displayIndex + 11) % 12;
        displayedCompany.text = companyOptions[displayIndex];
    }

    public void onCycleRightButton()
    {
        if(moduleSolved)
        {
            return;
        }
        GetComponent<KMSelectable>().AddInteractionPunch(0.5f);
        Audio.PlaySoundAtTransform("keyStroke", transform);
        displayIndex = (displayIndex + 1) % 12;
        displayedCompany.text = companyOptions[displayIndex];
    }

    public void onInvestButton()
    {
        if(moduleSolved)
        {
            return;
        }
        GetComponent<KMSelectable>().AddInteractionPunch();
        if(displayedCompany.text == correctAnswer)
        {
            Audio.PlaySoundAtTransform("cash", transform);
            GetComponent<KMBombModule>().HandlePass();
            Debug.LogFormat("[The Stock Market #{0}] You have invested in {1}. That is correct. Module disarmed.", moduleId, correctAnswer);
            moduleSolved = true;
        }
        else
        {
            GetComponent<KMBombModule>().HandleStrike();
            Debug.LogFormat("[The Stock Market #{0}] Strike! You have invested in {1}. That is incorrect. Module reset.", moduleId, displayedCompany.text);
            clearAll();
        }

    }

    void pickCompanies()
    {
        foreach(TextMesh company in companyNameDisplay)
        {
            int index = UnityEngine.Random.Range(0,12);
            while(selectedCompanies.Contains(index))
            {
                index = UnityEngine.Random.Range(0,12);
            }
            selectedCompanies.Add(index);
            company.text = companyOptionsABV[index];
            companyName[increaser] = companyOptions[index];
            increaser++;
        }
        increaser = 0;
        selectedCompanies.Clear();
    }

    void pickStartValues()
    {
        foreach(TextMesh company in companyNameDisplay)
        {
            int index = UnityEngine.Random.Range(0,8);
            company.color = colorOptions[index];
            companyStartValue[increaser] = startValues[index];
            increaser++;
        }
        increaser = 0;
    }

    void pickFlux()
    {
        for (var i = 0; i < company1Flux.Length; i++)
        {
            float fluxValue = UnityEngine.Random.Range(5f,50f);
            int negator = UnityEngine.Random.Range(0,2);
            if(negator == 0)
            {
                company1Flux[i] = fluxValue - (fluxValue % 0.01f);
            }
            else
            {
                company1Flux[i] = fluxValue - (fluxValue % 0.01f);
                company1Flux[i] = 0 - company1Flux[i];
            }
            if(company1Flux[i] > 0)
            {
                company1FluxString[i] = "+" + company1Flux[i].ToString("00.00");
            }
            else
            {
                company1FluxString[i] = company1Flux[i].ToString("00.00");
            }
        }
        for (var i = 0; i < company2Flux.Length; i++)
        {
            float fluxValue = UnityEngine.Random.Range(5f,50f);
            int negator = UnityEngine.Random.Range(0,2);
            if(negator == 0)
            {
                company2Flux[i] = fluxValue - (fluxValue % 0.01f);
            }
            else
            {
                company2Flux[i] = fluxValue - (fluxValue % 0.01f);
                company2Flux[i] = 0 - company2Flux[i];
            }
            if(company2Flux[i] > 0)
            {
                company2FluxString[i] = "+" + company2Flux[i].ToString("00.00");
            }
            else
            {
                company2FluxString[i] = company2Flux[i].ToString("00.00");
            }
        }
        for (var i = 0; i < company3Flux.Length; i++)
        {
            float fluxValue = UnityEngine.Random.Range(5f,50f);
            int negator = UnityEngine.Random.Range(0,2);
            if(negator == 0)
            {
                company3Flux[i] = fluxValue - (fluxValue % 0.01f);
            }
            else
            {
                company3Flux[i] = fluxValue - (fluxValue % 0.01f);
                company3Flux[i] = 0 - company3Flux[i];
            }
            if(company3Flux[i] > 0)
            {
                company3FluxString[i] = "+" + company3Flux[i].ToString("00.00");
            }
            else
            {
                company3FluxString[i] = company3Flux[i].ToString("00.00");
            }
        }
        for (var i = 0; i < company4Flux.Length; i++)
        {
            float fluxValue = UnityEngine.Random.Range(5f,50f);
            int negator = UnityEngine.Random.Range(0,2);
            if(negator == 0)
            {
                company4Flux[i] = fluxValue - (fluxValue % 0.01f);
            }
            else
            {
                company4Flux[i] = fluxValue - (fluxValue % 0.01f);
                company4Flux[i] = 0 - company4Flux[i];
            }
            if(company4Flux[i] > 0)
            {
                company4FluxString[i] = "+" + company4Flux[i].ToString("00.00");
            }
            else
            {
                company4FluxString[i] = company4Flux[i].ToString("00.00");
            }
        }

        for (var i = 0; i < company1Values.Length; i++)
        {
            if(editAdded == false)
            {
                companyStartValueEdit = companyStartValue[0];
            }
            company1Values[i] = companyStartValueEdit + company1Flux[i];
            companyStartValueEdit = companyStartValueEdit + company1Flux[i];
            editAdded = true;
        }
        editAdded = false;
        for (var i = 0; i < company2Values.Length; i++)
        {
            if(editAdded == false)
            {
                companyStartValueEdit = companyStartValue[1];
            }
            company2Values[i] = companyStartValueEdit + company2Flux[i];
            companyStartValueEdit = companyStartValueEdit + company2Flux[i];
            editAdded = true;
        }
        editAdded = false;
        for (var i = 0; i < company3Values.Length; i++)
        {
            if(editAdded == false)
            {
                companyStartValueEdit = companyStartValue[2];
            }
            company3Values[i] = companyStartValueEdit + company3Flux[i];
            companyStartValueEdit = companyStartValueEdit + company3Flux[i];
            editAdded = true;
        }
        editAdded = false;
        for (var i = 0; i < company4Values.Length; i++)
        {
            if(editAdded == false)
            {
                companyStartValueEdit = companyStartValue[3];
            }
            company4Values[i] = companyStartValueEdit + company4Flux[i];
            companyStartValueEdit = companyStartValueEdit + company4Flux[i];
            editAdded = true;
        }
        editAdded = false;
    }

    IEnumerator prices1()
    {
        while(moduleSolved == false)
        {
            quarter1 = 0;
            yield return new WaitForSeconds(3f);
            while (quarter1 < 4)
            {
                displayedFigures[0].text = "";
                figureLength1 = company1FluxString[quarter1].Count();
                character1 = 0;
                while(figureLength1 > displayedFigures[0].text.Count())
                {
                    if(company1Flux[quarter1] > 0 && character1 == 0)
                    {
                        displayedFigures[0].color = textColors[0];
                    }
                    else if(character1 == 0)
                    {
                        displayedFigures[0].color = textColors[1];
                    }
                    displayedFigures[0].text += company1FluxString[quarter1][character1];
                    yield return new WaitForSeconds(0.1f);
                    character1++;
                }
                if(company1Flux[quarter1] > 0)
                {
                    arrow1 = upDown[0];
                }
                else
                {
                    arrow1 = upDown[1];
                }
                character1 = 0;
                arrow1.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                while(character1 < 5)
                {
                    displayedFigures[0].text = blank;
                    arrow1.SetActive(false);
                    yield return new WaitForSeconds(0.3f);
                    displayedFigures[0].text = company1FluxString[quarter1];
                    arrow1.SetActive(true);
                    yield return new WaitForSeconds(0.8f);
                    character1++;
                }
                yield return new WaitForSeconds(1f);
                quarter1++;
                arrow1.SetActive(false);
                displayedFigures[0].text = blank;
                if(quarter1 == 5)
                {
                    quarter1 = 0;
                }
            }
        }
        foreach(TextMesh company in companyNameDisplay)
        {
            company.text = "";
        }
    }

    IEnumerator prices2()
    {
        while(moduleSolved == false)
        {
            quarter2 = 0;
            yield return new WaitForSeconds(3f);
            while (quarter2 < 4)
            {
                displayedFigures[1].text = "";
                figureLength2 = company2FluxString[quarter2].Count();
                character2 = 0;
                while(figureLength2 > displayedFigures[1].text.Count())
                {
                    if(company2Flux[quarter2] > 0 && character2 == 0)
                    {
                        displayedFigures[1].color = textColors[0];
                    }
                    else if(character2 == 0)
                    {
                        displayedFigures[1].color = textColors[1];
                    }
                    displayedFigures[1].text += company2FluxString[quarter2][character2];
                    yield return new WaitForSeconds(0.1f);
                    character2++;
                }
                if(company2Flux[quarter2] > 0)
                {
                    arrow2 = upDown[2];
                }
                else
                {
                    arrow2 = upDown[3];
                }
                character2 = 0;
                arrow2.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                while(character2 < 5)
                {
                    displayedFigures[1].text = blank;
                    arrow2.SetActive(false);
                    yield return new WaitForSeconds(0.3f);
                    displayedFigures[1].text = company2FluxString[quarter2];
                    arrow2.SetActive(true);
                    yield return new WaitForSeconds(0.8f);
                    character2++;
                }
                yield return new WaitForSeconds(1f);
                quarter2++;
                arrow2.SetActive(false);
                displayedFigures[1].text = blank;
                if(quarter2 == 5)
                {
                    quarter2 = 0;
                }
            }
        }
    }

    IEnumerator prices3()
    {
        while(moduleSolved == false)
        {
            quarter3 = 0;
            yield return new WaitForSeconds(3f);
            while (quarter3 < 4)
            {
                displayedFigures[2].text = "";
                figureLength3 = company3FluxString[quarter3].Count();
                character3 = 0;
                while(figureLength3 > displayedFigures[2].text.Count())
                {
                    if(company3Flux[quarter3] > 0 && character3 == 0)
                    {
                        displayedFigures[2].color = textColors[0];
                    }
                    else if(character3 == 0)
                    {
                        displayedFigures[2].color = textColors[1];
                    }
                    displayedFigures[2].text += company3FluxString[quarter3][character3];
                    yield return new WaitForSeconds(0.1f);
                    character3++;
                }
                if(company3Flux[quarter3] > 0)
                {
                    arrow3 = upDown[4];
                }
                else
                {
                    arrow3 = upDown[5];
                }
                character3 = 0;
                arrow3.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                while(character3 < 5)
                {
                    displayedFigures[2].text = blank;
                    arrow3.SetActive(false);
                    yield return new WaitForSeconds(0.3f);
                    displayedFigures[2].text = company3FluxString[quarter3];
                    arrow3.SetActive(true);
                    yield return new WaitForSeconds(0.8f);
                    character3++;
                }
                yield return new WaitForSeconds(1f);
                quarter3++;
                arrow3.SetActive(false);
                displayedFigures[2].text = blank;
                if(quarter3 == 5)
                {
                    quarter3 = 0;
                }
            }
        }
    }

    IEnumerator prices4()
    {
        while(moduleSolved == false)
        {
            quarter4 = 0;
            yield return new WaitForSeconds(3f);
            while (quarter4 < 4)
            {
                displayedFigures[3].text = "";
                figureLength4 = company4FluxString[quarter4].Count();
                character4 = 0;
                while(figureLength4 > displayedFigures[3].text.Count())
                {
                    if(company4Flux[quarter4] > 0 && character4 == 0)
                    {
                        displayedFigures[3].color = textColors[0];
                    }
                    else if(character4 == 0)
                    {
                        displayedFigures[3].color = textColors[1];
                    }
                    displayedFigures[3].text += company4FluxString[quarter4][character4];
                    yield return new WaitForSeconds(0.1f);
                    character4++;
                }
                if(company4Flux[quarter4] > 0)
                {
                    arrow4 = upDown[6];
                }
                else
                {
                    arrow4 = upDown[7];
                }
                character4 = 0;
                arrow4.SetActive(true);
                yield return new WaitForSeconds(0.1f);
                while(character4 < 5)
                {
                    displayedFigures[3].text = blank;
                    arrow4.SetActive(false);
                    yield return new WaitForSeconds(0.3f);
                    displayedFigures[3].text = company4FluxString[quarter4];
                    arrow4.SetActive(true);
                    yield return new WaitForSeconds(0.8f);
                    character4++;
                }
                yield return new WaitForSeconds(1f);
                quarter4++;
                arrow4.SetActive(false);
                displayedFigures[3].text = blank;
                if(quarter4 == 5)
                {
                    quarter4 = 0;
                }
            }
        }
    }

    void logicCompile()
    {
        foreach(float value in company1Values)
        {
            quarterlyCompanyValues.Add(value);
        }
        foreach(float value in company2Values)
        {
            quarterlyCompanyValues.Add(value);
        }
        foreach(float value in company3Values)
        {
            quarterlyCompanyValues.Add(value);
        }
        foreach(float value in company4Values)
        {
            quarterlyCompanyValues.Add(value);
        }
    }
    void logic()
    {
        logicCompile();

        var maxIndex = quarterlyCompanyValues.IndexOf(quarterlyCompanyValues.Max());
        if(maxIndex == 0 || maxIndex == 1 || maxIndex == 2 || maxIndex == 3)
        {
            companyPoints[0] += 30;
            quarterlyCompanyValues[0] = 0;
            quarterlyCompanyValues[1] = 0;
            quarterlyCompanyValues[2] = 0;
            quarterlyCompanyValues[3] = 0;
        }
        else if(maxIndex == 4 || maxIndex == 5 || maxIndex == 6 || maxIndex == 7)
        {
            companyPoints[1] += 30;
            quarterlyCompanyValues[4] = 0;
            quarterlyCompanyValues[5] = 0;
            quarterlyCompanyValues[6] = 0;
            quarterlyCompanyValues[7] = 0;
        }
        else if(maxIndex == 8 || maxIndex == 9 || maxIndex == 10 || maxIndex == 11)
        {
            companyPoints[2] += 30;
            quarterlyCompanyValues[8] = 0;
            quarterlyCompanyValues[9] = 0;
            quarterlyCompanyValues[10] = 0;
            quarterlyCompanyValues[11] = 0;
        }
        else
        {
            companyPoints[3] += 30;
            quarterlyCompanyValues[12] = 0;
            quarterlyCompanyValues[13] = 0;
            quarterlyCompanyValues[14] = 0;
            quarterlyCompanyValues[15] = 0;
        }

        maxIndex = quarterlyCompanyValues.IndexOf(quarterlyCompanyValues.Max());
        if(maxIndex == 0 || maxIndex == 1 || maxIndex == 2 || maxIndex == 3)
        {
            companyPoints[0] += 27;
            quarterlyCompanyValues[0] = 0;
            quarterlyCompanyValues[1] = 0;
            quarterlyCompanyValues[2] = 0;
            quarterlyCompanyValues[3] = 0;
        }
        else if(maxIndex == 4 || maxIndex == 5 || maxIndex == 6 || maxIndex == 7)
        {
            companyPoints[1] += 27;
            quarterlyCompanyValues[4] = 0;
            quarterlyCompanyValues[5] = 0;
            quarterlyCompanyValues[6] = 0;
            quarterlyCompanyValues[7] = 0;
        }
        else if(maxIndex == 8 || maxIndex == 9 || maxIndex == 10 || maxIndex == 11)
        {
            companyPoints[2] += 27;
            quarterlyCompanyValues[8] = 0;
            quarterlyCompanyValues[9] = 0;
            quarterlyCompanyValues[10] = 0;
            quarterlyCompanyValues[11] = 0;
        }
        else
        {
            companyPoints[3] += 27;
            quarterlyCompanyValues[12] = 0;
            quarterlyCompanyValues[13] = 0;
            quarterlyCompanyValues[14] = 0;
            quarterlyCompanyValues[15] = 0;
        }

        maxIndex = quarterlyCompanyValues.IndexOf(quarterlyCompanyValues.Max());
        if(maxIndex == 0 || maxIndex == 1 || maxIndex == 2 || maxIndex == 3)
        {
            companyPoints[0] += 24;
            quarterlyCompanyValues[0] = 0;
            quarterlyCompanyValues[1] = 0;
            quarterlyCompanyValues[2] = 0;
            quarterlyCompanyValues[3] = 0;
        }
        else if(maxIndex == 4 || maxIndex == 5 || maxIndex == 6 || maxIndex == 7)
        {
            companyPoints[1] += 24;
            quarterlyCompanyValues[4] = 0;
            quarterlyCompanyValues[5] = 0;
            quarterlyCompanyValues[6] = 0;
            quarterlyCompanyValues[7] = 0;
        }
        else if(maxIndex == 8 || maxIndex == 9 || maxIndex == 10 || maxIndex == 11)
        {
            companyPoints[2] += 24;
            quarterlyCompanyValues[8] = 0;
            quarterlyCompanyValues[9] = 0;
            quarterlyCompanyValues[10] = 0;
            quarterlyCompanyValues[11] = 0;
        }
        else
        {
            companyPoints[3] += 24;
            quarterlyCompanyValues[12] = 0;
            quarterlyCompanyValues[13] = 0;
            quarterlyCompanyValues[14] = 0;
            quarterlyCompanyValues[15] = 0;
        }

        if(companyPoints[0] == 0)
        {
            companyPoints[0] = 21;
        }
        else if(companyPoints[1] == 0)
        {
            companyPoints[1] = 21;
        }
        else if(companyPoints[2] == 0)
        {
            companyPoints[2] = 21;
        }
        else
        {
            companyPoints[3] = 21;
        }
        quarterlyCompanyValues.Clear();
        logicCompile();

        var minIndex = quarterlyCompanyValues.IndexOf(quarterlyCompanyValues.Min());
        if(minIndex == 0 || minIndex == 1 || minIndex == 2 || minIndex == 3)
        {
            companyPoints[0] -= 15;
            quarterlyCompanyValues[0] = 1000;
            quarterlyCompanyValues[1] = 1000;
            quarterlyCompanyValues[2] = 1000;
            quarterlyCompanyValues[3] = 1000;
        }
        else if(minIndex == 4 || minIndex == 5 || minIndex == 6 || minIndex == 7)
        {
            companyPoints[1] -= 15;
            quarterlyCompanyValues[4] = 1000;
            quarterlyCompanyValues[5] = 1000;
            quarterlyCompanyValues[6] = 1000;
            quarterlyCompanyValues[7] = 1000;
        }
        else if(minIndex == 8 || minIndex == 9 || minIndex == 10 || minIndex == 11)
        {
            companyPoints[2] -= 15;
            quarterlyCompanyValues[8] = 1000;
            quarterlyCompanyValues[9] = 1000;
            quarterlyCompanyValues[10] = 1000;
            quarterlyCompanyValues[11] = 1000;
        }
        else
        {
            companyPoints[3] -= 15;
            quarterlyCompanyValues[12] = 1000;
            quarterlyCompanyValues[13] = 1000;
            quarterlyCompanyValues[14] = 1000;
            quarterlyCompanyValues[15] = 1000;
        }

        minIndex = quarterlyCompanyValues.IndexOf(quarterlyCompanyValues.Min());
        if(minIndex == 0 || minIndex == 1 || minIndex == 2 || minIndex == 3)
        {
            companyPoints[0] -= 13;
            quarterlyCompanyValues[0] = 1000;
            quarterlyCompanyValues[1] = 1000;
            quarterlyCompanyValues[2] = 1000;
            quarterlyCompanyValues[3] = 1000;
        }
        else if(minIndex == 4 || minIndex == 5 || minIndex == 6 || minIndex == 7)
        {
            companyPoints[1] -= 13;
            quarterlyCompanyValues[4] = 1000;
            quarterlyCompanyValues[5] = 1000;
            quarterlyCompanyValues[6] = 1000;
            quarterlyCompanyValues[7] = 1000;
        }
        else if(minIndex == 8 || minIndex == 9 || minIndex == 10 || minIndex == 11)
        {
            companyPoints[2] -= 13;
            quarterlyCompanyValues[8] = 1000;
            quarterlyCompanyValues[9] = 1000;
            quarterlyCompanyValues[10] = 1000;
            quarterlyCompanyValues[11] = 1000;
        }
        else
        {
            companyPoints[3] -= 13;
            quarterlyCompanyValues[12] = 1000;
            quarterlyCompanyValues[13] = 1000;
            quarterlyCompanyValues[14] = 1000;
            quarterlyCompanyValues[15] = 1000;
        }

        minIndex = quarterlyCompanyValues.IndexOf(quarterlyCompanyValues.Min());
        if(minIndex == 0 || minIndex == 1 || minIndex == 2 || minIndex == 3)
        {
            companyPoints[0] -= 11;
            quarterlyCompanyValues[0] = 1000;
            quarterlyCompanyValues[1] = 1000;
            quarterlyCompanyValues[2] = 1000;
            quarterlyCompanyValues[3] = 1000;
        }
        else if(minIndex == 4 || minIndex == 5 || minIndex == 6 || minIndex == 7)
        {
            companyPoints[1] -= 11;
            quarterlyCompanyValues[4] = 1000;
            quarterlyCompanyValues[5] = 1000;
            quarterlyCompanyValues[6] = 1000;
            quarterlyCompanyValues[7] = 1000;
        }
        else if(minIndex == 8 || minIndex == 9 || minIndex == 10 || minIndex == 11)
        {
            companyPoints[2] -= 11;
            quarterlyCompanyValues[8] = 1000;
            quarterlyCompanyValues[9] = 1000;
            quarterlyCompanyValues[10] = 1000;
            quarterlyCompanyValues[11] = 1000;
        }
        else
        {
            companyPoints[3] -= 11;
            quarterlyCompanyValues[12] = 1000;
            quarterlyCompanyValues[13] = 1000;
            quarterlyCompanyValues[14] = 1000;
            quarterlyCompanyValues[15] = 1000;
        }

        minIndex = quarterlyCompanyValues.IndexOf(quarterlyCompanyValues.Min());
        if(minIndex == 0 || minIndex == 1 || minIndex == 2 || minIndex == 3)
        {
            companyPoints[0] -= 9;
            quarterlyCompanyValues[0] = 1000;
            quarterlyCompanyValues[1] = 1000;
            quarterlyCompanyValues[2] = 1000;
            quarterlyCompanyValues[3] = 1000;
        }
        else if(minIndex == 4 || minIndex == 5 || minIndex == 6 || minIndex == 7)
        {
            companyPoints[1] -= 9;
            quarterlyCompanyValues[4] = 1000;
            quarterlyCompanyValues[5] = 1000;
            quarterlyCompanyValues[6] = 1000;
            quarterlyCompanyValues[7] = 1000;
        }
        else if(minIndex == 8 || minIndex == 9 || minIndex == 10 || minIndex == 11)
        {
            companyPoints[2] -= 9;
            quarterlyCompanyValues[8] = 1000;
            quarterlyCompanyValues[9] = 1000;
            quarterlyCompanyValues[10] = 1000;
            quarterlyCompanyValues[11] = 1000;
        }
        else
        {
            companyPoints[3] -= 9;
            quarterlyCompanyValues[12] = 1000;
            quarterlyCompanyValues[13] = 1000;
            quarterlyCompanyValues[14] = 1000;
            quarterlyCompanyValues[15] = 1000;
        }
        quarterlyCompanyValues.Clear();

        totalBaseValue[0] = companyPoints[0];
        totalBaseValue[1] = companyPoints[1];
        totalBaseValue[2] = companyPoints[2];
        totalBaseValue[3] = companyPoints[3];

        int fluxPointInc = 0;
        foreach(float flux in company1Flux)
        {
            fluxPoints1[fluxPointInc] = (int)flux / 5;
            fluxPointInc++;
        }
        foreach(int points in fluxPoints1)
        {
            if(points >= 5)
            {
                totalFluxPoints[0] += (10 + ((points-5) * 4));
            }
            else if (points <= -5)
            {
                int newPoints = points+5;
                totalFluxPoints[0] += (newPoints * 4) - 10;
            }
            else
            {
                totalFluxPoints[0] += (points * 2);
            }
        }

        fluxPointInc = 0;
        foreach(float flux in company2Flux)
        {
            fluxPoints2[fluxPointInc] = (int)flux / 5;
            fluxPointInc++;
        }
        foreach(int points in fluxPoints2)
        {
            if(points >= 5)
            {
                totalFluxPoints[1] += (10 + ((points-5) * 4));
            }
            else if (points <= -5)
            {
                int newPoints = points+5;
                totalFluxPoints[1] += (newPoints * 4) - 10;
            }
            else
            {
                totalFluxPoints[1] += (points * 2);
            }
        }

        fluxPointInc = 0;
        foreach(float flux in company3Flux)
        {
            fluxPoints3[fluxPointInc] = (int)flux / 5;
            fluxPointInc++;
        }
        foreach(int points in fluxPoints3)
        {
            if(points >= 5)
            {
                totalFluxPoints[2] += (10 + ((points-5) * 4));
            }
            else if (points <= -5)
            {
                int newPoints = points+5;
                totalFluxPoints[2] += (newPoints * 4) - 10;
            }
            else
            {
                totalFluxPoints[2] += (points * 2);
            }
        }

        fluxPointInc = 0;
        foreach(float flux in company4Flux)
        {
            fluxPoints4[fluxPointInc] = (int)flux / 5;
            fluxPointInc++;
        }
        foreach(int points in fluxPoints4)
        {
            if(points >= 5)
            {
                totalFluxPoints[3] += (10 + ((points-5) * 4));
            }
            else if (points <= -5)
            {
                int newPoints = points+5;
                totalFluxPoints[3] += (newPoints * 4) - 10;
            }
            else
            {
                totalFluxPoints[3] += (points * 2);
            }
        }
        companyPoints[0] += totalFluxPoints[0];
        companyPoints[1] += totalFluxPoints[1];
        companyPoints[2] += totalFluxPoints[2];
        companyPoints[3] += totalFluxPoints[3];
        var correctIndex = Array.IndexOf(companyPoints, companyPoints.Max());
        correctAnswer = companyName[correctIndex];
    }

#pragma warning disable 414
	private string TwitchHelpMessage = "Submit HSBC with !{0} submit HSBC. You can also use the first letter of the company in the submit command.";
#pragma warning restore 414

	private IEnumerator ProcessTwitchCommand(string command)
	{
		command = command.Trim();
		string[] split = command.ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
		if (split[0] == "left")
		{
			yield return null;
			yield return new[] { cycleLeftButton };
		}
		else if (split[0] == "right")
		{
			yield return null;
			yield return new[] { cycleRightButton };
		}
		else if (split[0].EqualsAny("submit", "invest") && split.Length == 1)
		{
			yield return null;
			yield return new[] { investButton };
		}
		else if (split[0].EqualsAny("submit", "invest"))
		{
			bool valid = false;
			foreach (string company in companyOptions)
			{
				if (company.ToLowerInvariant().StartsWith(split[1][0].ToString()))
					valid = true;
			}
			if (!valid) yield break;

			yield return null;
			while (!displayedCompany.text.ToLowerInvariant().StartsWith(split[1][0].ToString()))
			{
				yield return null;
				yield return new[] { cycleRightButton };
				yield return "trycancel";
			}
			yield return new { investButton };
		}
		else yield break;
	}
}
