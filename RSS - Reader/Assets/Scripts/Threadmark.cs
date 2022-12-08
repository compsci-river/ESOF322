using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Threadmark
{
    private string thread;
    private string title;
    private string link;
    private DateTime datetime;
    private string read = null;

    public Threadmark(string _thread, string _title, string _link, DateTime _datetime)
    {
        thread = _thread;
        title = _title;
        link = _link;
        datetime = _datetime;
    }

    public Threadmark(ThreadmarkSave save)
    {
        thread = save.thread;
        title = save.title;
        link = save.link;
        datetime = DateTime.Parse(save.datetime);
        if (!save.read.Equals("null"))
        {
            read = save.read;
        }
    }

    public ThreadmarkSave save()
    {
        ThreadmarkSave save = new ThreadmarkSave();
        save.thread = thread;
        save.title = title;
        save.link = link;
        save.datetime = datetime.ToString();
        if (read == null)
        {
            save.read = "null";
        }
        else
        {
            save.read = read;
        }
        return save;
    }

    public string getTitle()
    {
        return title;
    }

    public string getLink()
    {
        return link;
    }

    public DateTime getDateTime()
    {
        return datetime;
    }

    public string getRead()
    {
        return read;
    }

    public string getThread()
    {
        return thread;
    }

    public void markRead()
    {
        read = DateTime.Now.ToString();
    }

    public void readThreadmark()
    {
        markRead();
        Application.OpenURL(link);
    }
}
