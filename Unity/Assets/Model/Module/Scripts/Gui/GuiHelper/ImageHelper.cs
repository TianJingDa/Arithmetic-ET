using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class ImageHelper
{
    public static string GetIndex(this Image self)
    {
        return self.GetComponent<ImageIndex>()?.index ?? "";
    }
}
