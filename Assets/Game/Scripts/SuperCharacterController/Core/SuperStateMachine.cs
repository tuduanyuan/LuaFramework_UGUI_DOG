// With a little help from UnityGems
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
/*
    SuperStateMachine则是来自Unity Gems Finite State Machine tutorial的修改版本。
    我的版本则是更加简单，同时也更加强大。
    简单易用的状态机实现对于大部分游戏来说都很重要。
    这个状态机是专门与SuperCharacterController一起使用的。
    角色的状态机逻辑实现在SuperStateMachine子类当中。
    我通常按照”角色名+Machine”的方式使用。
    比如我的Mario64项目中，有MarioMachine、GoombaMachine、BobombMachine，它们都继承自SuperStateMachine。
     */
/// <summary>
/// State machine model that recieves SuperUpdate messages from the SuperCharacterController
/// </summary>
public class SuperStateMachine : MonoBehaviour {
    //进入状态的时间
    protected float timeEnteredState;
    //装填的调用
    public class State
    {
        public Action DoSuperUpdate = DoNothing;
        public Action enterState = DoNothing;
        public Action exitState = DoNothing;

        public Enum currentState;
    }

    public State state = new State();
    //关键就是这个东西
    public Enum currentState
    {
        get
        {
            return state.currentState;
        }
        set
        {
            if (state.currentState == value)
                return;

            ChangingState();
            state.currentState = value;
            ConfigureCurrentState();
        }
    }

    [HideInInspector]
    public Enum lastState;

    void ChangingState()
    {
        lastState = state.currentState;
        timeEnteredState = Time.time;
    }

    /// <summary>
    /// Runs the exit method for the previous state. Updates all method delegates to the new
    /// state, and then runs the enter method for the new state.
    /// </summary>
    void ConfigureCurrentState()
    {
        //调用前一个状态的离开方法
        if (state.exitState != null)
        {
            state.exitState();
        }

        //Now we need to configure all of the methods ，状态赋值
        state.DoSuperUpdate = ConfigureDelegate<Action>("SuperUpdate", DoNothing);
        state.enterState = ConfigureDelegate<Action>("EnterState", DoNothing);
        state.exitState = ConfigureDelegate<Action>("ExitState", DoNothing);
        //新状态的进入方法
        if (state.enterState != null)
        {
            state.enterState();
        }
    }

    Dictionary<Enum, Dictionary<string, Delegate>> _cache = new Dictionary<Enum, Dictionary<string, Delegate>>();

    /// <summary>
    /// Retrieves the specific state method for the provided method root.
    /// 检索提供的方法根的具体状态方法。
    /// 这里通过反射获取的
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="methodRoot">
    /// Based method name that is appended to the state name by an underscore, in the form of X_methodRoot where X is a state name</param>
    /// 基于方法的名称，它以下划线附加到状态名称，形式为X_methodRoot，其中X是状态名称
    /// <param name="Default">默认方法</param>
    /// <returns>The state specific method as a delegate or Default if it does not exist</returns>
    T ConfigureDelegate<T>(string methodRoot, T Default) where T : class
    {

        Dictionary<string, Delegate> lookup;
        if (!_cache.TryGetValue(state.currentState, out lookup))
        {
            //缓存中是否有这个状态的记录，没有就加个新的
            _cache[state.currentState] = lookup = new Dictionary<string, Delegate>();
        }
        Delegate returnValue;
        if (!lookup.TryGetValue(methodRoot, out returnValue))
        {
            //这里在用对象的反射来获取
            var mtd = GetType().GetMethod(state.currentState.ToString() + "_" + methodRoot, System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.InvokeMethod);
            //就是要获取到:状态_methodRoot的方法
            if (mtd != null)
            {
                returnValue = Delegate.CreateDelegate(typeof(T), this, mtd);
            }
            else
            {
                returnValue = Default as Delegate;
            }
            lookup[methodRoot] = returnValue;
        }
        return returnValue as T;

    }

    /// <summary>
    /// Message callback from the SuperCharacterController that runs the state specific update between global updates
    /// 在全局更新之间运行状态特定更新的超级字符控制器的消息回调
    /// </summary>
    void SuperUpdate()
    {
        EarlyGlobalSuperUpdate();

        state.DoSuperUpdate();

        LateGlobalSuperUpdate();
    }

    protected virtual void EarlyGlobalSuperUpdate() { }

    protected virtual void LateGlobalSuperUpdate() { }

    static void DoNothing() { }
}
