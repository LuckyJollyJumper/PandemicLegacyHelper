using UnityEngine;
using UnityEngine.UI;

public class PopUp : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TMPro.TMP_InputField CardNameInput;
    [SerializeField] private TMPro.TMP_InputField CardColourInput;
    
    [SerializeField] private TMPro.TMP_InputField CardAmountInput;
    
    void Start(){
        this.gameObject.SetActive(false);
    }
    public void OnSubmit(){
        string name = this.CardNameInput.text;
        string text = this.CardAmountInput.text;
        string colour = this.CardColourInput.text;
        
        // Submit here
        this.gameObject.SetActive(false);
    }
}
