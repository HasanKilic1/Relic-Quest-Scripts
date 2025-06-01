using System;
using System.Collections.Generic;
using UnityEngine;

public class ChestUI : MonoBehaviour
{
    [Serializable]
    public struct ChestInfo
    {
        public RectTransform chest;
        public string chestInfo;
    }

    [SerializeField] List<ChestInfo> chestList;
    [SerializeField] RectTransform panelCenter;

}
