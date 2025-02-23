using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameEventListener : MonoBehaviour
{

    [Tooltip("The Event to listen to")]
    public GameEvent Event;

    [Tooltip("The Response once the event is raised")]
    public UnityEvent Response;


    private void OnEnable()
    {
        Event.RegisterGameEventListener(this);
    }

    private void OnDisable()
    {
        Event.UnregisterGameEventListener(this);
    }

    public void OnRaise()
    {
        Response.Invoke();
    }

}
