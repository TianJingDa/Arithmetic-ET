using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class TextHelper
{
    public static string GetIndex(this Text self)
    {
        return self.GetComponent<TextIndex>()?.index ?? "";
    }
}
