﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour {

	public Transform corner1;
	public Transform corner2;
	public bool generateFromCollider = false;

	bool groundedCurrentFrame;
	bool ledgeStepCurrentFrame;

	void Start() {
		if (generateFromCollider) {
			BoxCollider2D bc = GetComponent<BoxCollider2D>();
			Vector2 center = bc.offset;
			float radiusX = bc.bounds.extents.x;
			float radiusY = bc.bounds.extents.y;
			
			corner1 = new GameObject().transform;
			corner1.name = "corner1";
			corner1.transform.parent = this.transform;
			corner1.localPosition = center - new Vector2(-radiusX, radiusY+0.02f);
			
			corner2 = new GameObject().transform;
			corner2.name = "corner2";
			corner2.transform.parent = this.transform;
			corner2.localPosition = center - new Vector2(radiusX, radiusY+0.02f);
		}
	}

	bool LeftGrounded() {
		Debug.DrawLine(corner1.position + Vector3.up * 0.01f, corner1.position);
		return Physics2D.Linecast(transform.position, corner1.position, 1 << LayerMask.NameToLayer(Layers.Ground));
	}

	bool RightGrounded() {
		Debug.DrawLine(corner2.position + Vector3.up * 0.01f, corner2.position);
		return Physics2D.Linecast(transform.position, corner2.position, 1 << LayerMask.NameToLayer(Layers.Ground));
	}

	public bool IsGrounded() {
		return LeftGrounded() || RightGrounded();
	}

	public bool OnlyGroundedOneSide() {
		return LeftGrounded() ^ RightGrounded();
	}

	void Update() {
		bool groundedLastFrame = groundedCurrentFrame;
		groundedCurrentFrame = IsGrounded();
		if (!groundedLastFrame && groundedCurrentFrame) {
			GetComponent<Entity>().OnGroundHit();	
		} else if (groundedLastFrame && !groundedCurrentFrame) {
			GetComponent<Entity>().OnGroundLeave();
		}

		if (GetComponent<PlayerController>() != null) {
			bool ledgeStepLastFrame = ledgeStepCurrentFrame;
			ledgeStepCurrentFrame = OnlyGroundedOneSide();
			if (!ledgeStepLastFrame && ledgeStepCurrentFrame) {
				GetComponent<PlayerController>().OnLedgeStep();
			}
		}
	}

	public bool TouchingPlatform() {
		int layerMask = 1 << LayerMask.NameToLayer(Layers.Ground);
		RaycastHit2D g1 = Physics2D.Raycast(corner1.position + new Vector3(0, .2f), Vector3.down, 1f, layerMask);
		RaycastHit2D g2 = Physics2D.Raycast(corner2.position + new Vector3(0, .2f), Vector3.down, 1f, layerMask);
		if (g1.transform == null && g2.transform == null) {
			return false;
		}
		bool grounded1 = false;
		bool grounded2 = false;
		
		if (g1.transform != null) {
			grounded1 = g1.transform.gameObject.GetComponent<PlatformEffector2D>() != null;
		}
		if (g2.transform != null) {
			grounded2 = g2.transform.gameObject.GetComponent<PlatformEffector2D>() != null;
		}
		
		return grounded1 || grounded2;
	}
}
