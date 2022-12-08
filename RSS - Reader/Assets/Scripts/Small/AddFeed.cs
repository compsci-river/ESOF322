using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AddFeed : MonoBehaviour
{

    private Manager manager;

    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Manager").GetComponent<Manager>();
    }

    public void ToggleAddFeed()
    {
        //Debug.Log("Toggle add feed");
        manager.ToggleAddFeed();
    }

    public void SearchForFeed()
    {
        //Debug.Log("search");
        manager.SearchForNewFeed();
    }

    public void Add()
    {

    }

    public void Cancel()
    {

    }
}
