using System;
using MonoTouch.CoreLocation;
using MonoTouch.MapKit;
using MonoTouch.Foundation;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;

namespace FGUtils
{
	[Register("ClusteringMapView")]
	public partial class ClusteringMapView : MKMapView
	{
		public IClusteringMapDelegate ClusterDelegate { get; set; }

		public bool ClusteringEnabled { get; set; }
		public double MinLatDeltaForClustering { get; set; }
		public double ClusterSize { get; set; }

		private List<MKAnnotation> _allAnnotations;
		public new List<MKAnnotation> Annotations { get { return _allAnnotations; } }

		public NSObject[] VisibleAnnotations { get { return base.Annotations; } }

		public ClusteringMapView () : base() { Initialize(); }
		public ClusteringMapView(IntPtr handle) : base(handle) { Initialize(); }
		public ClusteringMapView(RectangleF frame) : base(frame) { Initialize(); }

		private void Initialize()
		{
			_allAnnotations = new List<MKAnnotation>();
			ClusteringEnabled = true;
			MinLatDeltaForClustering = 0.0;
			ClusterSize = 0.1;
		}

		public void SetAnnotations(MKAnnotation[] anns)
		{
			_allAnnotations = new List<MKAnnotation>(anns);
			PopulateMap();
		}

		public new void AddAnnotation(MKAnnotation ann)
		{
			_allAnnotations.Add(ann);
			PopulateMap();
		}

		public new void AddAnnotations(MKAnnotation[] anns)
		{
			_allAnnotations.AddRange(anns);
			PopulateMap();
		}

		public void RemoveAnnotation(MKAnnotation ann)
		{
			_allAnnotations.Remove(ann);
			PopulateMap();
		}

		public void RemoveAnnotations(MKAnnotation[] anns)
		{
			foreach (MKAnnotation ann in anns)
			{
				_allAnnotations.Remove(ann);
			}
			PopulateMap();
		}

		private void PopulateMap()
		{	
			List<MKAnnotation> tempList = 
				(from a in _allAnnotations
				 where (a as MKAnnotation) != null
				 select a).ToList();
			
			List<MKAnnotation> toAdd = GetVisibleAnnotations(tempList);

			if (ClusterDelegate != null && ClusteringEnabled && this.Region.Span.LatitudeDelta > MinLatDeltaForClustering)
			{
				DropClusteredAnnotations(toAdd);
			}
			else
			{
				DropAnnotations(toAdd);
			}
		}

		private void DropAnnotations(List<MKAnnotation> anns)
		{
			List<MKAnnotation> toRemove = 
				(from a in this.Annotations
				 where (a as MKAnnotation) != null
				 select a).ToList();

			foreach(MKAnnotation ann in anns)
			{
				toRemove.Remove(ann);
			}

			base.RemoveAnnotations(toRemove.ToArray());
			base.AddAnnotations(anns.ToArray());
		}

		private void DropClusteredAnnotations(List<MKAnnotation> anns)
		{
			List<MKAnnotation> clustered = new List<MKAnnotation>();

			foreach(MKAnnotation ann in anns)
			{
				bool shouldAdd = true;
				foreach(MKAnnotation ann2 in clustered)
				{
					if (LocationsAreProximate(ann.Coordinate, ann2.Coordinate, this.Region.Span.LatitudeDelta * ClusterSize))
					{
						shouldAdd = false;

						if (ann2 is AnnotationCluster)
						{
							((AnnotationCluster)ann2).AddAnnotation(ann);
							break;
						}
						else
						{
							clustered.Remove(ann2);
							AnnotationCluster annGroup = ClusterDelegate.GetNewAnnotationCluster(ann2);
							annGroup.AddAnnotation(ann);
							clustered.Add(annGroup);
							break;
						}
					}
				}
				
				if (shouldAdd)
					clustered.Add(ann);
			}

			List<MKAnnotation> toRemove = 
				(from a in this.VisibleAnnotations
				 where (a as MKAnnotation) != null
				 select (MKAnnotation)a).ToList();
			
			foreach(MKAnnotation ann in clustered)
			{
				toRemove.Remove(ann);
			}
			
			base.RemoveAnnotations(toRemove.ToArray());
			base.AddAnnotations(clustered.ToArray());
		}

		private List<MKAnnotation> GetVisibleAnnotations(List<MKAnnotation> toFilter)
		{
			List<MKAnnotation> filtered = new List<MKAnnotation>(toFilter.Count);

			double a = this.Region.Span.LatitudeDelta / 2;
			double b = this.Region.Span.LongitudeDelta / 2;
			double radius = Math.Sqrt(a*a + b*b);

			foreach (MKAnnotation ann in toFilter)
			{
				if (LocationsAreProximate(ann.Coordinate, this.CenterCoordinate, radius))
					filtered.Add(ann);
			}

			return filtered;
		}

		private double Distance(CLLocationCoordinate2D first, CLLocationCoordinate2D second)
		{
			double deltaLat = first.Latitude - second.Latitude;
			double deltaLong = first.Longitude - second.Longitude;

			return Math.Sqrt(deltaLat*deltaLat + deltaLong*deltaLong);
		}

		private bool LocationsAreProximate(CLLocationCoordinate2D first, CLLocationCoordinate2D second, double testDistance)
		{
			bool areProximate = true;

			double distance = Distance(first, second);

			if(distance > testDistance)
				areProximate = false;

			return areProximate;
		}
	}

	public interface IClusteringMapDelegate
	{
		AnnotationCluster GetNewAnnotationCluster(MKAnnotation annotation);
	}
}