using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


/// <summary>
/// 相机跟随玩家，并且可以左右旋转相机
/// 1.先设置好相机和玩家位置
/// 2.计算玩家和相机位置的的差值，这是一个固定的向量。
/// 3.使相机与玩家一直保持这个向量的距离：玩家位置-此向量=摄像机的位置，
/// 4.以玩家为原点，旋转此向量。使相机旋转
/// 5.使相机一直朝向玩家
/// </summary>
public class CameraFollowPlayer : MonoBehaviour
{

    public Transform mainCamera;
    public Vector3 differFrom;
    public float moveSpeed;

    private void Start()
    {
        differFrom = transform.position - mainCamera.position;
    }
    private void Update()
    {
       // Move();
    }

    private void Move()
    {
        mainCamera.position = TerminalPoint();
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed * 3);
            GetComponentInChildren<Animator>().SetBool("run", true);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(Vector3.forward * Time.deltaTime * moveSpeed * -3);
            GetComponentInChildren<Animator>().SetBool("run", true);
        }
        if (!Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            GetComponentInChildren<Animator>().SetBool("run", false);
        }
        if (Input.GetKey(KeyCode.A))
        {
            differFrom = dir(differFrom, -1);
            transform.Rotate(Vector3.up);
        }
        if (Input.GetKey(KeyCode.D))
        {
            differFrom = dir(differFrom, 1);
            transform.Rotate(Vector3.down);
        }
    }

    private void LateUpdate()
    {
        mainCamera.LookAt(transform);
    }
    Vector3 dir(Vector3 dir, float angle)
    {
        //向量偏移       
        Quaternion pianyi = Quaternion.Euler(Vector3.up * angle);

        //获得旋转之后的向量
        Vector3 Normal = pianyi * dir;
        return Normal;
    }

    Vector3 TerminalPoint()
    {
        var point = transform.position - differFrom;
        return point;
    }


    Quaternion MyRotateAround(Vector3 center, Vector3 axis, float angle)
    {
        var lastPoint = mainCamera.transform;
        Vector3 pos = lastPoint.position;
        Quaternion rot = Quaternion.AngleAxis(angle, axis);
        Vector3 dir = pos - center; //计算从圆心指向摄像头的朝向向量
        dir = rot * dir;            //旋转此向量
        //lastPoint.position = center + dir;//移动摄像机位置
        var myrot = lastPoint.rotation;
        //transform.rotation *= Quaternion.Inverse(myrot) * rot *myrot;//设置角度另一种方法
        return rot * myrot; //设置角度
    }

}
