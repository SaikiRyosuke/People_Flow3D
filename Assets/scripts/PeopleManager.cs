using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Mode
{
	X,Y,Z,Off
}

public class PeopleManager : MonoBehaviour
{
	public int peopleCount = 0;
	public List<People> peopleList = new List<People>();
	public static PeopleManager Instance;
	public GameObject A,B;
	public int generateRate = 5;
	public GameObject X,Y,Z,Aarea,Barea;
	public Mode mode = Mode.Off;
	public float wallMoveSpeed = 0.06f;
	public int frameCount = 0;
    void Awake()
    {
        Instance = this;
		for(int i = 0; i < A.transform.childCount; i++){
			peopleList.Add(A.transform.GetChild(i).GetComponent<People>());
			A.transform.GetChild(i).GetComponent<People>().id = peopleCount++;
		}
		for(int i = 0; i < B.transform.childCount; i++){
			peopleList.Add(B.transform.GetChild(i).GetComponent<People>());
			B.transform.GetChild(i).GetComponent<People>().id = peopleCount++;

		}
    }
	
	void Start()
	{
		
	}

    // Update is called once per frame
    void Update()
    {
		frameCount++;
		
        if(mode == Mode.X){
			X.SetActive(true);
			Y.SetActive(false);
			Z.SetActive(false);
			Aarea.SetActive(false);
			Barea.SetActive(false);
			X.transform.position = new Vector3(X.transform.position.x + wallMoveParameter()*wallMoveSpeed, X.transform.position.y, X.transform.position.z);
		}
		else if(mode == Mode.Y){
			Y.SetActive(true);
			Z.SetActive(false);
			X.SetActive(false);
			Aarea.SetActive(false);
			Barea.SetActive(false);
			Y.transform.position = new Vector3( Y.transform.position.x,Y.transform.position.y + wallMoveParameter()*wallMoveSpeed, Y.transform.position.z);
		}
		else if(mode == Mode.Z){
			Z.SetActive(true);
			X.SetActive(false);
			Y.SetActive(false);
			Aarea.SetActive(false);
			Barea.SetActive(false);
			Z.transform.position = new Vector3(Z.transform.position.x, Z.transform.position.y, Z.transform.position.z+ wallMoveParameter() * wallMoveSpeed);
		}
		else if(mode == Mode.Off){
			X.SetActive(false);
			Y.SetActive(false);
			Z.SetActive(false);
			Aarea.SetActive(true);
			Barea.SetActive(true);
		}
    }
	
	public void AddPeople(People people)
	{
		peopleList.Add(people);
		people.id = peopleCount++;
	}
	
	public void RemovePeople(People people)
	{
		peopleList.Remove(people);
	}
	
	public float wallMoveParameter()
	{
		if(Input.GetKey(KeyCode.U))	return 1f;
		if(Input.GetKey(KeyCode.I))	return -1f;
		else return 0f;
	}
}
