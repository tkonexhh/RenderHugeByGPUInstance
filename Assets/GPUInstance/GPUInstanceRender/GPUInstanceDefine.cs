using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GFrame.GPUInstance
{

    public class GPUInstanceDefine
    {
        public static int MAX_CAPACITY = 1023;


    }

    public delegate void OnCellCreate(GPUInstanceCell cell);
    public delegate void OnCellDraw(MaterialPropertyBlock mpb);
}