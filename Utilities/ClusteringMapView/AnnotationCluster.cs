using System;
using MonoTouch.MapKit;
using System.Collections.Generic;
using MonoTouch.CoreLocation;

namespace FGUtils
{
	public class AnnotationCluster : MKAnnotation
	{
		private List<MKAnnotation> _annotations;
		public List<MKAnnotation> Annotations {
			get { return _annotations; }
		}

		public override CLLocationCoordinate2D Coordinate {
			get {
				MKAnnotation ann = (MKAnnotation)_annotations[0];
				return ann.Coordinate;
			}
			
			set {
				throw new NotSupportedException ();
			}
		}

		public override string Title {
			get {
				return string.Format("POIs {0}", _annotations.Count);
			}
		}

		public AnnotationCluster (MKAnnotation annotation) : base()
		{
			_annotations = new List<MKAnnotation>();
			_annotations.Add(annotation);
		}

		public virtual void AddAnnotation(MKAnnotation annotation)
		{
			_annotations.Add(annotation);
		}
	}
}