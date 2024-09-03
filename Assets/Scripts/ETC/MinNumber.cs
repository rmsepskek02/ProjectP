using UnityEngine;

public class MinNumber : MonoBehaviour
{
    int[] number = { -2, -5, -3, -7, -1 };
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(minNumber(number));
    }

    int minNumber(int [] input)
    {
        int _min = Mathf.Min(input);
        
        return _min;
    }
}
