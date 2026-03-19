using UnityEngine;
using UnityEngine.UI;

public class CardPrefab : MonoBehaviour
{
    private CardData Data;
    [SerializeField] private TMPro.TextMeshProUGUI CardNameText;
    [SerializeField] private TMPro.TextMeshProUGUI CardPercentageText;
    [SerializeField] private TMPro.TextMeshProUGUI CardAmountText;
    [SerializeField] private Image backgroundImage;
    
    public void SetCardPrefab(CardData c, float percentage, int amount){
        this.Data = c;
        this.CardNameText.text = Data.CardName;
        this.CardPercentageText.text = $"{percentage}";
        this.CardAmountText.text = $"{amount}";
        this.backgroundImage.color = Color.white;
    }

}
