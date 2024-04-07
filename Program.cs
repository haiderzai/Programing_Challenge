using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

public class ChildspeakChallenge
{
    private static readonly string Vowels = "aeiouy";
    private static readonly string Consonants = "bcdfghjklmnpqrstvwxyz";

    public static void Main(string[] args)
    {
        string inputFilePath = "input.txt";
        string outputFilePath = "output.txt";

        if (!File.Exists(inputFilePath))
        {
            Console.WriteLine("Input file not found.");
            return;
        }

        List<string> words = File.ReadAllLines(inputFilePath).Distinct().ToList();
        Dictionary<string, int> pronunciationCounts = CountPronunciations(words);
        GenerateOutput(pronunciationCounts, outputFilePath);

        Console.WriteLine("Output has been written to output.txt");
    }

    private static Dictionary<string, int> CountPronunciations(List<string> words)
    {
        var transformedWords = words.Select(TransformToChildspeak).ToList();

        var pronunciationCounts = transformedWords.GroupBy(x => x)
            .ToDictionary(group => group.Key, group => group.Count());

        return words.Distinct().ToDictionary(word => word, word => pronunciationCounts[TransformToChildspeak(word)] - 1);
    }

    private static string TransformToChildspeak(string word)
    {
        word = UseUniqueConsonant(word);
        word = PrependConsonantForVowelStart(word);
        word = ReplaceConsecutiveConsonants(word);
        word = ReplaceConsecutiveVowels(word);
        word = IgnoreConsonantsAfterLastVowel(word);
        return word;
    }

    private static string UseUniqueConsonant(string word)
    {
        var firstConsonant = word.FirstOrDefault(c => Consonants.Contains(c));
        if (firstConsonant == default) return word;

        return string.Join("", word.Select(c => Consonants.Contains(c) ? firstConsonant : c));
    }

    private static string PrependConsonantForVowelStart(string word)
    {
        if (Vowels.Contains(word[0]))
        {
            var firstConsonant = word.FirstOrDefault(c => Consonants.Contains(c));
            if (firstConsonant != default)
            {
                int index = word.IndexOf(firstConsonant);
                word = word.Remove(index, 1);
                word = firstConsonant + word;
            }
        }
        return word;
    }

    private static string ReplaceConsecutiveConsonants(string word)
    {
        foreach (char consonant in Consonants)
        {
            string pattern = $"{consonant}+";
            word = Regex.Replace(word, pattern, consonant.ToString());
        }
        return word;
    }

    private static string ReplaceConsecutiveVowels(string word)
    {
        string pattern = $"[{Vowels}]+";
        return Regex.Replace(word, pattern, match => match.Value.Last().ToString());
    }

    private static string IgnoreConsonantsAfterLastVowel(string word)
    {
        int lastVowelIndex = word.LastIndexOfAny(Vowels.ToCharArray());
        if (lastVowelIndex != -1)
        {
            return word.Substring(0, lastVowelIndex + 1);
        }
        return word;
    }

    private static void GenerateOutput(Dictionary<string, int> pronunciationCounts, string outputFilePath)
    {
        var sortedLines = pronunciationCounts.OrderBy(kvp => kvp.Key)
            .Select(kvp => $"{kvp.Key} {kvp.Value}");

        File.WriteAllLines(outputFilePath, sortedLines);
    }
}
