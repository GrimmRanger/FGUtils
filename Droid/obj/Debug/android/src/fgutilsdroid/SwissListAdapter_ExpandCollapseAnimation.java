package fgutilsdroid;


public class SwissListAdapter_ExpandCollapseAnimation
	extends android.view.animation.Animation
	implements
		mono.android.IGCUserPeer
{
	static final String __md_methods;
	static {
		__md_methods = 
			"n_applyTransformation:(FLandroid/view/animation/Transformation;)V:GetApplyTransformation_FLandroid_view_animation_Transformation_Handler\n" +
			"";
		mono.android.Runtime.register ("FGUtilsDroid.SwissListAdapter/ExpandCollapseAnimation, FGUtilsDroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", SwissListAdapter_ExpandCollapseAnimation.class, __md_methods);
	}


	public SwissListAdapter_ExpandCollapseAnimation ()
	{
		super ();
		if (getClass () == SwissListAdapter_ExpandCollapseAnimation.class)
			mono.android.TypeManager.Activate ("FGUtilsDroid.SwissListAdapter/ExpandCollapseAnimation, FGUtilsDroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "", this, new java.lang.Object[] {  });
	}


	public SwissListAdapter_ExpandCollapseAnimation (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == SwissListAdapter_ExpandCollapseAnimation.class)
			mono.android.TypeManager.Activate ("FGUtilsDroid.SwissListAdapter/ExpandCollapseAnimation, FGUtilsDroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Content.Context, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:Android.Util.IAttributeSet, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065", this, new java.lang.Object[] { p0, p1 });
	}

	public SwissListAdapter_ExpandCollapseAnimation (android.view.View p0, int p1)
	{
		super ();
		if (getClass () == SwissListAdapter_ExpandCollapseAnimation.class)
			mono.android.TypeManager.Activate ("FGUtilsDroid.SwissListAdapter/ExpandCollapseAnimation, FGUtilsDroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", "Android.Views.View, Mono.Android, Version=0.0.0.0, Culture=neutral, PublicKeyToken=84e04ff9cfb79065:FGUtilsDroid.SwissListAdapter/ExpandCollapseAnimation/ExpandCollapseAnimationType, FGUtilsDroid, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null", this, new java.lang.Object[] { p0, p1 });
	}


	public void applyTransformation (float p0, android.view.animation.Transformation p1)
	{
		n_applyTransformation (p0, p1);
	}

	private native void n_applyTransformation (float p0, android.view.animation.Transformation p1);

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
