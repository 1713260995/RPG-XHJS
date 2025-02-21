using System;
using UnityEngine;

public class SkillBase : MonoBehaviour
{
    public Action updateEvent;

    public Action<Collider> triggerEvent;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (updateEvent != null)
        {
            updateEvent();
        }
    }
    private void OnDestroy()
    {

    }

    public void OnTriggerEnter(Collider other)
    {
        if (triggerEvent != null)
        {
            triggerEvent(other);
        }
    }

}
