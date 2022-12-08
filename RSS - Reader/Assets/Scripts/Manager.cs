using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.UI;

public class Manager : MonoBehaviour
{

    private string saveFile = "saveFile.txt";
    private List<Thread> threads = new List<Thread>();
    public bool displayRead = false;
    private List<GameObject> threadmarkObjects = new List<GameObject>();
    private List<GameObject> threadObjects = new List<GameObject>();
    private bool allThreads = true;
    private Thread selectedThread;

    public GameObject threadmarkObjectPrefab;
    public Transform threadmarkParentTransform;
    public GameObject headerText;
    public GameObject pullout;

    public GameObject threadObjectPrefab;
    public Transform threadParentTransform;
    public GameObject readImage;
    public GameObject unreadImage;
    public GameObject options;
    public GameObject refreshButton;
    public GameObject visitThreadButton;
    public GameObject deleteButton;
    public GameObject deleteCheck;

    public GameObject textFieldPrefab;
    public GameObject addFeedPanel;
    public Transform searchPanelTransform;
    private GameObject textField;

    public GameObject errorCheck;


    // Start is called before the first frame update
    void Start()
    {
        LoadData();
        Refresh();
    }

    private void LoadData()
    {
        if (File.Exists(saveFile))
        {
            StreamReader sr = new StreamReader(saveFile);
            string line = sr.ReadLine();
            while (line != null)
            {
                ThreadSave ts = JsonUtility.FromJson<ThreadSave>(line);
                threads.Add(new Thread(ts));
                line = sr.ReadLine();
            }
            sr.Close();
        }
    }

    private void SaveData()
    {
        string path = Path.Combine(Environment.CurrentDirectory, saveFile);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        string json = "";
        foreach (Thread thread in threads)
        {
            json += JsonUtility.ToJson(thread.save()) + "\n";
        }
        StreamWriter sw = new StreamWriter(saveFile);
        sw.Write(json);
        sw.Close();
    }

    public void Refresh()
    {
        foreach (Thread thread in threads)
        {
            thread.checkForUpdates();
        }
        SaveData();
        Display();
    }

    public void AddThread(string rssLink)
    {
        bool exists = false;
        foreach (Thread thread in threads)
        {
            if (rssLink.Equals(thread.getRSS()))
            {
                exists = true;
                break;
            }
        }
        if (!exists)
        {
            AddThread(new Thread(rssLink));
        }
        Refresh();
    }

    private void AddThread(Thread t)
    {
        List<Thread> sortedThreads = new List<Thread>();

        bool added = false;

        foreach (Thread thread in threads)
        {
            if (t.getTitle().CompareTo(thread.getTitle()) < 0 && added == false)
            {
                sortedThreads.Add(t);
                sortedThreads.Add(thread);
                added = true;
            }else if (t.getTitle().CompareTo(thread.getTitle()) >= 0 || added)
            {
                sortedThreads.Add(thread);
            }
        }

        if (!added)
        {
            sortedThreads.Add(t);
        }

        threads = sortedThreads;
    }

    private void SortThreads()
    {
        List<Thread> sortedThreads = new List<Thread>();

        while (threads.Count > 0)
        {
            string first = threads[0].getTitle();
            Thread firstT = threads[0];

            foreach (Thread thread in threads)
            {
                string temp = thread.getTitle();
                if (first.CompareTo(temp) < 0)
                {
                    first = temp;
                    firstT = thread;
                }
            }
            sortedThreads.Add(firstT);
            threads.Remove(firstT);
        }
        threads = sortedThreads;
    }

    private void Display()
    {
        if (threadObjects.Count > 0)
        {
            foreach (GameObject obj in threadObjects)
            {
                Destroy(obj);
            }
            threadObjects.Clear();
        }

        foreach (Thread thread in threads)
        {
            GameObject go = Instantiate(threadObjectPrefab, threadParentTransform);
            GameObject but = go.transform.GetChild(0).gameObject;
            but.transform.GetChild(0).GetComponent<Text>().text = thread.getTitle();
            but.GetComponent<ThreadButton>().thread = thread;
            threadObjects.Add(go);
        }

        if (threadmarkObjects.Count > 0)
        {
            foreach (GameObject obj in threadmarkObjects)
            {
                Destroy(obj);
            }
            threadmarkObjects.Clear();
        }

        if (allThreads)
        {
            DisplayAll();
        }
        else
        {
            DisplaySelectThread(selectedThread);
        }
    }

    private void DisplaySelectThread(Thread thread)
    {
        headerText.GetComponent<Text>().text = thread.getTitle();

        List<Threadmark> marks = new List<Threadmark>();

        foreach (Threadmark tm in thread.getThreadmarks())
        {
            if (tm.getRead() == null || displayRead)
            {
                marks.Add(tm);
            }
        }

        marks = sortThreadmarks(marks);

        foreach (Threadmark tm in marks)
        {
            GameObject go = Instantiate(threadmarkObjectPrefab, threadmarkParentTransform);
            GameObject but = go.transform.GetChild(0).GetChild(0).gameObject;
            but.transform.GetChild(0).GetComponent<Text>().text = tm.getTitle();
            but.transform.GetChild(1).GetComponent<Text>().text = tm.getThread();
            but.transform.GetChild(2).GetComponent<Text>().text = tm.getDateTime().ToString();
            but.GetComponent<ThreadmarkButton>().threadmark = tm;
            threadmarkObjects.Add(go);
        }
    }

    private void DisplayAll()
    {
        headerText.GetComponent<Text>().text = "All Feeds";

        List<Threadmark> marks = new List<Threadmark>();

        foreach (Thread thread in threads)
        {
            foreach (Threadmark tm in thread.getThreadmarks())
            {
                if (tm.getRead() == null || displayRead)
                {
                    marks.Add(tm);
                }
            }
        }

        marks = sortThreadmarks(marks);

        foreach (Threadmark tm in marks)
        {
            GameObject go = Instantiate(threadmarkObjectPrefab, threadmarkParentTransform);
            GameObject but = go.transform.GetChild(0).GetChild(0).gameObject;
            but.transform.GetChild(0).GetComponent<Text>().text = tm.getTitle();
            but.transform.GetChild(1).GetComponent<Text>().text = tm.getThread();
            but.transform.GetChild(2).GetComponent<Text>().text = tm.getDateTime().ToString();
            but.GetComponent<ThreadmarkButton>().threadmark = tm;
            threadmarkObjects.Add(go);
        }
    }

    private List<Threadmark> sortThreadmarks(List<Threadmark> threadmarks)
    {
        List<Threadmark> sortedThreadmarks = new List<Threadmark>();

        while (threadmarks.Count > 0)
        {
            DateTime min = threadmarks[0].getDateTime();
            Threadmark minT = threadmarks[0];

            foreach (Threadmark tm in threadmarks)
            {
                DateTime temp = tm.getDateTime();
                if (min.CompareTo(temp) > 0)
                {
                    min = temp;
                    minT = tm;
                }
            }

            sortedThreadmarks.Add(minT);
            threadmarks.Remove(minT);
        }

        return sortedThreadmarks;
    }

    public void SelectThread(Thread thread)
    {
        selectedThread = thread;
        allThreads = false;
        TogglePullout();
        Refresh();
    }

    public void SelectAllThreads()
    {
        allThreads = true;
        TogglePullout();
        Refresh();
    }

    public void TogglePullout()
    {
        pullout.SetActive(!pullout.activeSelf);
    }

    public void MarkRead()
    {
        foreach (GameObject go in threadmarkObjects)
        {
            go.transform.GetChild(0).GetChild(0).gameObject.GetComponent<ThreadmarkButton>().threadmark.markRead();
        }
        Refresh();
    }

    public void ToggleDisplayRead()
    {
        displayRead = !displayRead;
        readImage.SetActive(displayRead);
        unreadImage.SetActive(!displayRead);
        Refresh();
    }

    public void ToggleOptions()
    {
        deleteButton.SetActive(!allThreads);
        visitThreadButton.SetActive(!allThreads);
        refreshButton.SetActive(!options.activeSelf);
        options.SetActive(!options.activeSelf);
    }

    public void ToggleDeleteThreadCheck()
    {
        deleteCheck.SetActive(!deleteCheck.activeSelf);
        options.SetActive(false);
    }

    public void DeleteThreadButton()
    {
        threads.Remove(selectedThread);
        ToggleDeleteThreadCheck();
        allThreads = true;
        Refresh();
    }

    public void VisitThread()
    {
        selectedThread.Visit();
        ToggleOptions();
    }

    public void RefreshThreads()
    {
        Refresh();
        ToggleOptions();
    }

    public void ToggleAddFeed()
    {
        addFeedPanel.SetActive(!addFeedPanel.activeSelf);

        if (textField == null)
        {
            textField = Instantiate(textFieldPrefab, searchPanelTransform);
        }
        else
        {
            Destroy(textField);
            textField = null;
        }
        errorCheck.SetActive(false);
    }

    public void SearchForNewFeed()
    {
        errorCheck.SetActive(true);
        Text tText = textField.transform.GetChild(textField.transform.childCount - 1).gameObject.GetComponent<Text>();
        tText.horizontalOverflow = HorizontalWrapMode.Wrap;
        string text = tText.text;
        tText.horizontalOverflow = HorizontalWrapMode.Overflow;
        errorCheck.GetComponent<Text>().text = text;
        if (!text.Equals(""))
        {
            AddThread(text);
        }
        ToggleAddFeed();
    }
}
