using System;
using System.Reflection;
using UnityEngine;

public abstract class PerformanceState
{
	//meta
	public string stateName;


	//refs
	public PerformanceSM stateMachine;
	//=//-----|Flow Control|------------------------------------------//=//

	[Header("Parameters")] 
	public Enum exitState; 
	public bool clearOnSetState;
	public bool forceClearStateHeapOnEntry;

	[Header("Variables")]
	protected float stateEntryTimeStamp;
	public int priority;


	public PerformanceState(PerformanceSM sm)
	{
		this.stateName = GetType().Name;
		this.stateMachine = sm;
	}



    public virtual void Enter()
    {

    }
    public virtual void Exit()
    {

    }


    public virtual void Update()
    {

    }
    public virtual void FixedUpdate()
    {

    }
    public virtual void LateUpdate()
    {

    }

	//=//-----|Get & Set|---------------------------------------------//=//
	public int GetPriority()
	{
		return priority;
	}



	//=//-----|Debug|---------------------------------------------//=//
	public virtual bool VerifyState()
	{
		return CheckStateForNullFields();
	}
	protected bool CheckStateForNullFields()
	{
		bool passed = true;

		Type type = this.GetType();

		while (type != null && type != typeof(object))
		{
			FieldInfo[] fields = type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.DeclaredOnly);

			foreach (FieldInfo field in fields)
			{
				object value = field.GetValue(this);
				if (value == null)
				{
					passed = false;
					string message = $"Field {field.Name} is null, member of state {type.Name} of the state heirarchy.";
					LogCore.Log("CriticalError", message);
				}
			}

			type = type.BaseType;
		}

		return passed;
	}
}
