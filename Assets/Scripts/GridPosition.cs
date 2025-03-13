using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GridPosition : MonoBehaviour
{
    public int rows;
    public int cols;
    private GameObject[,] buttonGrid;

    public void InitializeGrid(List<GameObject> buttons)
    {
        buttonGrid = new GameObject[rows, cols];

        for (int i = 0; i < buttons.Count; i++)
        {
            int row = i / cols;
            int col = i % cols;
            buttonGrid[row, col] = buttons[i];
        }
    }

    public List<GameObject> GetValidNeighbors(int row, int col)
    {
        List<GameObject> neighbors = new List<GameObject>();
        int[] dRow = { -1, -1, -1, 0, 0, 1, 1, 1 };
        int[] dCol = { -1, 0, 1, -1, 1, -1, 0, 1 };

        for (int i = 0; i < 8; i++)
        {
            int newRow = row + dRow[i];
            int newCol = col + dCol[i];

            if (newRow >= 0 && newRow < rows && newCol >= 0 && newCol < cols)
            {
                neighbors.Add(buttonGrid[newRow, newCol]);
            }
        }
        return neighbors;
    }

    public void DebugLogNeighbors(int row, int col)
    {
        List<GameObject> neighbors = GetValidNeighbors(row, col);
        Debug.Log($"Button at ({row}, {col}) Neighbors:");

        foreach (GameObject neighbor in neighbors)
        {
            TextMeshProUGUI text = neighbor.GetComponentInChildren<TextMeshProUGUI>();
            string category = neighbor.tag;
            Debug.Log($"Name: {text.text}, Category: {category}");
        }
    }
}
