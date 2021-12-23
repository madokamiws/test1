using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 跑马灯转盘
/// </summary>
public class RotaryTablePanel : MonoBehaviour
{
    //单次开始抽奖抽奖结束的事件
    private Action<bool> PlayingAction;
    ////三连抽开始抽奖抽奖结束的事件
    //private Action<bool> PlayingThreeAction;
    ////是否是三连抽
    //bool isThreeDraw;
    // 抽奖按钮，
    public Button drawBtn;
    //跳过抽奖动画
    public Toggle jumpTgl;
    // 抽奖图片父物体
    public Transform rewardImgTran;
 
    //转动特效
    public Transform eff_TurnFrame;
    //中奖特效
    public Transform eff_SelectFrame;
    // 抽奖图片
    private Transform[] rewardTransArr;
    private RotaryCell[] rewardCellArr;

    // 默认展示状态
    private bool isInitState;
    // 抽奖结束 -- 结束状态，光环不转
    private bool drawEnd;
    // 中奖
    private bool drawWinning;

    [Header("展示状态时间 --> 控制光环转动初始速度")]
    public float setrewardTime = 1f;

    private float rewardTime;
    private float rewardTiming = 0;

    // 当前光环所在奖励的索引
    private int haloIndex = 0;
    // 本次中奖ID
    private int rewardIndex = 0;

    // 点了抽奖按钮正在抽奖
    private bool isOnClickPlaying;

    public bool IsOnClickPlaying
    {
        get => isOnClickPlaying;
        set
        {
            isOnClickPlaying = value;
            if (eff_TurnFrame != null)
            {
                eff_TurnFrame.gameObject.SetActive(isOnClickPlaying);
            }
        }
    }

    public bool DrawWinning
    {
        get => drawWinning;
        set => drawWinning = value;
    }

    public bool DrawEnd
    {
        get => drawEnd;
        set
        {
            drawEnd = value;
            if (eff_SelectFrame != null)
            {
                eff_SelectFrame.gameObject.SetActive(drawEnd);
            }
        }
    }

    /// <summary>
    /// 注册转盘抽奖事件
    /// </summary>
    /// <param name="playingAction"></param>
    public void SetPlayingAction(Action<bool> playingAction, Action<bool> playingThreeAction)
    {
        PlayingAction = playingAction;
        //PlayingThreeAction = playingThreeAction;
    }

    public void Start()
    {
        Init();
    }
    public void Init()
    {
         drawBtn.onClick.AddListener(OnClickDrawFun);
        rewardTransArr = new Transform[rewardImgTran.childCount];
        rewardCellArr = new RotaryCell[rewardImgTran.childCount];
        for (int i = 0; i < rewardImgTran.childCount; i++)
        {
            rewardTransArr[i] = rewardImgTran.GetChild(i);
            rewardCellArr[i] = rewardTransArr[i].GetComponent<RotaryCell>();
        }

        // 默认展示时间
        rewardTime = setrewardTime;
        rewardTiming = 0;

        DrawEnd = false;
        DrawWinning = false;
        IsOnClickPlaying = false;

    }

    public void RePrepare()
    {
        if (IsOnClickPlaying)
        {
            return;
        }
        rewardTime = setrewardTime;
        rewardTiming = 0;

        DrawEnd = false;
        DrawWinning = false;
        IsOnClickPlaying = false;
        if (true)
        {
            for (int i = 0; i < rewardCellArr.Length; i++)
            {
                rewardCellArr[i].ShowEff(RotaryCell.EffType.all, false);
            }
        }

    }

    /// <summary>
    /// 从中奖状态恢复到默认状态
    /// </summary>
    /// <param name="index"></param>
    public void RestoreDefault(int index = 0)
    {
        index--;
        rewardCellArr[index].ShowEff(RotaryCell.EffType.all, false);
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            RePrepare();
        }
        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    OnClickDrawFunThree();
        //}
        if (DrawEnd || rewardCellArr == null) return;
        if (!IsOnClickPlaying)
        {
            return;
        }

        // 抽奖展示
        rewardTiming += Time.deltaTime;
        if (rewardTiming >= rewardTime)
        {

            rewardTiming = 0;

            haloIndex++;
            if (haloIndex >= rewardCellArr.Length)
            {
                haloIndex = 0;
            }
            //if (isThreeDraw)
            //    SetHaloThreePos(haloIndex);
            //else
                SetHaloPos(haloIndex);
        }
    }

    // 设置光环显示位置
    void SetHaloPos(int index)
    {

        rewardCellArr[index - 1 < 0 ? rewardCellArr.Length - 1 : index - 1].ShowEff(RotaryCell.EffType.turn, false);
        rewardCellArr[index].ShowEff(RotaryCell.EffType.turn, true);

        // 中奖 && 此ID == 中奖ID
        if (DrawWinning && index == rewardIndex)
        {
            rewardCellArr[index].ShowEff(RotaryCell.EffType.select, true);
            rewardCellArr[index].ShowEff(RotaryCell.EffType.turn, false);
            IsOnClickPlaying = false;
            DrawEnd = true;
            if (PlayingAction != null)
            {
                PlayingAction(false);

            }

            //todo...展示中奖物品，维护数据 --> 注意: index是索引
            Debug.Log("恭喜您中奖，中奖物品索引是：" + index + "号");
        }
    }

    // 点击抽奖按钮
    void OnClickDrawFun()
    {
        if (!IsOnClickPlaying)
        {
            haloIndex = -1;
            RePrepare();

            // 随机抽中ID
            //rewardIndex = UnityEngine.Random.Range(0, rewardCellArr.Length);
            //rewardIndex = 3;
            int[] table = new int[] { 100000, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100, 100000 };//权重手动配置
            rewardIndex = RandomByWeight(table);
            Debug.Log("result = " + rewardIndex);

            Debug.Log("开始抽奖，本次抽奖随机到的ID是：" + rewardIndex);

            IsOnClickPlaying = true;
            DrawEnd = false;
            DrawWinning = false;
            if (PlayingAction != null)
            {
                PlayingAction(true);
            }

            if (jumpTgl != null && jumpTgl.isOn)
            {
                rewardTime = 0.02f;
                DrawWinning = true;
            }
            else
                StartCoroutine(StartDrawAni());
        }
    }

    /// <summary>
    /// 开始抽奖动画
    /// 先快后慢 -- 根据需求调整时间
    /// </summary>
    /// <returns></returns>
    IEnumerator StartDrawAni()
    {
        rewardTime = setrewardTime;

        // 加速
        for (int i = 0; i < setrewardTime / 0.05f - 1; i++)
        {
            yield return new WaitForSeconds(0.05f);
            rewardTime -= 0.05f;
        }

        yield return new WaitForSeconds(2f);
        // 减速
        for (int i = 0; i < setrewardTime / 0.05f - 4; i++)
        {
            yield return new WaitForSeconds(0.05f);
            rewardTime += 0.02f;
        }

        yield return new WaitForSeconds(0.5f);
        DrawWinning = true;
    }

    public void OnDestroy()
    {
        Debug.Log("C#的关闭");
    }
    public static int RandomByWeight(int[] weights)
    {
        int sum = 0;
        for (int i = 0; i < weights.Length; i++)
        {
            sum += weights[i];
        }
        int number_rand = rand(sum);
        Console.WriteLine("number_rand = " + number_rand);

        int sum_temp = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            sum_temp += weights[i];
            if (number_rand <= sum_temp)
            {
                return i;
            }
        }

        return -1;
    }
    static int rand(int n)
    {
        System.Random rd = new System.Random();
        return rd.Next(0, n );
    }

}

