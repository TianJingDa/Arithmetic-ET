using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public static class ToggleHelper
{
    public static int GetIndex(this Toggle self)
    {
        return self.GetComponent<ToggleIndex>()?.index ?? -1;
    }
}
