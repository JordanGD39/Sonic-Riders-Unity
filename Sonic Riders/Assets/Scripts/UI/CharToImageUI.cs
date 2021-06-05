using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharToImageUI : MonoBehaviour
{
    [SerializeField] private Sprite[] lapNumbers;
    [SerializeField] private Sprite[] orangeOutlineNumbers;
    [SerializeField] private Sprite[] blueOutlineNumbers;
    [SerializeField] private Sprite[] levelNumbers;

    public enum numberType { LAP, ORANGEOUTLINE, BLUEOUTLINE ,LEVEL}

    public Sprite ConvertCharToSprite(int number, string digitsConversion, numberType type)
    {
        return ConvertCharsToSprite(number, digitsConversion, type)[0];
    }

    public Sprite[] ConvertCharsToSprite(int number, string digitsConversion, numberType type)
    {
        if (digitsConversion[0].ToString() != "0")
        {
            Debug.LogError("Invalid digits conversion!");

            return null;
        }

        Sprite[] numbers = lapNumbers;

        switch (type)
        {
            case numberType.ORANGEOUTLINE:
                numbers = orangeOutlineNumbers;
                break;
            case numberType.BLUEOUTLINE:
                numbers = blueOutlineNumbers;
                break;
            case numberType.LEVEL:
                numbers = levelNumbers;
                break;
        }

        string numberString = number.ToString(digitsConversion);

        char[] chars = numberString.ToCharArray();

        List<Sprite> returningNumbers = new List<Sprite>();

        for (int i = 0; i < chars.Length; i++)
        {
            returningNumbers.Add(numbers[(int)char.GetNumericValue(chars[i])]);
        }

        return returningNumbers.ToArray();
    }
}
