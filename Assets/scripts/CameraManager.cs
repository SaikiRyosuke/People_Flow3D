using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	
	Vector3 direction = new Vector3(0,0,1);
	Vector3 right = new Vector3(1,0,0);
	Vector3 up = new Vector3(0,1,0);
	public float turnSideSpeed = 2f;
	public float turnUpSpeed = 4f;
	public float moveSpeed = 80f;
	public float jumpSpeed = 30f;
	float mouseX, mouseY;
	Quaternion originalQua;

	
    void Start()
    {
        direction = new Vector3(0,0,1);
		right = new Vector3(1,0,0);
		up = new Vector3(0,1,0);
		mouseX = Input.GetAxis("Mouse X");
		mouseY = Input.GetAxis("Mouse Y");
		originalQua = this.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        Quaternion quaternionX = Quaternion.AngleAxis((Input.GetAxis("Mouse X")-mouseX) * turnSideSpeed, Vector3.up);
		Quaternion quaternionY = Quaternion.AngleAxis((Input.GetAxis("Mouse Y")-mouseY) * turnUpSpeed, -right);
		direction = quaternionX * quaternionY * direction;
		right = quaternionX * right;
		up = Vector3.Normalize(Vector3.Cross(right, direction));
		//カメラの向きを変える処理。
		this.transform.rotation =quaternionX * quaternionY * this.transform.rotation;
		//カメラの位置を変える処理。
		if(Input.GetKey(KeyCode.W))	this.transform.position += direction * moveSpeed * Time.deltaTime;
		if(Input.GetKey(KeyCode.A))	this.transform.position += -right * moveSpeed * Time.deltaTime;
		if(Input.GetKey(KeyCode.S))	this.transform.position += -direction * moveSpeed * Time.deltaTime;
		if(Input.GetKey(KeyCode.D))	this.transform.position += right * moveSpeed * Time.deltaTime;
		if(Input.GetKey(KeyCode.Space)) this.transform.position += Vector3.up * jumpSpeed * Time.deltaTime;
		
		if(Input.GetKeyDown(KeyCode.LeftShift)){
			mouseX = Input.GetAxis("Mouse X");
			mouseY = Input.GetAxis("Mouse Y");
			this.transform.rotation = originalQua;
			direction = new Vector3(0,0,1);
			right = new Vector3(1,0,0);
		}
    }
}
