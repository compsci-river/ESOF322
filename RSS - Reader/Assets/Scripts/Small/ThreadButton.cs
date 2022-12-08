using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadButton : MonoBehaviour
{
    public Thread thread;
    private Manager manager;

    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<Manager>();
    }

    public void SelectThreadButton()
    {
        manager.SelectThread(thread);
    }

    public void SelectAllThreadsButton()
    {
        manager.SelectAllThreads();
    }

    public void TogglePullout()
    {
        manager.TogglePullout();
    }

    public void VisitThread()
    {
        manager.VisitThread();
    }

    public void Refresh()
    {
        manager.RefreshThreads();
    }

    public void DeleteThread()
    {
        manager.DeleteThreadButton();
    }
}
