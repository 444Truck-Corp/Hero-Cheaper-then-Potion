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

    public static string ChangeToStars(int number)
    {
        string stars = "";

        for (int i = 0; i < number / 2; i++) stars += "★";
        if (number % 2 == 1) stars += "☆";

        return stars;
    }
}