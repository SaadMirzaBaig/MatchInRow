using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BroadcastManager {

    public static UnityEvent ArrangeGrid = new UnityEvent();

    public static UnityEvent InitializeGame = new UnityEvent();

    public static UnityEvent ClearGrid = new UnityEvent();


}