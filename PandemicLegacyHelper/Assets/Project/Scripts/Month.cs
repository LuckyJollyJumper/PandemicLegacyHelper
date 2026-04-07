using UnityEngine;

public class Month : MonoBehaviour
{
    public enum Months {January, February, March, April, May, June, July, August, September, October, November, December};
    public Months currentMonth;

    [SerializeField] private TMPro.TextMeshProUGUI monthText;

    public void Start(){ 
        this.monthText = GetComponent<TMPro.TextMeshProUGUI>(); 
        SetMonth(currentMonth);
    }
    public void NextMonth(){
        if (currentMonth == Months.December){
            currentMonth = Months.January;
        } else {
            currentMonth++;
        }
        monthText.text = currentMonth.ToString();
    }
    public void SetMonth(Months month){
        currentMonth = month;
        monthText.text = currentMonth.ToString();
    }
}
