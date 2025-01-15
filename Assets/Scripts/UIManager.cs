using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Transform gameBoard; // Parent object for rows
    public GameObject rowPrefab; // Prefab for a row with GridLayoutGroup
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
        winMessage.SetActive(false);
    }

    public void DisplayWords(Dictionary<string, string> wordCategoryMapping)
    {
        // Clear the game board
        foreach (Transform child in gameBoard)
        {
            Destroy(child.gameObject);
        }

        // Create 10 rows
        List<GameObject> rows = new List<GameObject>();
        for (int i = 0; i < 10; i++)
        {
            GameObject row = Instantiate(rowPrefab, gameBoard);
            rows.Add(row);
        }

        // Distribute words into rows (4 words per row)
        int index = 0;
        foreach (var pair in wordCategoryMapping)
        {
            string word = pair.Key;
            string category = pair.Value;

            GameObject button = Instantiate(wordButtonPrefab, rows[index / 4].transform);
            button.GetComponentInChildren<TextMeshProUGUI>().text = word;
            button.tag = category;

            // Add event triggers for drag detection
            EventTrigger trigger = button.AddComponent<EventTrigger>();
            AddEventTrigger(trigger, EventTriggerType.PointerDown, (data) => OnButtonPressed(button, word, category));
            AddEventTrigger(trigger, EventTriggerType.PointerUp, (data) => StartCoroutine(OnButtonReleased()));
            AddEventTrigger(trigger, EventTriggerType.PointerEnter, (data) => OnButtonHovered(button, word, category));

            index++;
        }

        categoryText.text = "Categories: " + string.Join(", ", new HashSet<string>(wordCategoryMapping.Values));

        // Start coroutine to disable grid layout after Unity updates the layout
        StartCoroutine(DisableGridLayouts(rows));
    }

    private IEnumerator DisableGridLayouts(List<GameObject> rows)
    {
        // Wait for the end of the current frame to ensure Unity calculates the layout
        yield return new WaitForEndOfFrame();

        foreach (var row in rows)
        {
            GridLayoutGroup gridLayout = row.GetComponent<GridLayoutGroup>();
            if (gridLayout != null)
            {
                gridLayout.enabled = false;
                Debug.Log($"GridLayout disabled for row: {row.name}");
            }
        }
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
                // Destroy all but the last button
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
        Debug.Log(gameBoard.childCount);
        if (gameBoard.childCount == 2)
        {
            ShowWinMessage();
        }
        yield return new WaitForSeconds(0.2f);
        CheckAndDestroyEmptyRows();
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
        if (button != null)
        {
            Button btn = button.GetComponent<Button>();

            if (btn != null)
            {
                if (btn.colors.normalColor != Color.cyan)
                {
                    ChangeButtonColor(button, Color.white);
                }
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

        // Iterate through all rows in the gameBoard
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

        // Identify and remove the last remaining button of a category
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
                            Debug.Log($"Disabling the last button for category: {pair.Key}");
                            button.gameObject.SetActive(false);

                            // Check if the row is now empty
                            if (row.childCount == 1) // Only the button being deactivated remains
                            {
                                Debug.Log($"Row {row.name} is now empty. Destroying row.");
                                Destroy(row.gameObject);
                            }
                            break;
                        }
                    }
                }
            }
        }
    }


    private void CheckAndDestroyEmptyRows()
    {
        foreach (Transform row in gameBoard)
        {
            if (row.childCount == 0)
            {
                Debug.Log($"Destroying empty row: {row.name}");
                Destroy(row.gameObject);
                Debug.Log("Row Deleted");
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
    public void ShowWinMessage()
    {
        Debug.Log("YouWin!");
        if (winMessage != null)
        {
            winMessage.SetActive(true);
            Debug.Log("YouWin!");
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
