using UnityEngine;
using System.Collections;
using Tools.Data;
/// <summary>
///发送单一事件。
/// </summary>
public class SendEventCollider : MonoBehaviour
{
    
    public System.Action<EventListener, GameObject, object, object> ButtonToolAction;

    private object callData;

    private Dictionary data = new Dictionary();

    internal  string dataKey = "data";

    internal string gameobjectKey = "go";

    public GameObject target;

    public string callMethod = "ButtonAction";

   

    public EventListener eventListener=EventListener.onClick;

    public object CallData {

        get { return callData; }
        set { callData = value; }
    }

    public void initData(GameObject go, object callData, string callMethod, EventListener eventListener)
    {

            target = go;

            if (target == null)
                target = gameObject;

            this.data.Add(gameobjectKey, gameObject);

            if (callData != null)
            {
                if (this.data.ContainsKey(dataKey) == false)
                    this.data.Add(dataKey, data);
                else
                    this.data[dataKey] = data;

            }
            if (string.IsNullOrEmpty(callMethod) == false)
                this.callMethod = callMethod;
        if(eventListener !=EventListener.None)
             this.eventListener = eventListener;
    }

    public void initData( EventListener eventListener,object callData, System.Action<EventListener, GameObject, object, object> ButtonToolAction)
    {
        CallData = callData;

        if (ButtonToolAction != this.ButtonToolAction)
         this.ButtonToolAction = ButtonToolAction;

        if (  this.eventListener != eventListener)
        this.eventListener = eventListener;

    }
    public void SetCallData(object  data)
    {
        this.CallData = data;
    }
    void Start()
    {
        if (target == null)
            target = gameObject;
    }
    /// <summary>
    /// 释放
    /// </summary>
    public void Release()
    {
        if (callData != null)
            callData = null;
        if (ButtonToolAction != null)
            ButtonToolAction = null;
        if (string.IsNullOrEmpty(callMethod) == false)
            callMethod = string.Empty;
        if (target != null)
            target = null;
    }

    void OnSubmit()
    {
        if (eventListener==EventListener.onSubmit && enabled)
        {
            if (ButtonToolAction != null)
                ButtonToolAction(eventListener,gameObject, callData, null);
            else
                target.SendMessage(callMethod, data, SendMessageOptions.DontRequireReceiver);
        }
    }
    void OnClick()
    {

        if (eventListener == EventListener.onClick && enabled)
        {


            if (this.ButtonToolAction != null)
            
            {
                ButtonToolAction(eventListener, gameObject, callData, null);
            }

            else
            {
                if(target!=null && string.IsNullOrEmpty(callMethod)==false)
                    target.SendMessage(callMethod, data, SendMessageOptions.DontRequireReceiver);
            }
            
        }
    }
    void OnDoubleClick()
    {
        if (eventListener == EventListener.onDoubleClick && enabled)
        {
            if (ButtonToolAction != null)
                ButtonToolAction(eventListener,gameObject, callData, null);
            else
                target.SendMessage(callMethod, data, SendMessageOptions.DontRequireReceiver);
        }
    }
    void OnHover(bool isOver)
    {
        if (eventListener == EventListener.onHover && enabled)
        {
       
            if (ButtonToolAction != null)
            {
                ButtonToolAction(eventListener,gameObject,callData,isOver);
            }

            else 
            {
                if (data.ContainsKey(eventListener.ToString()) == false)
                {
                    data.Add(eventListener.ToString(), isOver);
                }
                else
                {
                    data[eventListener.ToString()] = isOver;
                }
                target.SendMessage(callMethod, data, SendMessageOptions.DontRequireReceiver);
            }
               
        }
    }
    void OnPress(bool isPressed)
    {
        if (eventListener == EventListener.onPress && enabled)
        {
        
            if (ButtonToolAction != null)
                ButtonToolAction(eventListener,gameObject,callData,isPressed);
            else
            {

                if (data.ContainsKey(eventListener.ToString()) == false)
                {
                    data.Add(eventListener.ToString(), isPressed);
                }
                else
                {
                    data[eventListener.ToString()] = isPressed;
                }
                target.SendMessage(callMethod, data, SendMessageOptions.DontRequireReceiver);
            }
              
        }
    }
    void OnSelect(bool selected)
    {
        if (eventListener == EventListener.onSelect && enabled)
        {

            if (ButtonToolAction != null)
                ButtonToolAction(eventListener,gameObject,callData,selected);
            else 
            {
                if (data.ContainsKey(eventListener.ToString()) == false)
                {
                    data.Add(eventListener.ToString(), selected);
                }
                else
                {
                    data[eventListener.ToString()] = selected;
                }
                target.SendMessage(callMethod, data, SendMessageOptions.DontRequireReceiver);
            }
              
        }
    }
    void OnScroll(float delta)
    {
        if (eventListener == EventListener.onScroll && enabled)
        {


            if (ButtonToolAction != null)
                ButtonToolAction(eventListener,gameObject,callData,delta);
            else 
            {
                if (data.ContainsKey(eventListener.ToString()) == false)
                {
                    data.Add(eventListener.ToString(), delta);
                }
                else
                {
                    data[eventListener.ToString()] = delta;
                }
                target.SendMessage(callMethod, data, SendMessageOptions.DontRequireReceiver);
            }
             
        }
    }
    void OnDrag(Vector2 delta)
    {
        if (eventListener == EventListener.onDrag && enabled)
        {
          

            if (ButtonToolAction != null){

                ButtonToolAction(eventListener,gameObject,callData,delta);
            }
              
            else
            {
                if (data.ContainsKey(eventListener.ToString()) == false)
                {
                    data.Add(eventListener.ToString(), delta);
                }
                else
                {
                    data[eventListener.ToString()] = delta;
                }
                target.SendMessage(callMethod, data, SendMessageOptions.DontRequireReceiver);
            }
               
        }
    }
    void OnDragOver()
    {
        if (eventListener == EventListener.onDragOver && enabled)
        {
            if (ButtonToolAction != null)
                ButtonToolAction(eventListener,gameObject, callData, null);
            else
                target.SendMessage(callMethod, data, SendMessageOptions.DontRequireReceiver);
        }
    }
    void OnDragOut()
    {
        if (eventListener == EventListener.onDragOut && enabled)
        {
            if (ButtonToolAction != null)
                ButtonToolAction(eventListener,gameObject, callData, null);
            else
                target.SendMessage(callMethod, data, SendMessageOptions.DontRequireReceiver);
        }
    }
    void OnDrop(GameObject go)
    {
        if (eventListener == EventListener.onDrop && enabled)
        {

            if (ButtonToolAction != null)
                ButtonToolAction(eventListener,gameObject, callData, go);
            else 
            {
                if (data.ContainsKey(eventListener.ToString()) == false)
                {
                    data.Add(eventListener.ToString(), go);
                }
                else
                {
                    data[eventListener.ToString()] = go;
                }
                target.SendMessage(callMethod, data, SendMessageOptions.DontRequireReceiver);
            }
               
        }
    }
    void OnKey(KeyCode key)
    {
        if (eventListener == EventListener.onKey && enabled)
        {

            if (ButtonToolAction != null)
                ButtonToolAction(eventListener,gameObject, callData, key);
            else
            {
                if (data.ContainsKey(eventListener.ToString()) == false)
                {
                    data.Add(eventListener.ToString(), key);
                }
                else
                {
                    data[eventListener.ToString()] = key;
                }
                target.SendMessage(callMethod, data, SendMessageOptions.DontRequireReceiver);
            }
               
        }
    }

    void OnDragStart()
    {
        if (eventListener == EventListener.onDragStart && enabled)
        {

            if (ButtonToolAction != null)
                ButtonToolAction(eventListener,gameObject, callData, null);
            else
            {
             
                target.SendMessage(callMethod, data, SendMessageOptions.DontRequireReceiver);
            }

        }
    }
    void OnTooltip(bool show) 
    {
        if (eventListener == EventListener.onTooltip && enabled)
        {

            if (ButtonToolAction != null)
                ButtonToolAction(eventListener,gameObject, callData, show);
            else
            {
                if (data.ContainsKey(eventListener.ToString()) == false)
                {
                    data.Add(eventListener.ToString(), show);
                }
                else
                {
                    data[eventListener.ToString()] = show;
                }
                target.SendMessage(callMethod, data, SendMessageOptions.DontRequireReceiver);
            }

        }
    }
}

public enum EventListener
{
    None, onSubmit, onClick, onDoubleClick, onHover, onPress, onSelect, onScroll, onDragStart, onDrag, onDragOver,
    onDragOut, onDragEnd, onDrop, onKey, onTooltip
}
