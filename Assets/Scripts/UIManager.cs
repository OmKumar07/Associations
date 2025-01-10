using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Transform gameBoard;
    public GameObject wordButtonPrefab;
    public GameObject winMessage;
    public TextMeshProUGUI currentLevel;
    public TextMeshProUGUI categoryText;
    public GameManager gameManager;

    private bool isDragging = false;
    private List<GameObject> draggedButtons = new List<GameObject>();
    private string dragCategory = null;
    private bool isMixedCategory = false;

    private void Start()
    {
        currentLevel.text = "Level " + gameManager.currentLevel.ToString();
    }
    public void DisplayWords(Dictionary<string, string> wordCategoryMapping)
    {
        foreach (Transform child in gameBoard)
        {
            Destroy(child.gameObject);
        }

        foreach (var pair in wordCategoryMapping)
        {
            string word = pair.Key;
            string category = pair.Value;

            GameObject button = Instantiate(wordButtonPrefab, gameBoard);

            button.GetComponentInChildren<TextMeshProUGUI>().text = word;
            button.tag = category;

            EventTrigger trigger = button.AddComponent<EventTrigger>();
            AddEventTrigger(trigger, EventTriggerType.PointerDown, (data) => OnButtonPressed(button, word, category));
            AddEventTrigger(trigger, EventTriggerType.PointerUp, (data) => StartCoroutine(OnButtonReleased()));
            AddEventTrigger(trigger, EventTriggerType.PointerEnter, (data) => OnButtonHovered(button, word, category));
        }

        categoryText.text = "Categories: " + string.Join(", ", new HashSet<string>(wordCategoryMapping.Values));
    }

    private void OnButtonPressed(GameObject button, string word, string category)
    {
        isDragging = true;
        dragCategory = category;
        isMixedCategory = false;
        draggedButtons.Clear();

        OnButtonHovered(button, word, category);
    }

    private IEnumerator OnButtonReleased()
    {
        isDragging = false;

        if (isMixedCategory)
        {
            Debug.Log("Lost one life! Dragged over multiple categories.");
            foreach (GameObject button in draggedButtons)
            {
                ChangeButtonColor(button, Color.red);
            }
            yield return new WaitForSeconds(2);

            foreach (GameObject button in draggedButtons)
            {
                ChangeButtonColor(button, Color.white);
            }
        }
        else if (draggedButtons.Count >= 2)
        {
            Debug.Log($"Updating last button to display category: {dragCategory}");

            for (int i = 0; i < draggedButtons.Count - 1; i++)
            {
                Destroy(draggedButtons[i]);
            }

            if (draggedButtons.Count > 0)
            {
                GameObject lastButton = draggedButtons[draggedButtons.Count - 1];
                TextMeshProUGUI buttonText = lastButton.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    buttonText.text = dragCategory;
                    ChangeButtonColor(lastButton, Color.cyan);
                }
            }
        }

        draggedButtons.Clear();
        dragCategory = null;

        CheckAndDestroyLastButton();
    }

    private void OnButtonHovered(GameObject button, string word, string category)
    {
        if (isDragging && !draggedButtons.Contains(button))
        {
            Debug.Log($"Dragged over word: {word} of Category: {category}");

            if (dragCategory != category)
            {
                isMixedCategory = true;
            }

            ChangeButtonColor(button, Color.green);
            StartCoroutine(ResetButtonColor(button, 2f));
            draggedButtons.Add(button);
        }
    }

    private IEnumerator ResetButtonColor(GameObject button, float delay)
    {
        yield return new WaitForSeconds(delay);

        Button btn = button.GetComponent<Button>();
        if (btn != null)
        {
            if (btn.colors.normalColor != Color.cyan)
            {
                ChangeButtonColor(button, Color.white);
            }
        }
    }

    private void ChangeButtonColor(GameObject button, Color color)
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

    private void CheckAndDestroyLastButton()
    {
        Dictionary<string, int> categoryCounts = new Dictionary<string, int>();

        foreach (Transform child in gameBoard)
        {
            string category = child.tag;

            if (!categoryCounts.ContainsKey(category))
            {
                categoryCounts[category] = 0;
            }
            categoryCounts[category]++;
        }

        foreach (KeyValuePair<string, int> pair in categoryCounts)
        {
            if (pair.Value == 1)
            {
                foreach (Transform child in gameBoard)
                {
                    if (child.tag == pair.Key)
                    {
                        Debug.Log($"Destroying the last button for category: {pair.Key}");
                        Destroy(child.gameObject);
                        break;
                    }
                }
            }
        }

        if (gameBoard.childCount <= 1)
        {
            ShowWinMessage();
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
        else
        {
            Debug.Log("All Levels Complete");
        }
    }
}
