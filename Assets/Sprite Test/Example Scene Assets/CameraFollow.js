#pragma strict

private var camOffset = Vector3.zero;

var followTarget : Transform;

function Start () {
	camOffset = transform.position - followTarget.position;
}

function Update () {
	transform.position = followTarget.position + camOffset;
}