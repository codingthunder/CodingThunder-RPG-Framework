using CodingThunder.DatingSim;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class TimeDisplay : MonoBehaviour
{
    public static TimeDisplay Instance;

    public TimeKeeper timeKeeper;

    public TextMeshProUGUI timeText;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
        }

        if (timeKeeper == null)
        {
            Destroy(this);
        }

        if (timeText == null)
        {
            Destroy(this);
        }

        Instance = this;

        DontDestroyOnLoad(gameObject);


    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //if (!timeKeeper.IsActive)
        //{
        //    timeText.gameObject.SetActive(false);
        //    return;
        //}

        //if (!timeText.gameObject.activeSelf)
        //{
        //    timeText.gameObject.SetActive(true);
        //}

        // I could use DayTime library, but lazy at the moment.
        string hourString = timeKeeper.hour.ToString("D2");
        string dayString = timeKeeper.day.ToString("D2");
        string monthString = timeKeeper.month.ToString();

        var fullString = $"Month {monthString}, Day {dayString}, Hour {hourString}";

        timeText.text = fullString;
    }

}
