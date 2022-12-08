using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadmarkButton : MonoBehaviour
{
    public Threadmark threadmark;
    private Manager manager;

    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<Manager>();
    }

    public void ReadButton()
    {
        threadmark.readThreadmark();
        manager.Refresh();
    }

    public void MarkRead()
    {
        manager.MarkRead();
    }

    public void ToggleDisplayRead()
    {
        manager.ToggleDisplayRead();
    }

    public void ToggleOptions()
    {
        manager.ToggleOptions();
    }

    public void ToggleDeleteCheck()
    {
        manager.ToggleDeleteThreadCheck();
    }
}
