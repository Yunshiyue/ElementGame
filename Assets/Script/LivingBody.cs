using UnityEngine;
/**
 * @Description: CanFight类是所有能够进行“攻击”的单位的父类。拥有血量、攻击力、是否死亡等基本属性，以及攻击、被攻击、死亡等基本方法。
 * @Author: ridger
 * @Date: 2021-1-26 19:51
 * 
 * @EditTime: 2021/1/27 16:03:00  改动时间
 * @Editors: 夜里猛  
 * @Edit:CanFight改为LivingBody 为player和Enermy的父类，去除了攻击、被攻击的方法,增加了移动的方法。

 */
abstract public class LivingBody : MonoBehaviour
{
    /// <summary>
    /// 最大生命值
    /// </summary>
    public int hpMax;
    /// <summary>
    /// 当前生命值
    /// </summary>
    public int hpCurrent;
    /// <summary>
    /// 该单位的攻击力
    /// </summary>
    protected int ap;
    /// <summary>
    /// 标识该单位是否死亡
    /// </summary>
    protected bool isDead;
    /// <summary>
    /// 初始化该单位
    /// </summary>
    abstract public void Initialize();
    /// <summary>
    /// 单位死亡
    /// </summary>
    abstract public void Dead();
    /// <summary>
    /// 移动
    /// </summary>
    abstract public void Movement();

    /// <summary>
    /// 受到damage伤害，并保证血量不为负值
    /// </summary>
    /// <param name="damage">造成的伤害，并不判定是否合法</param>
    /// <returns>返回具体造成的伤害</returns>
    //public int BeAttacked(int damage) { 
    //    int originHp = hpCurrent;
    //    hpCurrent = (hpCurrent - damage) < 0 ? 0 : (hpCurrent - damage);
    //    return hpCurrent == 0 ? originHp : damage;
    //}



    /// <summary>
    /// 范围性攻击，实现方法为输入范围数组，检测范围内的每个CanFight的敌人，并造成伤害
    /// </summary>
    /// <returns></returns>
    /// TODO
    //abstract public int[] AttackArea(RectStruct[] areas, int[] damages);
    /// <summary>
    /// 生成飞行道具攻击，实现方法为生成某个含有脚本的攻击物体
    /// </summary>
    //abstract public void AttackGenarate();

    /// <summary>
    /// 对一个能够战斗的目标造成伤害，作为底层私有函数被调用
    /// </summary>
    /// <param name="target">造成伤害的目标</param>
    /// <param name="ap">如果没有给出，则默认为该实例本身的攻击力ap；如果给出，则造成输入伤害</param>
    /// <returns>返回造成了多少伤害，具体用法有待进一步讨论</returns>
    //public int Attack(LivingBody target, int damage = int.MinValue)
    //{
    //    if (damage == int.MinValue)
    //        return target.BeAttacked(ap);

    //    return target.BeAttacked(damage);
    //}
}
