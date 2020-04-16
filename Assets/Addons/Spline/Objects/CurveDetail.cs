/// <summary>
/// SURGE FRAMEWORK
/// Author: Bob Berkebile
/// Email: bobb@pixelplacement.com
/// 
/// Holds details of a spline's curve.
/// 
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pixelplacement
{
	public struct CurveDetail
	{
		#region Public Variables
		public int currentCurve;
		public float currentCurvePercentage;
		#endregion

		#region Constructor
		public CurveDetail (int currentCurve, float currentCurvePercentage)
		{
			this.currentCurve = currentCurve;
			this.currentCurvePercentage = currentCurvePercentage;
		}
		#endregion
	}
}