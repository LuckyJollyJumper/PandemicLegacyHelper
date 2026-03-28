using UnityEngine;

/// <summary>
/// Ugly fix to remove the fillers when they are not needed anymore
/// </summary>
public class KnownDeckObject : DeckObject
{
    [SerializeField] GameObject dividerObject;

    /// <summary>
    /// Used for hiding the divider if the gameObject is empty
    /// </summary>
    private void OnTransformParentChanged(){
        if(dividerObject.transform.GetSiblingIndex() == 0){
            Destroy(this);
        }
    }
}
