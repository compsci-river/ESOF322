using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Xml;
using System.Text.RegularExpressions;

public class Thread
{
    private string title;
    private string threadLink;
    private string rssLink;
    private DateTime datetime;
    private List<Threadmark> threadmarks = new List<Threadmark>();

    private string tempSave;

    public Thread(string _rssLink)
    {
        rssLink = _rssLink;

        
    }

    public bool CreateThread()
    {
        WebClient client = new WebClient();
        string downloadString = "";
        try
        {
            downloadString = client.DownloadString(rssLink);
        }
        catch
        {
            return false;
        }
        tempSave = downloadString;
        XmlDocument doc = new XmlDocument();
        try
        {
            doc.LoadXml(downloadString);
        }
        catch
        {
            return false;
        }

        if (doc.GetElementsByTagName("rss").Count == 0)
        {
            return false;
        }

        title = doc.GetElementsByTagName("title")[0].InnerText;
        datetime = DateTime.Parse(doc.GetElementsByTagName("pubDate")[0].InnerText).AddDays(-2);

        threadLink = SetThreadLink();
        return true;
    }

    private string SetThreadLink()
    {
        string pattern = ".rss?";
        string replace = "";

        Regex rgx = new Regex(pattern);
        string result = rgx.Replace(rssLink, replace);

        pattern = ".rss";
        rgx = new Regex(pattern);
        result = rgx.Replace(result, replace);

        return result;
    }

    public Thread(ThreadSave save)
    {
        title = save.title;
        threadLink = save.threadLink;
        rssLink = save.rssLink;
        datetime = DateTime.Parse(save.datetime);
        foreach (ThreadmarkSave _save in save.threadmarks)
        {
            threadmarks.Add(new Threadmark(_save));
        }
    }

    public ThreadSave save()
    {
        ThreadSave save = new ThreadSave();
        save.title = title;
        save.threadLink = threadLink;
        save.rssLink = rssLink;
        save.datetime = datetime.ToString();
        foreach (Threadmark threadmark in threadmarks)
        {
            save.threadmarks.Add(threadmark.save());
        }

        return save;
    }

    private void initialize()
    {
        WebClient client = new WebClient();
        string downloadString = client.DownloadString(rssLink);
        tempSave = downloadString;
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(downloadString);

        title = doc.GetElementsByTagName("title")[0].InnerText;
        datetime = DateTime.Parse(doc.GetElementsByTagName("pubDate")[0].InnerText);

        threadLink = rssLink.Substring(0, rssLink.Length - 3) + "com";
    }

    public void checkForUpdates()
    {
        string downloadString = null;
        try
        {
            WebClient client = new WebClient();
            downloadString = client.DownloadString(rssLink);
        }
        catch (Exception e)
        {
            Debug.Log("Exception: " + e.Message);
            downloadString = tempSave;
        }
        
        if (downloadString != null) 
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(downloadString);

            DateTime pubdate = DateTime.Parse(doc.GetElementsByTagName("pubDate")[0].InnerText);

            if (DateTime.Compare(datetime, pubdate) < 0)
            {
                getUpdates(doc);
            }
            datetime = pubdate;
        }
    }

    private void getUpdates(XmlDocument doc)
    {
        foreach (Threadmark mark in threadmarks)
        {
            if (mark.getRead() != null && (DateTime.Now - DateTime.Parse(mark.getRead())).TotalDays >= 7)
            {
                threadmarks.Remove(mark);
            }
        }

        XmlNodeList list = doc.GetElementsByTagName("item");

        foreach (XmlNode node in list)
        {
            XmlNode work = node.FirstChild;
            string _title = work.InnerText;
            work = work.NextSibling;
            DateTime date = DateTime.Parse(work.InnerText);
            work = work.NextSibling;
            string _link = work.InnerText;
            if(DateTime.Compare(datetime, date) < 0)
            {
                newThreadmark(_title, _link, date);
            }
            else
            {
                break;
            }
        }

    }

    private void newThreadmark(string _title, string _link, DateTime date)
    {
        threadmarks.Add(new Threadmark(title, _title, _link, date));
    }

    public List<Threadmark> getThreadmarks()
    {
        return threadmarks;
    }

    public string getTitle()
    {
        return title;
    }

    public string getRSS()
    {
        return rssLink;
    }

    public void Visit()
    {
        Application.OpenURL(threadLink);
    }
}
