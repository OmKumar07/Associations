using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DragController : MonoBehaviour
{
    public UIManager uiManager;
    private bool isDragging = false;
    private string dragCategory = null;
    private bool isMixedCategory = false;
    [HideInInspector]
    public List<GameObject> draggedButtons = new List<GameObject>();

    private int life = 3;

    

    private Dictionary<string, int> categoryButtonCounts = new Dictionary<string, int>();


    private void Start()
    {
        InitializeCategoryCounts();
    }
    public void StopDragging()
    {
        if (isDragging)
        {
            isDragging = false;
            Debug.Log("Dragging stopped: Entered boundary button");
            foreach (GameObject button in draggedButtons)
            {
                uiManager.ChangeButtonColor(button, Color.white);
            }

            draggedButtons.Clear();
            dragCategory = null;
        }
    }
    public void OnButtonPressed(GameObject button, string word, string category)
    {
        isDragging = true;
        dragCategory = category;
        isMixedCategory = false;
        draggedButtons.Clear();

        OnButtonHovered(button, word, category);
    }
    private void InitializeCategoryCounts()
    {
        categoryButtonCounts.Clear();

        foreach (Transform row in uiManager.gameBoard)
        {
            foreach (Transform button in row)
            {
                string buttonCategory = button.tag;
                if (!categoryButtonCounts.ContainsKey(buttonCategory))
                {
                    categoryButtonCounts[buttonCategory] = 0;
                }
                categoryButtonCounts[buttonCategory]++;
            }
        }
    }
    public void OnButtonHovered(GameObject button, string word, string category)
    {
        if (isDragging && !draggedButtons.Contains(button))
        {
            draggedButtons.Add(button);
            uiManager.ChangeButtonColor(button, uiManager.customGreen);

            if (dragCategory != category)
            {
                isMixedCategory = true;
            }

            uiManager.UpdateLineRenderer();
        }
    }
    public IEnumerator OnButtonReleased()
    {
        isDragging = false;

        if (draggedButtons.Count < 2)
        {
            draggedButtons.Clear();
            dragCategory = null;
            yield break;
        }
        bool allSameCategory = true;



        foreach (GameObject button in draggedButtons)
        {
            if (button.tag != dragCategory)
            {
                allSameCategory = false;
                break;
            }
        }

        if (allSameCategory)
        {
            int combinedButtons = 0;
            foreach (GameObject button in draggedButtons)
            {
                TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();
                if (buttonText != null)
                {
                    string[] parts = buttonText.text.Split('/');
                    if (parts.Length == 2 && int.TryParse(parts[0], out int parsedCount))
                    {
                        combinedButtons += parsedCount;
                    }
                    else
                    {
                        combinedButtons += 1;
                    }
                }
            }
            int storedCategoryButtons = categoryButtonCounts.ContainsKey(dragCategory) ? categoryButtonCounts[dragCategory] : combinedButtons;

            GameObject lastButton = draggedButtons[draggedButtons.Count - 1];
            TextMeshProUGUI lastButtonText = lastButton.GetComponentInChildren<TextMeshProUGUI>();
            if (lastButtonText != null)
            {
                lastButtonText.text = $"{combinedButtons}/{storedCategoryButtons}";
            }

            uiManager.ChangeButtonColor(lastButton, uiManager.customYellow);
            lastButton.transform.GetChild(1).GetComponent<Image>().sprite = uiManager.GetCategoryIcon(dragCategory);
            lastButton.transform.GetChild(1).GetComponent<Image>().color = uiManager.customRed;

            foreach (GameObject button in draggedButtons)
            {
                if (button != lastButton)
                {
                    Destroy(button);
                }
            }

            draggedButtons.Clear();
            draggedButtons.Add(lastButton);
        }
        else
        {
            life--;
            foreach (GameObject button in draggedButtons)
            {
                uiManager.ChangeButtonColor(button, uiManager.customRed);
            }
            yield return new WaitForSeconds(2);
            foreach (GameObject button in draggedButtons)
            {
                uiManager.ChangeButtonColor(button, Color.white);
            }
        }

        draggedButtons.Clear();
        dragCategory = null;

        if (uiManager.gameBoard.childCount == 2)
        {
            uiManager.ShowWinMessage();
        }

        yield return new WaitForSeconds(0.01f);
        uiManager.UpdateLineRenderer();
        yield return new WaitForSeconds(0.2f);
        uiManager.CheckAndDestroyEmptyRows();
        uiManager.CheckAndDestroyLastButton();
    }
}
