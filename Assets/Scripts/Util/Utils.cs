using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class Utils
{
    public static string ChangeToRomanNumeral(int number)
    {
        string[] thousands = { "", "M", "MM", "MMM" };
        string[] hundreds = { "", "C", "CC", "CCC", "CD", "D", "DC", "DCC", "DCCC", "CM" };
        string[] tens = { "", "X", "XX", "XXX", "XL", "L", "LX", "LXX", "LXXX", "XC" };
        string[] ones = { "", "I", "II", "III", "IV", "V", "VI", "VII", "VIII", "IX" };
        return thousands[number / 1000] + hundreds[(number % 1000) / 100] + tens[(number % 100) / 10] + ones[number % 10];
    }

    private const char filledStar = '★';
    private const char blankStar = '☆';
    public static string ChangeToStars(int number)
    {
        string stars = "";

        for (int i = 0; i < number / 2; i++) stars += filledStar;
        if (number % 2 == 1) stars += blankStar;

        return stars;
    }

    private const char filledHp = '■';
    private const char blankHp = '□';
    private const int hpSlot = 10;
    public static string BuildHpBar(HeroData hero)
    {
        if (hero.maxHP <= 0) return new string(blankHp, hpSlot);

        int filled = Mathf.RoundToInt((float)hero.curHP / hero.maxHP * hpSlot);
        filled = Mathf.Clamp(filled, 0, hpSlot);

        return new string(filledHp, filled) + new string(blankHp, hpSlot - filled);
    }

    public static void RefreshHorizontalRow(Transform parentHLG, TextMeshProUGUI changedText)
    {
        changedText.ForceMeshUpdate();
        LayoutRebuilder.ForceRebuildLayoutImmediate(changedText.rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate((RectTransform)parentHLG);
        Canvas.ForceUpdateCanvases();
    }
}