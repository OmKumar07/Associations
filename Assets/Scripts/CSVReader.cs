using System.Collections.Generic;
using UnityEngine;

public class CSVReader : MonoBehaviour
{
    [System.Serializable]
    public class CategoryData
    {
        public string category;
        public List<string> words;
    }

    public List<CategoryData> allCategories = new List<CategoryData>();

    public char delimiter = ',';

    [Range(0, 1)] public float shuffleIntensity = 1f;

    public void LoadCSV(string fileName)
    {
        TextAsset csvData = Resources.Load<TextAsset>(fileName);
        allCategories.Clear();
        string[] lines = csvData.text.Split('\n');

        Dictionary<string, List<string>> categoryMap = new Dictionary<string, List<string>>();

        foreach (string line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            string[] splitLine = line.Split(delimiter);
            if (splitLine.Length < 2) continue;

            string category = splitLine[0].Trim();
            string word = splitLine[1].Trim();

            if (string.IsNullOrEmpty(category) || string.IsNullOrEmpty(word))
            {
                Debug.LogWarning("Skipping malformed line: " + line);
                continue;
            }

            if (!categoryMap.ContainsKey(category))
            {
                categoryMap[category] = new List<string>();
            }
            categoryMap[category].Add(word);
        }

        foreach (var entry in categoryMap)
        {
            CategoryData categoryData = new CategoryData
            {
                category = entry.Key,
                words = entry.Value
            };
            allCategories.Add(categoryData);
        }

        Debug.Log("CSV loaded successfully. Categories: " + allCategories.Count);
    }

    public Dictionary<string, string> GetWordsFromCategories(
    List<string> categories,
    int minWordsPerCategory,
    int maxWordsPerCategory,
    int totalWordCount)
    {
        Dictionary<string, string> wordCategoryMapping = new Dictionary<string, string>();
        System.Random random = new System.Random();

        List<CategoryData> validCategories = new List<CategoryData>();
        foreach (string category in categories)
        {
            CategoryData categoryData = allCategories.Find(c => c.category == category);
            if (categoryData != null && categoryData.words.Count > 0)
            {
                validCategories.Add(categoryData);
            }
            else
            {
                Debug.LogWarning($"Category '{category}' is empty or not found.");
            }
        }

        if (validCategories.Count == 0)
        {
            Debug.LogWarning("No valid categories available.");
            return wordCategoryMapping;
        }

        int remainingWords = totalWordCount;
        int categoriesCount = validCategories.Count;

        int baseWordsPerCategory = Mathf.Max(minWordsPerCategory, totalWordCount / categoriesCount);
        int[] categoryWordCounts = new int[validCategories.Count];

        for (int i = 0; i < validCategories.Count; i++)
        {
            int maxPossible = Mathf.Min(maxWordsPerCategory, validCategories[i].words.Count);
            categoryWordCounts[i] = Mathf.Min(baseWordsPerCategory, maxPossible);
            remainingWords -= categoryWordCounts[i];
        }

        while (remainingWords > 0)
        {
            bool distributed = false;
            for (int i = 0; i < validCategories.Count && remainingWords > 0; i++)
            {
                if (categoryWordCounts[i] < Mathf.Min(maxWordsPerCategory, validCategories[i].words.Count))
                {
                    categoryWordCounts[i]++;
                    remainingWords--;
                    distributed = true;
                }
            }
            if (!distributed) break;
        }

        List<KeyValuePair<string, string>> selectedWordList = new List<KeyValuePair<string, string>>();

        for (int i = 0; i < validCategories.Count; i++)
        {
            CategoryData categoryData = validCategories[i];

            List<string> shuffledWords = new List<string>(categoryData.words);
            for (int j = 0; j < shuffledWords.Count; j++)
            {
                int swapIndex = random.Next(j, shuffledWords.Count);
                (shuffledWords[j], shuffledWords[swapIndex]) = (shuffledWords[swapIndex], shuffledWords[j]);
            }

            for (int j = 0; j < categoryWordCounts[i]; j++)
            {
                string word = shuffledWords[j];
                selectedWordList.Add(new KeyValuePair<string, string>(word, categoryData.category));
            }
        }

        int shuffleCount = Mathf.CeilToInt(shuffleIntensity * selectedWordList.Count);
        for (int i = 0; i < shuffleCount; i++)
        {
            int indexA = random.Next(selectedWordList.Count);
            int indexB = random.Next(selectedWordList.Count);
            (selectedWordList[indexA], selectedWordList[indexB]) = (selectedWordList[indexB], selectedWordList[indexA]);
        }

        foreach (var pair in selectedWordList)
        {
            wordCategoryMapping.Add(pair.Key, pair.Value);
        }

        return wordCategoryMapping;
    }






    public Dictionary<string, string> GetRandomWords(int totalWords)
    {
        Dictionary<string, string> wordCategoryMapping = new Dictionary<string, string>();

        System.Random random = new System.Random();
        List<CategoryData> shuffledCategories = new List<CategoryData>(allCategories);

        if (shuffleIntensity > 0)
        {
            shuffledCategories.Sort((a, b) => random.Next(-1, 2));
        }

        foreach (CategoryData category in shuffledCategories)
        {
            if (wordCategoryMapping.Count >= totalWords) break;

            int remainingWords = totalWords - wordCategoryMapping.Count;

            List<string> shuffledWords = new List<string>(category.words);

            if (shuffleIntensity > 0)
            {
                shuffledWords.Sort((a, b) => random.Next(-1, 2));
            }

            for (int i = 0; i < Mathf.Min(remainingWords, shuffledWords.Count); i++)
            {
                string word = shuffledWords[i];
                wordCategoryMapping[word] = category.category;
            }
        }

        return wordCategoryMapping;
    }

    public void ClearData()
    {
        allCategories.Clear();
    }
}
