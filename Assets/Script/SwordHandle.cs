using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class SwordHandle : MonoBehaviour
{
    // Start is called before the first frame update
    private int swordCount = 11;
    private float angle = 10f;
    private float radius = 5f;
    private float delayTime = 0.1f;

    List<GameObject> mylist = new List<GameObject>();
    public GameObject swordPrefab;

    public void SkillAttack(Vector2 attackPos)
    {
        swordPrefab.SetActive(true);
        List<Vector2> spawnPosList = new List<Vector2>();
        for(int i = 0; i < swordCount; i++)
        {
            var dir = i % 2 == 0 ? -1 : 1;
            // Quaternion.Euler(0, 0, angle * ((i + 1) / 2) * dir)绕z轴旋转对应角度
            var p = Quaternion.Euler(0, 0, angle * ((i + 1) / 2) * dir) * new Vector2(0, radius);
            spawnPosList.Add(p);
        }
        var startPos = attackPos + new Vector2(0, radius);//开始的地方

       

        for (int i = 0; i < spawnPosList.Count; i++)
        {
            //对一个对象进行复制操作
            Quaternion q1 = Quaternion.Euler(0, 0, 225f);
            var sword = Instantiate(swordPrefab, startPos, q1); //Quaternion.identity -> Quaternion(0, 0, 0, 0)
           
            mylist.Add(sword);

            //sword.transform.up = Vector2.down;
            //🗡的dotween序列
            var mSequence = DOTween.Sequence();
            //🗡的位置  ?
            var pos = attackPos + spawnPosList[i];
            //🗡攻击的方向
            var dir = (attackPos - pos).normalized;//normalized向量规范化
            //Debug.Log(dir);
            var dirto = i % 2 == 0 ? -1 : 1;
            mSequence.AppendInterval((i + 1) / 2 * delayTime);//添加一个间隔 等待一定时间再执行以下操作
            mSequence.Append(DOTween.To(() => sword.transform.position, x => sword.transform.position = x, (Vector3)dir, 0.3f));//平滑的旋转 append自上一个动画以后
            mSequence.Join(sword.transform.DORotate(new Vector3(0,0,225+angle * ((i + 1) / 2) * dirto), 0.3f));//Join 与上一个动画一起 旋转的同时移动散开
            mSequence.Join(sword.transform.DOMove(pos, 0.3f));//Join 与上一个动画一起 旋转的同时移动散开
            mSequence.AppendInterval(1);//等待1s
            mSequence.Append(sword.transform.DOMove(-dir * 2 + pos, 0.1f));//方向乘距离+位置 后退移动一点 0.1f为时间 
            mSequence.Append(sword.transform.DOMove(attackPos, 0.3f));// 进行攻击 DOMove(a,b) a为目标位置，b为时间
            mSequence.Play();
        }

        //yield return new WaitForSeconds(5.0f);
        //for (int i = 0; i < mylist.Count; i++)
        //{ 
        //    Destroy(mylist[i]);
        //}
        Invoke("DestorySword", 2.5f);
    }
    public void DestorySword()
    {
        Debug.Log("Destory");
        for (int i = 0; i < mylist.Count; i++)
        {
            Destroy(mylist[i]);
        }
        swordPrefab.SetActive(false);
    }

}
