package fgutilsdroid;


public abstract class SwissListAdapter
	extends android.widget.BaseAdapter
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("FGUtilsDroid.SwissListAdapter, FGUtilsDroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", SwissListAdapter.class, __md_methods);
	}


	public SwissListAdapter ()
	{
		super ();
		if (getClass () == SwissListAdapter.class)
			mono.android.TypeManager.Activate ("FGUtilsDroid.SwissListAdapter, FGUtilsDroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}

	java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
