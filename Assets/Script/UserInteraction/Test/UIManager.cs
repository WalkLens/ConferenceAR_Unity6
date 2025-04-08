using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider headerSlider;
    
    [SerializeField] private List<GameObject> pages;
    private int currentPage = 0;
    // public TMP_Text PIN; // PIN is set in PinInputManager
    public TMP_Text name_tmp;
    public TMP_Text Language;
    public TMP_Text Job;
    public TMP_Text Introduction_Text;
    public TMP_Text URL;

    public string pin;
    public string introduction_1;
    public string introduction_2;
    public string introduction_3;
    public string introduction_4;
    public string introduction_5;
    public string interest_1;
    public string interest_2;
    public string interest_3;
    public string interest_4;
    public string interest_5;
    public string photo_url;
    public bool autoaccept;
    
    public static UIManager Instance { get; private set;}
    private Dictionary<string, string> inputComponent;
    private Dictionary<string, string> inputData;
    private UserData userData;

    // Start is called before the first frame update
    void Start()
    {
        if(Instance == null)
        {
            Instance = this;
            userData = new UserData();
            headerSlider.maxValue = pages.Count;
        }
        else
        {
            Debug.LogWarning("UIManager already exists. This instance will be destroyed.");
            Destroy(this);
        }
    }

    public void OnNextButtonClicked()
    {
        if(currentPage < pages.Count - 1)
        {
            pages[currentPage].SetActive(false);
            pages[currentPage + 1].SetActive(true);
            currentPage += 1;
            headerSlider.value = currentPage + 1;
        }
        else // Last Page
        {
            UserData userData = new UserData
            {
                pin = pin,
                name = name_tmp.text,
                job = Job.text,
                language = Language.text,
                introduction_1 = introduction_1,
                introduction_2 = introduction_2,
                introduction_3 = introduction_3,
                introduction_4 = introduction_4,
                introduction_5 = introduction_5,
                interest_1 = interest_1,
                interest_2 = interest_2,
                interest_3 = interest_3,
                interest_4 = interest_4,
                interest_5 = interest_5,
                introduction_text = Introduction_Text.text,
                url = URL.text,
                photo_url = photo_url,
                autoaccept = autoaccept
            };
        }
    }

    public void OnSaveButtonClicked()
    {
        // UserData 객체 생성 및 데이터 초기화
        UserData userData = new UserData
        {
            pin = pin,
            name = name_tmp.text,
            job = Job.text,
            language = Language.text,
            introduction_1 = introduction_1,
            introduction_2 = introduction_2,
            introduction_3 = introduction_3,
            introduction_4 = introduction_4,
            introduction_5 = introduction_5,
            interest_1 = interest_1,
            interest_2 = interest_2,
            interest_3 = interest_3,
            interest_4 = interest_4,
            interest_5 = interest_5,
            introduction_text = Introduction_Text.text,
            url = URL.text,
            photo_url = photo_url,
            autoaccept = autoaccept
        };
        DatabaseManager.Instance.registerProfile(userData);
    }

    public UserData getInputData()
    {
        UserData userData = new UserData
        {
            pin = pin,
            name = name_tmp.text,
            job = Job.text,
            language = Language.text,
            introduction_1 = introduction_1,
            introduction_2 = introduction_2,
            introduction_3 = introduction_3,
            introduction_4 = introduction_4,
            introduction_5 = introduction_5,
            interest_1 = interest_1,
            interest_2 = interest_2,
            interest_3 = interest_3,
            interest_4 = interest_4,
            interest_5 = interest_5,
            introduction_text = Introduction_Text.text,
            url = URL.text,
            photo_url = photo_url,
            autoaccept = autoaccept
        };

        return userData;
    }
}
