using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public CSVReader csvReader;
    public UIManager uiManager;
    public int totalWords = 40;
    public int currentLevel = 1;
    public List<List<string>> levelCategories = new List<List<string>>();

    [Range(1, 10)] public int minWordsPerCategory = 4;
    [Range(1, 10)] public int maxWordsPerCategory = 8;

    void Start()
    {
        Application.runInBackground = true;
        Application.targetFrameRate = 60;
        csvReader.LoadCSV("csvdata");

        levelCategories.Add(new List<string> { "Numbers", "Colors","Animals", "Body Parts", "Fruits", "Birds", "Countries", "Family" }); // Level 1
        levelCategories.Add(new List<string> { "Birds", "Weather", "Sports", "Languages", "Foods", "Occupation", "Stationery","Beverages" }); // Level 2
        levelCategories.Add(new List<string> { "Dances", "Months", "Currencies", "Greek Letters", "Planets", "Weapons","Chemistry", "Furniture" }); // Level 3
        levelCategories.Add(new List<string> { "Fish","Boy","Birds","Accessories","Ocean Life","Shapes", "Zodiac Sign","Emotions" }); // Level 4
        levelCategories.Add(new List<string> { "Astronomy","Weather","Superheroes","City","Flowers","Music Genre","Dogs","Marriage" }); // Level 5

        StartCoroutine(LoadLevel(currentLevel));
    }

    IEnumerator LoadLevel(int level)
    {
        if (level <= levelCategories.Count)
        {
            List<string> categoriesForLevel = levelCategories[level - 1];

            Dictionary<string, string> wordCategoryMapping = csvReader.GetWordsFromCategories(
                categoriesForLevel,
                minWordsPerCategory,
                maxWordsPerCategory,
                totalWords
            );

            uiManager.DisplayWords(wordCategoryMapping);
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            Debug.LogWarning("No more levels available.");
        }
    }
}
