﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GFrame.GPUInstance;
public class FootmanGroup : GPUInstanceGroup
{
    public static int AnimRate1ID = Shader.PropertyToID("_AnimRate1");
    public static int AnimRate2ID = Shader.PropertyToID("_AnimRate2");
    public static int AnimLerpID = Shader.PropertyToID("_AnimLerp");

    public FootmanGroup(Mesh mesh, Material material, AnimDataInfo animDataInfo) : base(mesh, material, animDataInfo)
    {
        FootmanCellItem.SetAnimData(animDataInfo);

        for (int x = 0; x < 5; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                FootmanCellItem item = new FootmanCellItem();
                item.Play("Walk", true);
                // item.CrossFade("Run", 1.0f, true);
                item.pos = new Vector3(x * 2, 0, y * 2);
                item.rotation = Quaternion.identity;
                AddCellItem(item);
            }
        }

    }

    public void CrossFade()
    {
        for (int i = 0; i < 25; i++)
        {
            (m_Cells[0].Get(i) as GPUInstanceAnimedCellItem).CrossFade("Run", 1.0f, true);
        }
    }

    protected override GPUInstanceCell OnCreateCell()
    {
        return new FootmanCell(this);
    }

}
