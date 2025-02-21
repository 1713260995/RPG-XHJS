using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using System.IO;

public class MyTools
{
    #region 简化Unity自带方法
    private static void Print(object msg)
    {
        Debug.Log(msg);
    }
    private static void Destroy(GameObject obj)
    {
        GameObject.Destroy(obj);
    }
    #endregion

    /// <summary>
    /// 生成指定长度的唯一字符串
    /// </summary>
    /// <param name="len">len=0时为默认长度,最大长度不超过20</param>
    /// <returns></returns>
    public static string GenerateStringID(int len = 0)
    {
        if (len > 20)
        {
            len = 20;
        }
        var res = Guid.NewGuid().ToString();
        return len == 0 ? res : res.Substring(0, len);
    }

    /// <summary>
    /// 生成指定长度的唯一数（纯数字）
    /// </summary>
    /// <param name="len"></param>
    /// <returns></returns>
    public static int GenerateIntID(int len)
    {
        byte[] buffer = Guid.NewGuid().ToByteArray();
        string res = BitConverter.ToInt64(buffer, 0).ToString();
        string r = res.Substring(0, len);
        int result = Convert.ToInt32(r);
        return result;
    }

    /// <summary>
    /// 获取本机局域网ip地址
    /// </summary>
    /// <returns></returns>
    public static string GetLocalIP()
    {
        return Dns.GetHostEntry(Dns.GetHostName()).AddressList.FirstOrDefault(p => p.AddressFamily == AddressFamily.InterNetwork).ToString();
    }

    /// <summary>
    /// 获取鼠标点击的世界坐标
    /// </summary>
    /// <param name="LorR"></param>
    public static Vector3 GetPonitByMouse(int LorR)
    {
        if (Input.GetMouseButtonDown(LorR))
        {
            //从摄像机向鼠标点击位置发射一条射线
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var point = hit.point;//获取发射到的第一个碰撞体
                return point;
            }
        }
        return Vector3.zero;
    }

    #region 获取两个点的角度
    /// <summary>
    /// 两个点的角度。 0 至 90 右，90 正上， 90 至 180 左上，-90 正下， 0 至 -90 左下， -90 至 -180 右下。
    /// </summary>
    /// <param name="p1">初始点</param>
    /// <param name="p2"></param>
    /// <returns></returns>
    public static float PointToAngle(Vector2 p1, Vector2 p2)
    {
        Vector2 p;
        p.x = p2.x - p1.x;
        p.y = p2.y - p1.y;
        return Mathf.Atan2(p.y, p.x) * 180 / Mathf.PI;
    }

    /// <summary>
    /// -180至180（PointToAngle方法） 转为 0-360
    /// </summary>
    /// <param name="angle"></param>
    /// <returns></returns>
    public static float Angle180To360(float angle)
    {
        if (angle >= 0 && angle <= 180)
            return angle;
        else
            return 360 + angle;
    }

    /// <summary>
    /// 返回Unity的角度（值和PointToAngle方法的一样）
    /// </summary>
    /// <param name="p1">初始点</param>
    /// <param name="p2"></param>
    /// <returns></returns>
    public static float GetUnityDirection(Vector2 p1, Vector2 p2)
    {
        float angle = Angle180To360(PointToAngle(p1, p2));
        float temp = 360 * 0.125f;//分为8个方向
        float dir = 0;
        for (int i = 0; i < 8; i++)
        {
            if (angle >= (i * temp) - (temp * 0.5f) && angle < (i * temp) + (temp * 0.5f))
            {
                dir = i * temp;
                break;
            }
        }
        return dir;
    }
    #endregion

    /// <summary>
    /// 游戏对象向前移动
    /// </summary>
    /// <param name="transform">游戏对象的位置</param>
    /// <param name="MoveSpeed">移动速度</param>
    /// <param name="action">一般是加该物体的跑步动画</param>
    /// <param name="direction">1为前进，-1为后退</param>
    public static void Forward(Transform transform, float MoveSpeed, Action action = null, int direction = 1)
    {
        if (action != null)
        {
            action();
        }
        transform.Translate(Vector3.forward * Time.deltaTime * MoveSpeed * direction);
    }

    /// <summary>
    /// 使物体左右旋转
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="rotateSpeed"></param>
    /// <param name="direction">1为左转，-1为右转</param>
    public static void Rotate(Transform transform, float rotateSpeed, int direction)
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime * direction);
    }

    /// <summary>
    /// 控制人物将其旋转至指定角度
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="v3"></param>
    public static void RotateAppointAngleForModel(Transform transform, float angle, float rotateSpeed = 0.05f)
    {
        //*-1是因为默认的旋转方向是逆时针，unity内的旋转方向是顺时针，*-1将旋转方向改为和unity的一致
        Quaternion quaternion = Quaternion.Euler(0, -1 * (angle - 90), 0);
        transform.rotation = Quaternion.Lerp((Quaternion)transform.rotation, quaternion, rotateSpeed);
    }

    /// <summary>
    /// 逐渐旋转至指定角度.
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="axle">轴向</param>
    /// <param name="angle">角度</param>
    public static void RotateAppointAngleByAxle(Transform transform, Vector3 axle, float angle, float lerpSpeed = 0.3f)
    {
        Quaternion quaternion = Quaternion.Euler(axle * angle);
        transform.rotation = Quaternion.Lerp(transform.rotation, quaternion, lerpSpeed);
    }


    /// <summary>
    ///  朝向目标，并向目标移动
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="Target"></param>
    /// <param name="action">可不传参，一般用来启动物体的跑步动画</param>
    public static void FindTarget(Transform transform, Transform Target, float moveSpeed, Action action = null, float rotateSpeed = 2.5f)
    {
        FindTarget(transform, Target.position, moveSpeed, action, rotateSpeed);
    }
    public static void FindTarget(Transform transform, Vector3 Target, float moveSpeed, Action action = null, float rotateSpeed = 2.5f)
    {
        if (action != null)
        {
            action();
        }
        var direction = Target - transform.position;//目标方向
        RotateToTarget(transform, Target, rotateSpeed);
        ////angle 0-90度前方，90-180度后方
        //var angle = Vector3.Angle(transform.forward, direction);//获取夹角
        ////由于y轴是朝上的，根据叉乘的y值判断目标在左方还是右方，小于0左方，大于0右方
        //var cross = Vector3.Cross(transform.forward, direction);
        //var turn = cross.y >= 0 ? 1f : -1f;
        //transform.Rotate(transform.up, angle * Time.deltaTime * rotateSpeed * turn, Space.World);//当夹角（angle）为0时不在旋转
        transform.Translate(direction.normalized * Time.deltaTime * moveSpeed, Space.World);//向目标方向移动，normalized归一实现匀速移动
    }

    /// <summary>
    /// 使游戏对象朝着指定目标的位置旋转。
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="Target"></param>
    /// <param name="rotateSpeed"></param>
    public static float RotateToTarget(Transform transform, Transform Target, float rotateSpeed = 2.5f)
    {
        return RotateToTarget(transform, Target.position, rotateSpeed);
    }
    public static float RotateToTarget(Transform transform, Vector3 Target, float rotateSpeed = 2.5f)
    {
        var direction = Target - transform.position;//目标方向
        //angle 0-90度前方，90-180度后方
        var angle = Vector3.Angle(transform.forward, direction);//获取夹角
        //由于y轴是朝上的，根据叉乘的y值判断目标在左方还是右方，小于0左方，大于0右方
        var cross = Vector3.Cross(transform.forward, direction);
        var turn = cross.y >= 0 ? 1f : -1f;
        transform.Rotate(transform.up, angle * Time.deltaTime * rotateSpeed * turn, Space.World);//当夹角（angle）为0时不在旋转
        return angle;
    }

    /// <summary>
    /// 检测目标是否在距离内
    /// </summary>
    /// <param name="org"></param>
    /// <param name="target"></param>
    /// <param name="maxRange"></param>
    /// <returns></returns>
    public static bool CheckDistanceIsContain(Transform org, Transform target, float maxRange)
    {
        var heading = target.position - org.position;
        return heading.sqrMagnitude < (maxRange * maxRange);
    }

    /// <summary>
    /// 获得终点坐标。根据起点，角度，距离，三角函数计算
    /// </summary>
    /// <param name="angle">角度</param>
    /// <param name="startPoint">起点</param>
    /// <param name="distance">距离</param>
    /// <returns>终点坐标</returns>
    public static Vector2 GetEndPointByTrigonometric(Vector2 startPoint, double angle, double distance)
    {
        //角度转弧度
        double radian = (angle * Math.PI) / 180;
        //计算新坐标 r 就是两者的距离
        var x = startPoint[0] + distance * Math.Cos(radian);
        var y = startPoint[1] + distance * Math.Sin(radian);
        return new Vector2((float)x, (float)y);
    }

    /// <summary>
    /// 已知一个起点a1和目标点a2的位置，求另一个原点b1的目标点, （此目标点和b1的方向）和（a1，a2的方向）相同。
    /// 注意：如果是三维坐标，只能填二维坐标的数据，上下移动就是x，y；前后左右移动就是x，z
    /// </summary>
    /// <param name="origin_a1"></param>
    /// <param name="target_a2"></param>
    /// <param name="origin_b1"></param>
    /// <param name="func">这个函数用来设置目标点的距离</param>
    /// <returns></returns>
    public static Vector2 GetEndPointByDistance(Vector2 origin_a1, Vector2 target_a2, Vector2 origin_b1, Func<float, float> func = null)
    {
        float distance = Vector2.Distance(target_a2, origin_a1);
        if (func != null)
        {
            distance = func(distance);
        }
        var rotateAngle = Angle180To360(PointToAngle(origin_a1, target_a2));
        Vector2 target_b2 = GetEndPointByTrigonometric(origin_b1, rotateAngle, distance);
        return target_b2;
    }

    /// <summary>
    /// 在场景指定位置生成预制体，再生成游戏对象
    /// </summary>
    /// <param name="path"></param>
    /// <param name="createTransform"></param>
    /// <param name="name"></param>
    public static GameObject LoadPrefab(string path, Vector3 createPosition, string name = null)
    {
        GameObject gameObj = (GameObject)Resources.Load(path);    //加载预制体到内存
        return LoadObj(gameObj, createPosition, name);
    }

    /// <summary>
    /// 加载游戏对象
    /// </summary>
    /// <param name="gameObj"></param>
    /// <param name="createPos"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public static GameObject LoadObj(GameObject gameObj, Vector3 createPos, string name = null)
    {
        gameObj = UnityEngine.Object.Instantiate(gameObj);    //实例化敌人
        gameObj.name = (name == null ? gameObj.name : name);//不命名则为默认，自定义命名则必须在Instantiate后命名
        gameObj.transform.position = createPos;    //生成的位置
        return gameObj;
    }

    /// <summary>
    /// 返回当前层级正在播放动画的完成度
    /// </summary>
    /// <param name="anim"></param>
    /// <returns>结果为0-1， 0为播放开始，1为播放结束</returns>
    public static float CheakCurrentAnimNormalized(Animator anim, int layerIndex = 0)
    {
        return anim.GetCurrentAnimatorStateInfo(layerIndex).normalizedTime;
    }

    /// <summary>
    /// 判断该动画状态机的指定层级的指定动画是否在播放
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="StateName">动画状态名字</param>
    /// <param name="index">第一层的索引为0</param>
    /// <returns></returns>
    public static bool PlayingThisAnim(Animator animator, string StateName, int index = 0)
    {
        var stateinfo = animator.GetCurrentAnimatorStateInfo(index);
        //判断动画QSkill是否正在播放
        return stateinfo.IsName(StateName);

    }

    /// <summary>
    /// 将动画重置值初始状态。如果该动画处于初始状态，就不操作
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="StateName">初始动画的状态名字</param>
    /// <param name="layer">动画层级，0为第一层</param>
    public static void AnimatorReset(Animator animator, string StateName, int layer = 0)
    {
        if (!PlayingThisAnim(animator, StateName))
        {
            foreach (var item in animator.parameters)
            {
                if (item.type == AnimatorControllerParameterType.Bool)
                {
                    animator.SetBool(item.name, false);
                }
            }
            //Play方法的一个最后参数值的区间为0-1.表示从该动画从哪开始播放。0为初始位置，0.5为中间，1为末尾
            animator.Play(StateName, layer, 0);
        }
    }

    /// <summary>
    /// 获取游戏对象下一级的所有子节点
    /// </summary>
    /// <param name="father"></param>
    /// <returns></returns>
    public static List<Transform> GetChilds(Transform father)
    {
        var childsList = new List<Transform>();
        for (int i = 0; i < father.childCount; i++)
        {
            childsList.Add(father.GetChild(i));
        }
        return childsList;
    }

    /// <summary>
    /// 检测鼠标是否点击到指定UI
    /// </summary>
    /// <param name="mousePosition"></param>
    /// <returns></returns>
    public static bool IsMouseGetUI(Vector2 mousePosition, string name)
    {
        //创建一个点击事件
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = mousePosition;
        List<RaycastResult> raycastResults = new List<RaycastResult>();
        //向点击位置发射一条射线，检测是否点击UI
        EventSystem.current.RaycastAll(eventData, raycastResults);
        foreach (var item in raycastResults)
        {
            if (item.gameObject.name == name)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// 移除刚体组件
    /// </summary>
    /// <param name="obj"></param>
    public static void RemoveRigidbody(GameObject obj)
    {
        GameObject.Destroy(obj.GetComponent<Rigidbody>());
    }

    /// <summary>
    /// 判断此游戏对象的层级是否和指定层级相等
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="layerName"></param>
    /// <returns></returns>
    public static bool IsEqualLayer(GameObject obj, string layerName)
    {
        return LayerMask.NameToLayer(layerName) == obj.layer;
    }

    /// <summary>
    /// 使物体逐渐消失
    /// </summary>
    /// <param name="obj"></param>
    public static async void DisappearObj(GameObject obj)
    {
        for (int i = 0; i < 10; i++)
        {
            await Task.Delay(30);
            if (obj == null)
            {
                return;
            }
            float a = Mathf.Lerp(obj.transform.localScale.x, 0, 0.05f);
            obj.transform.localScale = new Vector3(a, a, a);
        }
        Destroy(obj);
    }

    /// <summary>
    /// 获取鼠标按住时的位置
    /// </summary>
    /// <param name="isLeft">true为左键，false为右键</param>
    public static Vector3? GetMouseStayPiont(bool isLeft)
    {
        int i = isLeft == true ? 0 : 1;
        if (Input.GetMouseButtonDown(i))
        {
            Ray myRay = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(myRay, out hit))
            {
                return new Vector3(hit.point.x, hit.point.y, hit.point.z);
            }
        }
        return null;
    }

    /// <summary>
    /// 写入文件
    /// </summary>
    /// <param name="path"></param>
    /// <param name="content"></param>
    public static void WriteTxt(string path, string content)
    {
        path = Application.dataPath + "\\" + path;
        Print("读取地址是" + path);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        FileStream fs = new FileStream(path, FileMode.Create);   //打开一个写入流
        byte[] bytes = Encoding.UTF8.GetBytes(content);
        fs.Write(bytes, 0, bytes.Length);
        fs.Flush();     //流会缓冲，此行代码指示流不要缓冲数据，立即写入到文件。
        fs.Close();     //关闭流并释放所有资源，同时将缓冲区的没有写入的数据，写入然后再关闭。
        fs.Dispose();   //释放流所占用的资源，Dispose()会调用Close(),Close()会调用Flush();    也会写入缓冲区内的数据。
    }

    /// <summary>
    /// 读取文件
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static string ReadTxt(string path)
    {
        path = Application.dataPath + "\\" + path;
        Print("读取地址是" + path);
        if (!File.Exists(path))
        {
            return null;
        }
        // 读取文件的所有内容
        var text = File.ReadAllText(path, Encoding.UTF8);
        return text;
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public static void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif

    }
}