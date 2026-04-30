using UnityEngine;

/// <summary>
/// Class that handles and represents a month.
/// </summary>
public class Month : MonoBehaviour
{
    public enum Months {January, February, March, April, May, June, July, August, September, October, November, December};
    [SerializeField] private Months currentMonth;
    public Months CurrentMonth => currentMonth;

    [SerializeField] private TMPro.TMP_Dropdown monthText;

    public void Start(){ 
        this.monthText = GetComponent<TMPro.TMP_Dropdown>(); 
        SetMonth(currentMonth);
    }
    public void NextMonth(){
        if (currentMonth == Months.December){ return; } 
        else { currentMonth++; }
        SetMonth(currentMonth);
    }
    public void SetMonth(Months month){
        currentMonth = month;
        monthText.value = (int)month;
        monthText.RefreshShownValue();
        // monthText.text = currentMonth.ToString();
    }

    public void SetMonth(){
        currentMonth = (Months)monthText.value;
        monthText.RefreshShownValue();
    }
}
