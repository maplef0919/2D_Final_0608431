using UnityEngine;
using UnityEngine.UI; // 引用 介面 API
using UnityEngine.SceneManagement;  //引用 場景管理 API


public class Player : MonoBehaviour
{
    [Header("移動速度"), Range(0, 300)]
    public float speed = 0.5f;
    [Header("角色名稱"), Tooltip("這是角色的名稱")]
    public string cName = "狐狸";
    [Header("虛擬搖桿")]
    public FixedJoystick joystick;
    [Header("變形元件")]
    public Transform tra;
    [Header("偵測範圍")]
    public float rangeAttack = 1.2f;
    [Header("血量")]
    public float hp = 200;
    [Header("血條系統")]
    public HpManager hpManager;
    [Header("攻擊力"), Range(0, 1000)]
    public float attack = 20;
    public Animator m_ani;

    private bool isDead = false;
    private float hpMax;

    private void Start()
    {
        m_ani = gameObject.GetComponent<Animator>();
        hpMax = hp;
    }


    const string idle = "idle";
    const string move = "move";
    public void ChangeAniState(string Animation)
    {
        m_ani.Play(Animation);
    }
    
    //事件:繪製圖示
    private void OnDrawGizmos()
    {
        //指定圖示顏色(紅，綠，藍，透明度)
        Gizmos.color = new Color(1, 0, 0, 0.2f);
        //繪製圖示 球體(中心點，半徑)
        Gizmos.DrawSphere(transform.position, rangeAttack);
    }

    /// <summary>
    /// 移動
    /// </summary>
    private void Move()
    {
        if (isDead) return;                     //如果 死亡 就跳出
        
        float h = joystick.Horizontal;
        float v = joystick.Vertical;
        if (h != 0)
        {
            ChangeAniState(move);
        }
        else
        {
            ChangeAniState(idle);
        }


        if (h <= 0)
        {
            tra.localScale = new Vector3(1, 1, 0);
        }
        else if (h >= 0)
        {
            tra.localScale = new Vector3(-1, 1, 0);
        }
        //變形元件.位移
        tra.Translate(h * speed * Time.deltaTime, v * 0 * Time.deltaTime, 0);

    }

  

    //要被按鈕呼叫必須設定為公開 public
    public void Attack()
    {
        if (isDead) return;                     //如果 死亡 就跳出


        //2D物理 圓形碰撞(中心點，半徑，方向，距離，圖層編號(1<<X))
        RaycastHit2D hit = Physics2D.CircleCast(transform.position, rangeAttack, -transform.up, 0, 1 << 8);

        //如果 打到的標籤是 敵人 就對他造成傷害
        if (hit && hit.collider.tag == "敵人") hit.collider.GetComponent<Enemy>().Hit(attack);

    }

    /// <summary>
    /// 受傷
    /// </summary>
    /// <param name="damage">接受到的傷害直</param>
    public void Hit(float damage)
    {
        print("受傷");
        hp -= damage;                                  //扣除傷害直
        hpManager.UpdateHpBar(hp, hpMax);              //更新血條
        StartCoroutine(hpManager.ShowDamage(damage));  //啟動協同程序(顯示傷害數值)

        if (hp <= 0) Dead();                           //如果 血量 <= 0 就死亡
    }

    /// <summary>
    /// 死亡
    /// </summary>
    private void Dead()
    {
        hp = 0;
        isDead = true;
        Invoke("Replay", 2);               //延遲呼叫("方法名稱"，延遲時間)
    }

    /*
    IEnumerator CCmove( )
    {
        print("OK");
        ChangeAniState(idle);
        yield return new WaitForSeconds(1F);
        print("OK");
        ChangeAniState(move);
        yield return new WaitForSeconds(1F);
        ChangeAniState(idle);
        print("OK");

        yield break;
    }*/
    void Update()
    {
        Move();
    }


}
