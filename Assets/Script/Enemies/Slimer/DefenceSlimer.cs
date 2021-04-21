/**
 * @Description: 史莱姆的防御组件，只增加了debug信息功能
 * @Author: ridger

 * 
 */

using UnityEngine;
using System.Text;
using UnityEngine.UI;

public class DefenceSlimer : Defence
{
    //ui:DebugInfo
    private Text debugInfoUI;
    private StringBuilder debugInfo = new StringBuilder(1024);

    protected override void Awake()
    {
        base.Awake();
        debugInfoUI = GameObject.Find("SlimerDebug").GetComponent<Text>();
    }

    public override void AttackCheck()
    {
        SetStatistic();
        Damage();
        ChangeDebugInfo();
    }
    public void ChangeDebugInfo()
    {
        debugInfo.Append("hp: ");
        debugInfo.AppendLine(hp.ToString());

        debugInfo.Append("isDead: ");
        debugInfo.AppendLine(isDead.ToString());

        debugInfo.Append("attackNum: ");
        debugInfo.AppendLine(attackNum.ToString());

        debugInfo.Append("damageSum: ");
        debugInfo.AppendLine(damageSum.ToString());

        debugInfo.Append("maxInterrupt: ");
        debugInfo.AppendLine(maxInterrupt.ToString());

        debugInfo.Append("hpReduction: ");
        debugInfo.AppendLine(hpReduction.ToString());

        debugInfoUI.text = debugInfo.ToString();

        debugInfo.Clear();
    }
}
