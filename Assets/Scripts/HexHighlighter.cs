using System.Collections.Generic;
using UnityEngine;
using static Hex;

public class HexHighlighter : MonoBehaviour {
    private const string HEX_HOVER_IDENTIFIER = "Selected";
    private static GameObject previous;
    private static Hex previousRangeMainHex;
    private static Dictionary<Hex, GameObject> previousHexMap;
    private static List<Hex> previousHexesInRange;


    public static void HighlightHex(GameObject hexObject) {
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

    public static void HighlightMultipleHexes(Hex mainHex, Dictionary<Hex, GameObject> hexMap, List<Hex> hexesInRange) {
        if (previousRangeMainHex is not null && mainHex == previousRangeMainHex) {
            return;
        }
        UnhighlightHexesRange();
        hexMap.TryGetValue(mainHex, out GameObject mainHexObject);
        previousRangeMainHex = mainHex;
        previousHexMap = hexMap;
        previousHexesInRange = hexesInRange;
    
        mainHexObject?.transform.Find(HEX_HOVER_IDENTIFIER).gameObject.SetActive(true);
        foreach (var hexInRange in hexesInRange) {
            hexMap.TryGetValue(hexInRange, out GameObject inRangeHexObject);
            inRangeHexObject?.transform.Find(HEX_HOVER_IDENTIFIER).gameObject.SetActive(true);
        }
    }
    
    public static void UnhighlightHexesRange() {
        if (previousHexesInRange is null) return;
    
        // previ?.transform.Find(HEX_HOVER_IDENTIFIER).gameObject.SetActive(true);
        foreach (var hexInRange in previousHexesInRange) {
            previousHexMap.TryGetValue(hexInRange, out GameObject inRangeHexObject);
            inRangeHexObject?.transform.Find(HEX_HOVER_IDENTIFIER).gameObject.SetActive(false);
        }
    
        previousRangeMainHex = null;
        previousHexMap = null;
        previousHexesInRange = null;
    }
}