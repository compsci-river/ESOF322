using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class ThreadSave
{
    public string title;
    public string threadLink;
    public string rssLink;
    public string datetime;
    public List<ThreadmarkSave> threadmarks = new List<ThreadmarkSave>();
}

[System.Serializable]
public class ThreadmarkSave
{
    public string thread;
    public string title;
    public string link;
    public string datetime;
    public string read;
}