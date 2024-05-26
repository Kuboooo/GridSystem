using UnityEngine;

public class HexHighlighter : MonoBehaviour {

    private const string HEX_HOVER_IDENTIFIER = "Selected";
    private GameObject previous;

    public void HighlightHex(GameObject hexObject) {
        if (previous != null && previous.transform.Find(HEX_HOVER_IDENTIFIER) != null) {
            previous.transform.Find(HEX_HOVER_IDENTIFIER).gameObject.SetActive(false);
        }

        if (hexObject != null && hexObject.transform.Find(HEX_HOVER_IDENTIFIER) != null) {
            hexObject.transform.Find(HEX_HOVER_IDENTIFIER).gameObject.SetActive(true);
            previous = hexObject;
        }
        else {
            previous = null;
        }
    }

}