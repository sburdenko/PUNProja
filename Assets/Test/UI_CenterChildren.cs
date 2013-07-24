//by Joaquin Del Canto

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class UI_CenterChildren : MonoBehaviour {
	
	public enum Direction { None, Horizontal, Vertical, Both }
	
	public Direction direction = Direction.Both;
	public bool everyFrame = true;
	public bool repositionNow = false;
	
	void Start() {
		
		Recenter();
	}
	
	void Update () {
		
		if (everyFrame)
			Recenter();
		
		if (repositionNow) {
			
			repositionNow = false;
			Recenter();
		}
	}
	
	void Recenter() {
		
		if (direction != Direction.None) {
			
			Vector3 boundsCenter = NGUIMath.CalculateAbsoluteWidgetBounds(transform).center;
			
			if (direction == Direction.Horizontal || direction == Direction.Both) {
				
				float differenceHorizontal = transform.position.x - boundsCenter.x;
				
				foreach (Transform child in transform) {
					
					child.Translate(differenceHorizontal, 0f, 0f, Space.World);
				}
			}
			
			if (direction == Direction.Vertical || direction == Direction.Both) {
				
				float differenceVertical = transform.position.y - boundsCenter.y;
				
				foreach (Transform child in transform) {
					
					child.Translate(0f, differenceVertical, 0f, Space.World);
				}
			}
		}
	}
}
