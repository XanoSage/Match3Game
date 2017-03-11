using System;

public interface IPresenter
{
	int LinkPriority { get; }
	void Init(object target);
}

public interface IPresenter<TTarget> : IPresenter
{
	void Init(TTarget controller);
}

public interface IControllable
{
	object Controller { get; }
}

public abstract class BehaviourPresenter : UnityEngine.MonoBehaviour, IPresenter
{
	public virtual int LinkPriority { get { return 0; } }

	void IPresenter.Init(object target)
	{
		InitRaw(target);
	}

	protected abstract void InitRaw(object obj);
}

public abstract class BehaviourPresenter<TTarget> : BehaviourPresenter, IPresenter<TTarget>
	where TTarget : class
{
	public abstract void Init(TTarget controller);

	protected sealed override void InitRaw(object obj)
	{
		var target = obj as TTarget;
		if (target != null)
			Init(target);
	}
}

// Depends on declareted above interfaces
public static class PresentersExtensions
{
	public static void InitPresentersStrict<T>(this UnityEngine.GameObject gameObject, T target)
	{
		var presenters = gameObject.GetComponentsInChildren<IPresenter<T>>();
		Array.Sort(presenters, (p1, p2) => p1.LinkPriority.CompareTo(p2.LinkPriority));

		for (int i = 0; i < presenters.Length; ++i)
		{
			var p = presenters[i];
			p.Init(target);
		}
	}

	public static void InitPresenters(this UnityEngine.GameObject gameObject, object target)
	{
		var presenters = gameObject.GetComponentsInChildren<IPresenter>();
		Array.Sort(presenters, (p1, p2) => p1.LinkPriority.CompareTo(p2.LinkPriority));

		for (int i = 0; i < presenters.Length; ++i)
		{
			var p = presenters[i];
			p.Init(target);
		}
	}

	public static void InitPresentersDeep<T>(this UnityEngine.GameObject gameObject, T target)
	{
		gameObject.InitPresenters(target);

		var c = target as IControllable;
		if (c == null)
			return;

		var controller = c.Controller;
		if (ReferenceEquals(controller, c))
			return;

		gameObject.InitPresenters(controller);
	}
}



