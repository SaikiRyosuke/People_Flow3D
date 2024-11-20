using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

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
	public GameObject X,Y,Z,Aarea,Barea;
	public Mode mode = Mode.Off;
	public float wallMoveSpeed = 0.06f;
	
	public float[,] phiArray;
	public int[,] npArray, nmArray;
	StreamWriter streamWriter;
	public float phi;
	
	public Text phiText;
	public Slider timeScaleSlider;
	public Text timeScaleText;
	public GameObject phiPlate;
	public GameObject[,] phiPlateArray;
	
	//なくてもよいが、可読性を高めるために
	float Lx, Ly, Lz;
	int phiDivision;
	
    void Awake()
    {
        Instance = this;
    }
	
	void Start()
	{
		A.SetActive(true);	B.SetActive(true);
		Lx = Parameters.Instance.Lx; Ly = Parameters.Instance.Ly; Lz = Parameters.Instance.Lz;
		phiDivision = Parameters.Instance.phiDivision;
		timeScaleSlider.value = 1f;
		phiArray = new float[phiDivision, phiDivision];
		npArray = new int[phiDivision, phiDivision];
		nmArray = new int[phiDivision, phiDivision];
		phiPlateArray = new GameObject[phiDivision, phiDivision];
		streamWriter = Parameters.Instance.streamWriter;
		streamWriter.WriteLine("t,phi");
		for(int i = 0; i < phiDivision; i++)	for(int j = 0; j < phiDivision; j++){
			var obj = Instantiate(phiPlate, new Vector3(Lx + 5, -Ly + Ly/phiDivision + 2*Ly/phiDivision * i,-Lz + Lz/phiDivision + 2*Lz/phiDivision * j ), Quaternion.identity);
			obj.transform.localScale = new Vector3(0.1f, 2*Ly/phiDivision, 2*Lz/phiDivision);
			phiPlateArray[i,j] = obj;
		}	
	}

    // Update is called once per frame
    void Update()
    {
		
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
		
		foreach(People people in peopleList){
			int i = (int) ((people.pos.y + Ly) * phiDivision / 2 / Ly);
			int j = (int) ((people.pos.z + Lz) * phiDivision / 2 / Lz);
			if(i < 0 || i >= phiDivision)	continue;
			if(j < 0 || j >= phiDivision)	continue;
			
			if(people.type == Type.A)	npArray[i,j] += 1;
			else if(people.type == Type.B) nmArray[i,j] += 1;
		}
		int count = 0;	float sum = 0;
		for(int i = 0; i < phiDivision; i++){
			for(int j = 0; j < phiDivision; j++){
				int np = npArray[i,j]; int nm = nmArray[i,j];
				if(np == 0 || nm == 0){
					phiArray[i,j] = 5;
					phiPlateArray[i,j].GetComponent<Renderer>().material.color = new Color(0,0,0,1);
				}	
				else{
					phiArray[i,j] = (np - nm) / (np + nm);
					count++; sum += Mathf.Abs(phiArray[i,j]);
					//ここに色を変更する処理を加える
					Color phiColor = new Color(0,0,0,1);
					if(phiArray[i,j] >= 0)	phiColor = new Color(1,1-phiArray[i,j], 1-phiArray[i,j],1);
					else if(phiArray[i,j] <= 0) phiColor = new Color(1-phiArray[i,j], 1-phiArray[i,j], 1,1);
					phiPlateArray[i,j].GetComponent<Renderer>().material.color = phiColor;
				} 
			}
		}
		phi = sum / count;
		Debug.Log(phi);
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
	
	public void ChangeTimeScale()
	{
		Time.timeScale = timeScaleSlider.value;
		timeScaleText.text = Time.timeScale.ToString();
	}
	
	public void ResetSim()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}
