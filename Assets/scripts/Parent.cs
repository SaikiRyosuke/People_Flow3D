using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parent : MonoBehaviour
{
    public Type type = Type.A;
	public GameObject people;
	
	void Start()
	{
		GameObject obj;
		Vector3 pos = new  Vector3(0,0,0);
		for(int i= 0; i< Parameters.Instance.numberOfPeople; i++){
			if(type == Type.A)	pos = new Vector3( Random.Range(-Parameters.Instance.Lx, Parameters.Instance.Lx), Random.Range(-Parameters.Instance.Ly, Parameters.Instance.Ly), Random.Range(-Parameters.Instance.Lz, Parameters.Instance.Lz));
			else if(type == Type.B)	pos = new Vector3( Random.Range(-Parameters.Instance.Lx,Parameters.Instance.Lx), Random.Range(-Parameters.Instance.Ly, Parameters.Instance.Ly), Random.Range(-Parameters.Instance.Lz, Parameters.Instance.Lz));
			obj = Instantiate(people, pos, Quaternion.identity);
			obj.transform.parent = this.transform;
			obj.transform.GetComponent<People>().pos = obj.transform.position;
			PeopleManager.Instance.AddPeople(obj.transform.GetComponent<People>());

			
		}
	}
	
}
