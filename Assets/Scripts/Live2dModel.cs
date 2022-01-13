using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using live2d;
using live2d.framework;
public class Live2dModel : MonoBehaviour {
    public TextAsset modelFile;

    private Live2DModelUnity live2dModel;
    public Texture2D[] textures;

    public TextAsset[] motionFiles;
    public Live2DMotion[] motions;

    private L2DMotionManager l2DMotionManager;
    //优先级设置标准
    //动作未进行的状态 0
    //待机动作优先级为1
    //其他动作 优先级为2
    //无视优先级，强制发生动作为4

    private Matrix4x4 live2dCanvasPos; //liv2d自己的画布

    private MotionQueueManager mtionQueueManager = new MotionQueueManager();           
    private MotionQueueManager mtionQueueManager2 = new MotionQueueManager();
    public int motionIndex;


    //自动眨眼
    private EyeBlinkMotion eyeBlinkMotion;

    //鼠标拖拽引起的动作变化
    private L2DTargetPoint drag;


    //套用物理运算设定
    //private PhysicsHair physicsHairSide;
    private PhysicsHair physicsHairRight;
    private PhysicsHair physicsHairLeft;

    private PhysicsHair physicsHairBackLeft;
    private PhysicsHair physicsHairBackRight;


    //表情
    public TextAsset[] expressionFiles;
    public L2DExpressionMotion[] expressions;
    private MotionQueueManager expressionMotionQueueManager;

    //private Live2DMotion live2dMontion1;
    void Start () {
        Live2D.init();
        //Live2DModelUnity.loadModel(Application.dataPath + "/Resources/Epsilon/runtime/Epsilon.moc");

        //TextAsset mocFile = Resources.Load<TextAsset>("/Resources/Epsilon/runtime/Epsilon_byte.moc.bytes");
        live2dModel = Live2DModelUnity.loadModel(modelFile.bytes);

        //与贴图简历关联
        //Texture2D texture2D_1 = Resources.Load<Texture2D>("/Epsilon/runtime/Epsilon.1024/texture_00");
        //Texture2D texture2D_2 = Resources.Load<Texture2D>("/Epsilon/runtime/Epsilon.1024/texture_01");
        //Texture2D texture2D_3 = Resources.Load<Texture2D>("/Epsilon/runtime/Epsilon.1024/texture_02");
        //live2dModel.setTexture(0, texture2D_1);
        //live2dModel.setTexture(0, texture2D_2);
        //live2dModel.setTexture(0, texture2D_3);
        for (int i = 0; i < textures.Length; i++)
        {
            live2dModel.setTexture(i, textures[i]);
        }
        //指定显示位置与尺寸
        float modelwidth = live2dModel.getCanvasWidth();
        float modelHeigh = live2dModel.getCanvasHeight();

        live2dCanvasPos = Matrix4x4.Ortho(0, modelwidth, modelwidth, 0, -50, 50);


        //播放动作
        //实例化动作对象
        //live2dMontion1 = Live2DMotion.loadMotion(Application.dataPath + "");

        //TextAsset mtnFile = Resources.Load<TextAsset>("");
        //live2dMontion1 = Live2DMotion.loadMotion(mtnFile.bytes);
        motions = new Live2DMotion[motionFiles.Length];
        for (int i = 0; i < motions.Length; i++)
        {
            motions[i] = Live2DMotion.loadMotion(motionFiles[i].bytes);
        }
        
        motions[0].setLoopFadeIn(false);//重复播放不淡入
        motions[0].setFadeOut(1000);//淡出时间（ms）
        motions[0].setFadeIn(1000);//淡入时间（ms）
        //motions[0].setLoop(true);//动画是否循环播放

        ////mtionQueueManager = new MotionQueueManager();
        //mtionQueueManager.startMotion(motions[0]);
        //mtionQueueManager2.startMotion(motions[5]);

        //动作的优先级使用
        l2DMotionManager = new L2DMotionManager();

        //眨眼
        eyeBlinkMotion = new EyeBlinkMotion();

        //鼠标拖拽实例化
        drag = new L2DTargetPoint();

        ///头发摇摆
        #region 左右两侧头发摇摆
        ///左侧头发摇摆
        physicsHairLeft = new PhysicsHair();

        //套用物理运算
        physicsHairLeft.setup(0.2f,0.5f,0.14f);//1 长度 单位公尺 影响摇摆快慢  2 空气阻力 0~1  3 质量 单位千克 

        PhysicsHair.Src srcXLeft = PhysicsHair.Src.SRC_TO_X; //横向摇摆
        //PhysicsHair.Src srcXLeft = PhysicsHair.Src.SRC_TO_G_ANGLE;
        //第三个参数 PARAM_ANGLE_X变动时头发收到0.005倍的影响度的输入参数
        physicsHairLeft.addSrcParam(srcXLeft, "PARAM_ANGLE_X",0.005f,1);

        //设置输出的表现
        PhysicsHair.Target targetLeft = PhysicsHair.Target.TARGET_FROM_ANGLE;//表现形式

        physicsHairLeft.addTargetParam(targetLeft,"PARAM_HAIR_SIDE_L", 0.005f, 1);



        //右侧头发摇摆
        physicsHairRight = new PhysicsHair();

        //套用物理运算
        physicsHairRight.setup(0.2f, 0.5f, 0.14f);//1 长度 单位公尺 影响摇摆快慢  2 空气阻力 0~1  3 质量 单位千克 

        PhysicsHair.Src srcXRight = PhysicsHair.Src.SRC_TO_X; //横向摇摆
        //第三个参数 PARAM_ANGLE_X变动时头发收到0.005倍的影响度的输入参数
        physicsHairRight.addSrcParam(srcXRight, "PARAM_ANGLE_X", 0.005f, 1);

        //设置输出的表现
        PhysicsHair.Target targetRight = PhysicsHair.Target.TARGET_FROM_ANGLE;//表现形式

        physicsHairRight.addTargetParam(targetRight, "PARAM_HAIR_SIDE_R", 0.005f, 1);


        #endregion

        #region 左右后发的摇摆
        physicsHairBackLeft = new PhysicsHair();


        physicsHairBackLeft.setup(0.24f, 0.5f, 0.18f);

        //左边
        PhysicsHair.Src srcXBackLeft = PhysicsHair.Src.SRC_TO_X;
        PhysicsHair.Src srcZBackLeft = PhysicsHair.Src.SRC_TO_G_ANGLE;
        physicsHairBackLeft.addSrcParam(srcXBackLeft, "PARAM_ANGLE_X", 0.005f, 1);
        physicsHairBackLeft.addSrcParam(srcZBackLeft, "PARAM_ANGLE_Z", 0.8f, 1);

        PhysicsHair.Target targetBackLeft = PhysicsHair.Target.TARGET_FROM_ANGLE;
        physicsHairBackLeft.addTargetParam(targetBackLeft, "PARAM_HAIR_BACK_L", 0.005f, 1);

        //右边
        physicsHairBackRight = new PhysicsHair();
        physicsHairBackRight.setup(0.24f, 0.5f, 0.18f);

        PhysicsHair.Src srcXBackRight = PhysicsHair.Src.SRC_TO_X;
        PhysicsHair.Src srcZBackRight = PhysicsHair.Src.SRC_TO_G_ANGLE;

        physicsHairBackRight.addSrcParam(srcXBackRight, "PARAM_ANGLE_X", 0.005f, 1);
        physicsHairBackRight.addSrcParam(srcZBackRight, "PARAM_ANGLE_Z", 0.8f, 1);
        //physicsHairBackRight.setup(0.2f, 0.5f, 0.14f);
        PhysicsHair.Target targetBackRight = PhysicsHair.Target.TARGET_FROM_ANGLE;
        physicsHairBackRight.addTargetParam(targetBackRight, "PARAM_HAIR_BACK_R", 0.005f, 1);
        #endregion

        //表情
        expressionMotionQueueManager = new MotionQueueManager();
        expressions = new L2DExpressionMotion[expressionFiles.Length];
        for (int i = 0; i < expressions.Length; i++)
        {
            expressions[i] = L2DExpressionMotion.loadJson(expressionFiles[i].bytes);
        }

        //释放
        //live2d.dispose();


    }

    // Update is called once per frame
    void Update () {
        live2dModel.setMatrix(transform.localToWorldMatrix* live2dCanvasPos);

        //if (Input.GetKeyDown(KeyCode.M))
        //{
        //    motionIndex++;
        //    if (motionIndex >= motions.Length)
        //    {
        //        motionIndex = 0;
        //    }
        //    mtionQueueManager.startMotion(motions[motionIndex]);
        //}
        //mtionQueueManager.updateParam(live2dModel);
        //mtionQueueManager2.updateParam(live2dModel);

        //表情管理
        if (Input.GetKeyDown(KeyCode.M))
        {
            motionIndex++;
            if (motionIndex >= expressions.Length)
            {
                motionIndex = 0;
            }
            expressionMotionQueueManager.startMotion(expressions[motionIndex]);
        }
        expressionMotionQueueManager.updateParam(live2dModel);


        //判断待机动作
        //if (l2DMotionManager.isFinished())
        //{
        //    StartMotion(0, 1);
        //}
        //else if (Input.GetKeyDown(KeyCode.M))
        //{
        //    StartMotion(14,2);
        //}
        //l2DMotionManager.updateParam(live2dModel);



        //设置参数
        //live2dModel.setParamFloat("PARAM_ANGLE_X",30,1);
        //live2dModel.addToParamFloat("PARAM_ANGLE_X", 1);

        //live2dModel.multParamFloat("PARAM_ANGLE_X", 1);

        //参数的保存与恢复
        //live2dModel.saveParam();
        //live2dModel.loadParam();

        //设置模型某一部分透明度
        //live2dModel.setPartsOpacity("PARTS_01_FACE_001", 0);

        //眨眼
        eyeBlinkMotion.setParam(live2dModel);

        //模型更新鼠标转向和看向
        Vector3 pos = Input.mousePosition;
        if (Input.GetMouseButton(0))
        {
            drag.Set(pos.x / Screen.width * 2 - 1, pos.y / Screen.height * 2 - 1);//固定公式
        }
        else if (Input.GetMouseButtonUp(0))
        {
            drag.Set(0, 0);
        }
        //参数及时更新
        drag.update();

        //模型的转向
        if (drag.getX() != 0)
        {
            live2dModel.setParamFloat("PARAM_ANGLE_X",30*drag.getX());
            live2dModel.setParamFloat("PARAM_ANGLE_Y", 30 * drag.getY());
            live2dModel.setParamFloat("PARAM_BODY_ANGLE_X", 10 * drag.getX());
            live2dModel.setParamFloat("PARAM_EYE_BALL_X", 10 * drag.getX());
            live2dModel.setParamFloat("PARAM_EYE_BALL_Y", 10 * drag.getY());
        }
        long time = UtSystem.getSystemTimeMSec();//执行时间

        physicsHairRight.update(live2dModel, time);//
        physicsHairLeft.update(live2dModel, time);
        physicsHairBackLeft.update(live2dModel, time);
        physicsHairBackRight.update(live2dModel, time);
        //更新顶点 参数 等.....

        live2dModel.update();


	}
    //绘图
    private void OnRenderObject()
    {
        live2dModel.draw();
    }
    private void StartMotion(int motionindex, int priority)
    {
        if (l2DMotionManager.getCurrentPriority() >= priority)
        {
            return;
        }
        l2DMotionManager.startMotion(motions[motionindex]);
 
    }

}
