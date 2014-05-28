using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {

	string curAnimName;

	// Use this for initialization
	void Start () {
		curAnimName = animation.animation.name;
	}

	void SetAnimation(string name)
	{
		if (name != curAnimName)
		{
			curAnimName = name;
			animation.CrossFade(name);
		}
	}
	
	// Update is called once per frame
	void Update () {
		float move = Input.GetAxis("Horizontal");
		if (move != 0.0f) SetAnimation("walk");
		else SetAnimation("idle");
	}
}
