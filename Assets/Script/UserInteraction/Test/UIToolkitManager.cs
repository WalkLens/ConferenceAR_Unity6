using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIToolkitManager : MonoBehaviour
{
    // 0. Common
    [SerializeField] private UIDocument uiDocument;
    
    private VisualElement root;
    private VisualElement container; // 컨텐츠를 감싸는 컨테이너
    private float screenWidth; // 각 뷰의 너비 (화면 크기)
    private int currentPage = 1;
    [SerializeField] private int totalPages = 7;
    
    private Button _submitButton;

    // 1. BasicInfoScreen
    private VisualElement _profileArea;
    
    // 2. KeywordScreen
    [SerializeField] private string[] researchChipString = {"게임 기획", "서비스 기획", "콘텐츠 기획", "프로젝트 매니징", "데이터 분석", "마케팅", "사업 기획"};
    [SerializeField] private string[] designChipString = {"그래픽 디자인", "UI/UX 디자인", "영상/애니메이션", "건축/조경", "패션 디자인", "게임 디자인", "산업 디자인", "순수 예술"};
    [SerializeField] private string[] devChipString = {"프론트엔드", "백엔드/서버", "풀스택", "게임 개발/그래픽", "데이터/AI", "Web 3.0", "블록체인", "데브옵스/지원", "임베디드 시스템"};

    private Button tabResearch;
    private Button tabDesign;
    private Button tabDev;
    private VisualElement keywordResearchContainer;
    private VisualElement keywordDesignContainer;
    private VisualElement keywordDevContainer;

    private int researchChipCount;
    private int designChipCount;
    private int devChipCount;

    // 3. InterestScreen

    // 4. IntroduceScreen

    // 5. RegisterCompleteScreen

    // 6. SetPINScreen
    private TextField PIN1;
    private TextField PIN2;
    private TextField PIN3;
    private TextField PIN4;
    private TextField PIN5;

    // 7. ConnectDeviceScreen
    
    private void OnEnable()
    {
        root = uiDocument.rootVisualElement;

        Debug.Log("연결완료");

        // VisualElement 생성 및 스타일 클래스 추가
        // 초기 화면 너비 출력
        UpdateScreenWidth();

        // 크기 변경 이벤트 등록
        root.RegisterCallback<GeometryChangedEvent>(evt => UpdateScreenWidth());


        // 0. Common
        container = root.Q<VisualElement>("Content");

        _submitButton = root.Q<Button>("SubmitButton");

        _submitButton.RegisterCallback<ClickEvent>(evt => Submit());

        // 1. BasicInfoScreen

        _profileArea = root.Q<VisualElement>("Photo");

        _profileArea.RegisterCallback<ClickEvent>(evt => UpdatePhoto());

        // 2. KeywordScreen
        tabResearch = root.Q<Button>("InterestResearch");
        tabDesign = root.Q<Button>("InterestDesign");
        tabDev = root.Q<Button>("InterestDev");
        tabResearch.RegisterCallback<ClickEvent>(evt => ToggleTabButton(tabResearch, 0));
        tabDesign.RegisterCallback<ClickEvent>(evt => ToggleTabButton(tabDesign, 1));
        tabDev.RegisterCallback<ClickEvent>(evt => ToggleTabButton(tabDev, 2));
        
        keywordResearchContainer = root.Q<VisualElement>("KeywordResearchChips");
        keywordDesignContainer = root.Q<VisualElement>("KeywordDesignChips");
        keywordDevContainer = root.Q<VisualElement>("KeywordDevChips");

        foreach(var chipString in researchChipString)
        {
            var chip = new Button
            {
                text = chipString
            };

            chip.RegisterCallback<ClickEvent>(evt => ToggleChip(chip, 0));
            chip.AddToClassList("chip");
            keywordResearchContainer.Add(chip);
        }

        foreach(var chipString in designChipString)
        {
            var chip = new Button
            {
                text = chipString
            };
            
            chip.RegisterCallback<ClickEvent>(evt => ToggleChip(chip, 1));
            chip.AddToClassList("chip");
            keywordDesignContainer.Add(chip);
        }

        foreach(var chipString in devChipString)
        {
            var chip = new Button
            {
                text = chipString
            };
            
            chip.RegisterCallback<ClickEvent>(evt => ToggleChip(chip, 2));
            chip.AddToClassList("chip");
            keywordDevContainer.Add(chip);
        }

        // 6. SetPINScreen
        PIN1 = root.Q<TextField>("PIN1");
        PIN2 = root.Q<TextField>("PIN2");
        PIN3 = root.Q<TextField>("PIN3");
        PIN4 = root.Q<TextField>("PIN4");
        PIN5 = root.Q<TextField>("PIN5");
    }

    private void UpdatePhoto()
    {
        Debug.Log("Update Photo");
    }

    private void Submit()
    {
        Debug.Log("Clicked!!");
        if (currentPage < totalPages)
        {
            currentPage++;
            UpdateContainerPosition();

            switch(currentPage){
                case 1:
                    _submitButton.text = "다음으로";
                    break;
                case 2:
                    _submitButton.text = "다음으로 (0/5)";
                    break;
                case 3:
                    _submitButton.text = "다음으로 (0/5)";
                    break;
                case 4:
                    _submitButton.text = "다음으로";
                    break;
                case 5:
                    _submitButton.text = "프로필을 확인했어요.";
                    break;
                case 6:
                    _submitButton.text = "TODO";
                    break;
                case 7:
                    _submitButton.text = "착용을 완료했어요.";
                    break;
            }
        }
    }

    private void UpdateContainerPosition()
    {
        // 새로운 위치 계산 (왼쪽으로 이동)
        float newX = -(currentPage - 1) * screenWidth;
        container.style.translate = new Translate(newX, 0, 0);
    }

    private void UpdateScreenWidth()
    {
        screenWidth = root.resolvedStyle.width;
        Debug.Log($"Updated Screen Width: {screenWidth}px");
    }

    private void ToggleTabButton(Button tab, int category)
    {
        if (tab.ClassListContains("tab-button-selected"))
        {
            Debug.Log("'tab-button-selected' 클래스가 존재합니다.");
            tab.RemoveFromClassList("tab-button-selected");
            switch(category)
            {
                case 0:
                    ;
                    break;
                case 1:
                    ;
                    break;
                case 2:
                    ;
                    break;
                default:
                    break;
            }
        }
        else // Chip Deactivated
        {
            Debug.Log("'tab-button-selected' 클래스가 없습니다.");
            tab.AddToClassList("tab-button-selected");
            switch(category)
            {
                case 0:
                    ;
                    break;
                case 1:
                    ;
                    break;
                case 2:
                    ;
                    break;
                default:
                    break;
            }
        }
    }
    // For all chips
    private void ToggleChip(Button chip, int category) // TODO ClickEvent 넘겨주는 방식으로 처리?
    {
        if (chip.ClassListContains("chip-active"))
        {
            Debug.Log("'chip-active' 클래스가 존재합니다.");
            chip.RemoveFromClassList("chip-active");
            switch(category)
            {
                case 0:
                    researchChipCount -= 1;
                    break;
                case 1:
                    designChipCount -= 1;
                    break;
                case 2:
                    devChipCount -= 1;
                    break;
                default:
                    break;
            }
        }
        else // Chip Deactivated
        {
            Debug.Log("'chip-active' 클래스가 없습니다.");
            chip.AddToClassList("chip-active");
            switch(category)
            {
                case 0:
                    researchChipCount += 1;
                    break;
                case 1:
                    designChipCount += 1;
                    break;
                case 2:
                    devChipCount += 1;
                    break;
                default:
                    break;
            }
        }
    }
}
