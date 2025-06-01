public static class NumberFormatter
{
    public static string GetDisplay(int moneyAmount)
    {
        string formattedMoney;

        if (moneyAmount >= 1000000)
        {
            float moneyInM = moneyAmount / 1000000.0f;
            formattedMoney = moneyInM.ToString("0.00") + "m";
        }
        else if (moneyAmount >= 1000)
        {
            float moneyInK = moneyAmount / 1000.0f;
            formattedMoney = moneyInK.ToString("0.00") + "k";
        }
        else
        {
            formattedMoney = moneyAmount.ToString();
        }

        return formattedMoney;
    }
}
