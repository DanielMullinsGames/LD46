/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// 
/// Creates and manages splines.
/// 
/// </summary>

using UnityEngine;
using System.Collections;
using System;

namespace Pixelplacement
{
	public enum SplineDirection { Forward, Backwards }

	[ExecuteInEditMode]
	public class Spline : MonoBehaviour
	{
		#region Public Events
		public event Action OnSplineShapeChanged;
		#endregion

		#region Public Variables
		public Color color = Color.yellow;
		[Range(0, 1)] public float toolScale = .1f;
		public TangentMode defaultTangentMode;
		public SplineDirection direction;
		public bool loop;
		public SplineFollower[] followers;
		#endregion

		#region Private Variables
		SplineAnchor[] _anchors;
		int _curveCount;
		int _previousAnchorCount;
		int _previousChildCount;
		bool _wasLooping;
		bool _previousLoopChoice;
		bool _anchorsChanged;
		SplineDirection _previousDirection;
		float _curvePercentage = 0;
		int _operatingCurve = 0;
		float _currentCurve = 0;
		[SerializeField, Tooltip("Optimize by removing all renderers in built projects.")] bool _removeRenderers = true;
		[SerializeField, Tooltip("Visualizes how fast or slow a curve segment will be. This operation is expensive so turn it off when not needed.")] bool _showVelocityTicks;
		[SerializeField, Tooltip("How many velocity ticks to show per curve segment.")] int _velocityTickCount = 20;
		[SerializeField, Range(0, 1)] float _velocityTickScale = .5f;
		#endregion

		#region Public Properties
		public SplineAnchor[] Anchors
		{
			get
			{
				//if loop is toggled make sure we reset anchors:
				if (loop != _wasLooping)
				{
					_previousAnchorCount = -1;
					_wasLooping = loop;
				}

				if (!loop)
				{
					if (transform.childCount != _previousAnchorCount || transform.childCount == 0)
					{
						_anchors = GetComponentsInChildren<SplineAnchor>();
						_previousAnchorCount = transform.childCount;
					}

					return _anchors;
				}
				else
				{
					if (transform.childCount != _previousAnchorCount || transform.childCount == 0)
					{
						//for a loop we need an array whose last element is the first element:
						_anchors = GetComponentsInChildren<SplineAnchor>();
						Array.Resize(ref _anchors, _anchors.Length + 1);
						_anchors[_anchors.Length - 1] = _anchors[0];
						_previousAnchorCount = transform.childCount;
					}
					return _anchors;
				}
			}
		}

		public Color SecondaryColor
		{
			get
			{
				Color secondaryColor = Color.Lerp(color, Color.black, .2f);
				return secondaryColor;
			}
		}
		#endregion

		#region Gizmos
		private void OnDrawGizmos ()
		{
			if (_showVelocityTicks)
			{
				for (int i = 0; i < _velocityTickCount; i++)
				{
					float percentage = i / (float)_velocityTickCount;
					Gizmos.color = SecondaryColor;
					Gizmos.DrawSphere(GetPosition(percentage), toolScale * _velocityTickScale);
				}
			}
		}
		#endregion

		#region Init
		void Awake()
		{
			Reset();

			//runtime optimizations:
			if (!Application.isEditor && _removeRenderers)
			{
				foreach (var item in GetComponentsInChildren<MeshFilter>())
				{
					Destroy(item);
				}

				foreach (var item in GetComponentsInChildren<MeshRenderer>())
				{
					Destroy(item);
				}

				foreach (var item in GetComponentsInChildren<SkinnedMeshRenderer>())
				{
					Destroy(item);
				}
			}
		}

		void Reset()
		{
			//if we don't have at least 2 anchors, fix it:
			if (Anchors.Length < 2)
			{
				AddAnchors(2 - Anchors.Length);
			}
		}
		#endregion

		#region Loop
		void Update()
		{
			//place followers (if supplied and something relavent changed):
			if (followers != null && followers.Length > 0 && Anchors.Length >= 2)
			{
				bool needToUpdate = false;

				//was anything else changed?
				if (_anchorsChanged || _previousChildCount != transform.childCount || direction != _previousDirection || loop != _previousLoopChoice)
				{
					_previousChildCount = transform.childCount;
					_previousLoopChoice = loop;
					_previousDirection = direction;
					_anchorsChanged = false;
					needToUpdate = true;
				}

				//were any followers moved?
				for (int i = 0; i < followers.Length; i++)
				{
					if (followers[i].WasMoved || needToUpdate)
					{
						followers[i].UpdateOrientation(this);
					}
				}
			}

			//manage anchors:
			if (Anchors.Length > 1)
			{
				for (int i = 0; i < Anchors.Length; i++)
				{
					//if this spline has changed notify and wipe cached percentage:
					if (Anchors[i].Changed)
					{
						if (OnSplineShapeChanged != null) OnSplineShapeChanged();
						Anchors[i].Changed = false;
						_anchorsChanged = true;
					}

					//if this isn't a loop then the first and last tangents are unnecessary:
					if (!loop)
					{
						//turn first tangent off:
						if (i == 0)
						{
							Anchors[i].SetTangentStatus(false, true);
							continue;
						}

						//turn last tangent off:
						if (i == Anchors.Length - 1)
						{
							Anchors[i].SetTangentStatus(true, false);
							continue;
						}

						//turn both tangents on:
						Anchors[i].SetTangentStatus(true, true);

					}
					else
					{
						//all tangents are needed in a loop:
						Anchors[i].SetTangentStatus(true, true);
					}
				}
			}
		}
		#endregion

		#region Public Methods
		/// <summary>
		/// Get the up vector at a percentage along the spline.
		/// </summary>
		public Vector3 Up(float percentage)
		{
			Quaternion lookRotation = Quaternion.LookRotation(GetDirection(percentage));
			return lookRotation * Vector3.up;
		}

		/// <summary>
		/// Get the right vector at a percentage along the spline.
		/// </summary>
		public Vector3 Right(float percentage)
		{
			Quaternion lookRotation = Quaternion.LookRotation(GetDirection(percentage));
			return lookRotation * Vector3.right;
		}

		/// <summary>
		/// Get the forward vector at a percentage along the spline - this is simply a wrapper for the direction since they are the same thing.
		/// </summary>
		public Vector3 Forward(float percentage)
		{
			return GetDirection(percentage);
		}

		/// <summary>
		/// Returns a facing vector at the given percentage along the spline to allow content to properly orient along the spline.
		/// </summary>
		public Vector3 GetDirection(float percentage)
		{
			//get direction:
			CurveDetail curveDetail = GetCurve(percentage);

			//avoid an error in editor usage where this index can be -1:
			if (curveDetail.currentCurve < 0) return Vector3.zero;

			SplineAnchor startAnchor = Anchors[curveDetail.currentCurve];
			SplineAnchor endAnchor = Anchors[curveDetail.currentCurve + 1];
			return BezierCurves.GetFirstDerivative(startAnchor.Anchor.position, endAnchor.Anchor.position, startAnchor.OutTangent.position, endAnchor.InTangent.position, curveDetail.currentCurvePercentage).normalized;
		}

		/// <summary>
		/// Returns a position on the spline at the given percentage.
		/// </summary>
		public Vector3 GetPosition(float percentage, bool evenDistribution = true, int distributionSteps = 100)
		{
			//evaluate curve:
			CurveDetail curveDetail = GetCurve(percentage);

			//avoid an error in editor usage where this index can be -1:
			if (curveDetail.currentCurve < 0) return Vector3.zero;

			SplineAnchor startAnchor = Anchors[curveDetail.currentCurve];
			SplineAnchor endAnchor = Anchors[curveDetail.currentCurve + 1];
			return BezierCurves.GetPoint(startAnchor.Anchor.position, endAnchor.Anchor.position, startAnchor.OutTangent.position, endAnchor.InTangent.position, curveDetail.currentCurvePercentage, evenDistribution, distributionSteps);
		}

		/// <summary>
		/// Returns a position on the spline at the given percentage with a relative offset.
		/// </summary>
		public Vector3 GetPosition(float percentage, Vector3 relativeOffset, bool evenDistribution = true, int distributionSteps = 100)
		{
			//get position and look rotation:
			Vector3 position = GetPosition(percentage, evenDistribution, distributionSteps);
			Quaternion lookRotation = Quaternion.LookRotation(GetDirection(percentage));

			//get each axis at the current position:
			Vector3 up = lookRotation * Vector3.up;
			Vector3 right = lookRotation * Vector3.right;
			Vector3 forward = lookRotation * Vector3.forward;

			//translate position:
			Vector3 offset = position + right * relativeOffset.x;
			offset += up * relativeOffset.y;
			offset += forward * relativeOffset.z;

			return offset;
		}

		/// <summary>
		/// Given a world point and a number of divisions (think granularity or precision) this returns the closest point on the spline.
		/// </summary>
		public float ClosestPoint(Vector3 point, int divisions = 100)
		{
			//make sure we have at least one division:
			if (divisions <= 0) divisions = 1;

			//variables:
			float shortestDistance = float.MaxValue;
			Vector3 position = Vector3.zero;
			Vector3 offset = Vector3.zero;
			float closestPercentage = 0;
			float percentage = 0;
			float distance = 0;

			//iterate spline and find the closest point on the spline to the provided point:
			for (float i = 0; i < divisions + 1; i++)
			{
				percentage = i / divisions;
				position = GetPosition(percentage);
				offset = position - point;
				distance = offset.sqrMagnitude;

				//if this point is closer than any others so far:
				if (distance < shortestDistance)
				{
					shortestDistance = distance;
					closestPercentage = percentage;
				}
			}

			return closestPercentage;
		}


		/// <summary>
		/// Makes a spline longer.
		/// </summary>
		public GameObject[] AddAnchors(int count)
		{
			//refs:
			GameObject anchorTemplate = Resources.Load("Anchor") as GameObject;

			//if we have no anchors let's make sure we create a spline segment and not just a single anchor:
			if (Anchors.Length == 0) count = 2;

			//create return array:
			GameObject[] returnObjects = new GameObject[count];

			for (int i = 0; i < count; i++)
			{
				//previous anchor refs:
				Transform previousPreviousAnchor = null;
				Transform previousAnchor = null;
				if (Anchors.Length == 1)
				{
					previousPreviousAnchor = transform;
					previousAnchor = Anchors[0].transform;
				}
				else if (Anchors.Length > 1)
				{
					previousPreviousAnchor = Anchors[Anchors.Length - 2].transform;
					previousAnchor = Anchors[Anchors.Length - 1].transform;
				}

				//create a new anchor:
				GameObject newAnchor = Instantiate<GameObject>(anchorTemplate);
				newAnchor.name = newAnchor.name.Replace("(Clone)", "");
				SplineAnchor anchor = newAnchor.GetComponent<SplineAnchor>();
				anchor.tangentMode = defaultTangentMode;
				newAnchor.transform.parent = transform;
				newAnchor.transform.rotation = Quaternion.LookRotation(transform.forward);

				//tilt tangents for variety as we add new anchors:
				//anchor.Tilt (new Vector3 (0, 0, 0));
				anchor.InTangent.Translate(Vector3.up * .5f);
				anchor.OutTangent.Translate(Vector3.up * -.5f);

				//position new anchor:
				if (previousPreviousAnchor != null && previousAnchor != null)
				{
					//determine direction for next placement:
					Vector3 direction = (previousAnchor.position - previousPreviousAnchor.position).normalized;
					if (direction == Vector3.zero) direction = transform.forward;

					//place from the previous anchor in the correct direction:
					newAnchor.transform.position = previousAnchor.transform.position + (direction * 1.5f);
				}
				else
				{
					newAnchor.transform.localPosition = Vector3.zero;
				}

				//catalog this new anchor for return:
				returnObjects[i] = newAnchor;
			}

			return returnObjects;
		}

		/// <summary>
		/// Gets the current curve at the percentage.
		/// </summary>
		public CurveDetail GetCurve(float percentage)
		{
			//clamp or loop percentage:
			if (loop)
			{
				percentage = Mathf.Repeat(percentage, 1);
			}
			else
			{
				percentage = Mathf.Clamp01(percentage);
			}

			//curve identification and evaluation:
			if (Anchors.Length == 2)
			{
				//direction reversed?
				if (direction == SplineDirection.Backwards)
				{
					percentage = 1 - percentage;
				}

				//simply evaluate the curve since there is only one:
				return new CurveDetail(0, percentage);
			}
			else
			{
				//figure out which curve we are operating on from the spline and a percentage along it:
				_curveCount = Anchors.Length - 1;
				_currentCurve = _curveCount * percentage;

				if ((int)_currentCurve == _curveCount)
				{
					_currentCurve = _curveCount - 1;
					_curvePercentage = 1;
				}
				else
				{
					_curvePercentage = _currentCurve - (int)_currentCurve;
				}

				_currentCurve = (int)_currentCurve;
				_operatingCurve = (int)_currentCurve;

				//direction reversed?
				if (direction == SplineDirection.Backwards)
				{
					_curvePercentage = 1 - _curvePercentage;
					_operatingCurve = (_curveCount - 1) - _operatingCurve;
				}

				return new CurveDetail(_operatingCurve, _curvePercentage);
			}
		}
		#endregion
	}
}