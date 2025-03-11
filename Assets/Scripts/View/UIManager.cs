using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Transform gameBoard;
    public GameObject rowPrefab;
    public GameObject wordButtonPrefab;
    public GameObject winMessage;
    public TextMeshProUGUI currentLevel;
    public Transform categoryContainer;
    public GameObject categoryPrefab;
    public GameManager gameManager;

    // Boundary Buttons
    public Button topBoundaryButton;
    public Button bottomBoundaryButton;
    public Button leftBoundaryButton;
    public Button rightBoundaryButton;




    //colors
    public Color customGreen = new Color(0.35f, 0.8f, 0.16f, 0.78f);
    public Color customRed = new Color(0.8f, 0.1607f, 0.266f, 0.78f);
    public Color customYellow = new Color(0.8f, 0.647f, 0.1607f, 0.78f);



    private LineRenderer lineRenderer;


    
    public DragController dragController;
    


    public class ButtonCategory : MonoBehaviour
    {
        public string category;
    }
    private void Start()
    {

        currentLevel.text = "Level " + gameManager.currentLevel.ToString();
        winMessage.SetActive(false);

        AddBoundaryButtonListeners();

        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = customGreen;
        lineRenderer.endColor = customGreen;
        lineRenderer.useWorldSpace = false;


    }
    public void UpdateLineRenderer()
    {
        if (dragController.draggedButtons.Count > 1)
        {
            lineRenderer.positionCount = dragController.draggedButtons.Count;

            for (int i = 0; i < dragController.draggedButtons.Count; i++)
            {

                Vector3 worldPosition = dragController.draggedButtons[i].transform.position;


                Vector3 screenPosition = Camera.main.WorldToScreenPoint(worldPosition);


                RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    gameBoard.GetComponent<RectTransform>(),
                    screenPosition,
                    Camera.main,
                    out Vector2 localPosition
                );


                Vector3 correctedPosition = new Vector3(localPosition.x, localPosition.y, 0);


                lineRenderer.SetPosition(i, correctedPosition);
            }
        }
        else
        {
            lineRenderer.positionCount = 0;
        }
    }

    private void AddBoundaryButtonListeners()
    {
        AddBoundaryTrigger(topBoundaryButton, dragController.StopDragging);
        AddBoundaryTrigger(bottomBoundaryButton, dragController.StopDragging);
        AddBoundaryTrigger(leftBoundaryButton, dragController.StopDragging);
        AddBoundaryTrigger(rightBoundaryButton, dragController.StopDragging);
    }

    private void AddBoundaryTrigger(Button boundaryButton, System.Action action)
    {
        EventTrigger trigger = boundaryButton.gameObject.AddComponent<EventTrigger>();

        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerEnter
        };
        entry.callback.AddListener((data) => action.Invoke());
        trigger.triggers.Add(entry);
    }

    public void DisplayWords(Dictionary<string, string> wordCategoryMapping)
    {
        DisplayCategories(new HashSet<string>(wordCategoryMapping.Values));

        List<GameObject> rows = new List<GameObject>();
        for (int i = 0; i < 10; i++)
        {
            GameObject row = Instantiate(rowPrefab, gameBoard);
            rows.Add(row);
        }

        int index = 0;
        foreach (var pair in wordCategoryMapping)
        {
            string word = pair.Key;
            string category = pair.Value;

            GameObject button = Instantiate(wordButtonPrefab, rows[index / 4].transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = word;
            button.tag = category;
            ButtonCategory buttonCategory = button.AddComponent<ButtonCategory>();
            buttonCategory.category = category;

            EventTrigger trigger = button.AddComponent<EventTrigger>();
            AddEventTrigger(trigger, EventTriggerType.PointerDown, (data) => dragController.OnButtonPressed(button, word, category));
            AddEventTrigger(trigger, EventTriggerType.PointerUp, (data) => StartCoroutine(dragController.OnButtonReleased()));
            AddEventTrigger(trigger, EventTriggerType.PointerEnter, (data) => dragController.OnButtonHovered(button, word, category));

            index++;
        }

        StartCoroutine(DisableGridLayouts(rows));
    }

    private void DisplayCategories(HashSet<string> categories)
    {
        Vector2[] fixedPositions = new Vector2[]
        {
            new Vector2(-300f, 80f),
            new Vector2(0f, 80f),
            new Vector2(300f, 80f),
            new Vector2(-145f, 1f),
            new Vector2(155f, 1f),
            new Vector2(-289f, -78f),
            new Vector2(11f, -78f),
            new Vector2(311f, -78f)
        };

        int index = 0;

        foreach (string category in categories)
        {
            if (index >= fixedPositions.Length) break;

            GameObject categoryItem = Instantiate(categoryPrefab, categoryContainer);
            categoryItem.transform.localPosition = fixedPositions[index];

            Image icon = categoryItem.transform.Find("Icon").GetComponent<Image>();
            TextMeshProUGUI text = categoryItem.transform.Find("Text").GetComponent<TextMeshProUGUI>();

            icon.sprite = GetCategoryIcon(category);
            text.text = category;

            index++;
        }
    }

    public Sprite GetCategoryIcon(string category)
    {
        switch (category)
        {
            // Level 1 Categories
            case "Numbers":
                return Resources.Load<Sprite>("Icons/numbers");
            case "Colors":
                return Resources.Load<Sprite>("Icons/colors");
            case "Animals":
                return Resources.Load<Sprite>("Icons/animals");
            case "Body Parts":
                return Resources.Load<Sprite>("Icons/body_parts");
            case "Fruits":
                return Resources.Load<Sprite>("Icons/fruits");
            case "Birds":
                return Resources.Load<Sprite>("Icons/birds");
            case "Countries":
                return Resources.Load<Sprite>("Icons/countries");
            case "Family":
                return Resources.Load<Sprite>("Icons/family");

            // Level 2 Categories
            case "Weather":
                return Resources.Load<Sprite>("Icons/weather");
            case "Sports":
                return Resources.Load<Sprite>("Icons/sports");
            case "Languages":
                return Resources.Load<Sprite>("Icons/languages");
            case "Foods":
                return Resources.Load<Sprite>("Icons/foods");
            case "Occupation":
                return Resources.Load<Sprite>("Icons/occupation");
            case "Stationery":
                return Resources.Load<Sprite>("Icons/stationery");
            case "Beverages":
                return Resources.Load<Sprite>("Icons/beverages");

            // Level 3 Categories
            case "Dances":
                return Resources.Load<Sprite>("Icons/dances");
            case "Months":
                return Resources.Load<Sprite>("Icons/months");
            case "Currencies":
                return Resources.Load<Sprite>("Icons/currencies");
            case "Greek Letters":
                return Resources.Load<Sprite>("Icons/greek_letters");
            case "Planets":
                return Resources.Load<Sprite>("Icons/planets");
            case "Weapons":
                return Resources.Load<Sprite>("Icons/weapons");
            case "Chemistry":
                return Resources.Load<Sprite>("Icons/chemistry");
            case "Furniture":
                return Resources.Load<Sprite>("Icons/furniture");

            // Level 4 Categories
            case "Fish":
                return Resources.Load<Sprite>("Icons/fish");
            case "Boy":
                return Resources.Load<Sprite>("Icons/boy");
            case "Accessories":
                return Resources.Load<Sprite>("Icons/accessories");
            case "Ocean Life":
                return Resources.Load<Sprite>("Icons/ocean_life");
            case "Shapes":
                return Resources.Load<Sprite>("Icons/shapes");
            case "Zodiac Sign":
                return Resources.Load<Sprite>("Icons/zodiac_sign");
            case "Emotions":
                return Resources.Load<Sprite>("Icons/emotions");

            // Level 5 Categories
            case "Astronomy":
                return Resources.Load<Sprite>("Icons/astronomy");
            case "Superheroes":
                return Resources.Load<Sprite>("Icons/superheroes");
            case "City":
                return Resources.Load<Sprite>("Icons/city");
            case "Flowers":
                return Resources.Load<Sprite>("Icons/flowers");
            case "Music Genre":
                return Resources.Load<Sprite>("Icons/music_genre");
            case "Dogs":
                return Resources.Load<Sprite>("Icons/dogs");
            case "Marriage":
                return Resources.Load<Sprite>("Icons/marriage");

            // Default Category
            default:
                return Resources.Load<Sprite>("Icons/default");
        }
    }

    private IEnumerator DisableGridLayouts(List<GameObject> rows)
    {
        yield return new WaitForEndOfFrame();

        foreach (var row in rows)
        {
            GridLayoutGroup gridLayout = row.GetComponent<GridLayoutGroup>();
            if (gridLayout != null)
            {
                gridLayout.enabled = false;
            }
        }
    }

    private void AddEventTrigger(EventTrigger trigger, EventTriggerType eventType, System.Action<BaseEventData> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = eventType
        };
        entry.callback.AddListener((data) => action.Invoke(data));
        trigger.triggers.Add(entry);
    }

    public void ChangeButtonColor(GameObject button, Color color)
    {
        Button btn = button.GetComponent<Button>();
        if (btn != null)
        {
            ColorBlock colorBlock = btn.colors;
            colorBlock.normalColor = color;
            colorBlock.highlightedColor = color;
            colorBlock.pressedColor = color;
            colorBlock.selectedColor = color;
            colorBlock.disabledColor = Color.gray;
            btn.colors = colorBlock;
        }
    }

    public void CheckAndDestroyLastButton()
    {
        Dictionary<string, int> categoryCounts = new Dictionary<string, int>();

        foreach (Transform row in gameBoard)
        {
            foreach (Transform button in row)
            {
                string category = button.tag;

                if (!categoryCounts.ContainsKey(category))
                {
                    categoryCounts[category] = 0;
                }
                categoryCounts[category]++;
            }
        }

        foreach (KeyValuePair<string, int> pair in categoryCounts)
        {
            if (pair.Value == 1)
            {
                foreach (Transform row in gameBoard)
                {
                    foreach (Transform button in row)
                    {
                        if (button.tag == pair.Key)
                        {
                            button.gameObject.SetActive(false);

                            if (row.childCount == 1)
                            {
                                Destroy(row.gameObject);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }

    public void CheckAndDestroyEmptyRows()
    {
        foreach (Transform row in gameBoard)
        {
            if (row.childCount == 0)
            {
                Destroy(row.gameObject);
            }
        }
    }

    public void ShowWinMessage()
    {
        if (winMessage != null)
        {
            winMessage.SetActive(true);
        }
    }

    public void NextLevel()
    {
        if (gameManager.currentLevel < 5)
        {
            SceneManager.LoadScene(gameManager.currentLevel);
        }
    }
}
