using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DebugTipsText : SingletonMono<DebugTipsText> {
    private TextMeshProUGUI text;
    private List<string> tipsTextList;

    void Awake() {
        text = GetComponent<TextMeshProUGUI>();
        
    }

    void Start() {
        // AddTips("种菜需要先耕地浇水后才可以播种");
        // AddTips("完成屏幕左上角的任务获得金钱");
        // AddTips("把蔬菜放在下方的蓝色区域就可以打包");
        // AddTips("在田地左上方的宝石商店中可以升级土地");
        // AddTips("（请按顺序升级，否则可能会出现bug）");
        // AddTips("（没有BGM，大家放歌听吧）");
        // AddTips("WASD移动 Shift奔跑");
    }

    public void AddTips(string tipsText) {
        if (tipsTextList == null) {
            tipsTextList = new();
        }

        tipsTextList.Add(tipsText);

        FreshTipsText();
    }

    private void FreshTipsText() {
        string tipsContent = "";
        foreach (string tipsText in tipsTextList) {
            tipsContent += tipsText + '\n';
        }
        text.SetText(tipsContent);
    }

}
