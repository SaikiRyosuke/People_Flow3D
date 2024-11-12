using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parent : MonoBehaviour
{
    public Type type = Type.A;
	public GameObject people;
	public int generateRate = 5;
	
	IEnumerator Start()
	{
		for(int i = 0; i < this.transform.childCount; i++){
			if(type == Type.A)	this.transform.GetChild(i).GetComponent<People>().velocity.x = 1f;
			if(type== Type.B) this.transform.GetChild(i).GetComponent<People>().velocity.x = -1f;
		}
		while(true){
			yield return new WaitForSeconds(0.5f);
			GameObject obj = people;
			for(int i= 0; i< PeopleManager.Instance.generateRate; i++){
				if(type == Type.A)	obj = Instantiate(people, new Vector3(-10,Random.Range(-9,9),Random.Range(-9,9)), Quaternion.identity );
				if(type == Type.B)	obj = Instantiate(people, new Vector3(10,Random.Range(-9,9),Random.Range(-9,9)), Quaternion.identity );
				obj.transform.parent = this.transform;
				obj.transform.GetComponent<People>().pos = obj.transform.position;
				PeopleManager.Instance.AddPeople(obj.transform.GetComponent<People>());
			}
			
		}
	}
	
}
