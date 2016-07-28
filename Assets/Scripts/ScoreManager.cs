using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScoreManager : MonoBehaviour
{
    Text score;
    public static int scoreValue;

    void Awake()
    {
        score = GetComponent<Text>();
    }

    void Update()
    {
        score.text = scoreValue.ToString();
    }
}
