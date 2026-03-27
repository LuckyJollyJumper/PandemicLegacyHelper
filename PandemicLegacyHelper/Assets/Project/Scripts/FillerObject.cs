using UnityEngine;

/// <summary>
/// Ugly fix to remove the fillers when they are not needed anymore
/// </summary>
public class FillerObject : MonoBehaviour
{
    private int lastSiblingIndex = -1;

    private void Update(){
        int currentSiblingIndex = transform.GetSiblingIndex();
        if (currentSiblingIndex != lastSiblingIndex){
            lastSiblingIndex = currentSiblingIndex;
            CheckIfTopmost();
        }
    }

    private void CheckIfTopmost(){
        if (transform.parent != null && transform.GetSiblingIndex() == 0){
            Destroy(gameObject);
        }
    }
}
