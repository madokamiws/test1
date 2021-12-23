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
