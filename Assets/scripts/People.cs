using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class People : MonoBehaviour
{
	public int id = 0;
	Parent startParent, goalParent;
	public Type type;
	public Vector3 velocity = new Vector3();
	Vector3 f_soc = new Vector3(0,0,0);
	Vector3 f_ph = new Vector3(0,0,0);
	Vector3 f_b_soc = new Vector3(0,0,0);
	Vector3 f_b_ph = new Vector3(0,0,0);
	Vector3 e = new Vector3(0,0,0);
	public Vector3 pos = new Vector3(0,0,0);
	
	float r = 0f, d = 0f, deltaV = 0f, cos_phiAlphaBeta = 0f;
	Vector3 n = new Vector3(0,0,0), t = new Vector3(0,0,0);
	

	//parameters follow
	private float tau = 1f; //tau: relaxation time
	private Vector3 xi = new Vector3(0,0,0); //xi: fluctuation
	private float v0 = 2f; //v0: desired speed
	private Vector3 e0 = new Vector3(0,0,0);
	private float xiAmp = 1f; //
	
	private float Aalpha_same = 0.1f; 	//Aalpha: the altitude of soc-force
	private float Aalpha_opp = 1f; 	//Aalpha: the altitude of soc-force
	private float radius = 0.5f; //radius: the radius of the person's body
	private float BAlpha = 0.1f; //bAlpha: decrease rate in socio-phychological force
	private float lambdaAlpha = 0.5f; //lambdaAlpha: description of anisotropic force
	private float k = 10f; //k: body counteracting force
	private float kappa = 5f; //kappa: sliding friction force


	void Awake()
	{
		pos = this.transform.position;
		tau = Parameters.Instance.tau;
		v0 = Parameters.Instance.v0;
		Aalpha_same = Parameters.Instance.Aalpha_same;
		Aalpha_opp = Parameters.Instance.Aalpha_opp;
		radius = Parameters.Instance.radius;
		BAlpha = Parameters.Instance.BAlpha;
		lambdaAlpha = Parameters.Instance.lambdaAlpha;
		k = Parameters.Instance.k;
		kappa = Parameters.Instance.kappa;
		this.gameObject.transform.localScale = this.radius * 2 * new Vector3(1,1,1);
		
	}

	void Start()
	{
		type = transform.parent.gameObject.GetComponent<Parent>().type;

		/*startParent = start.GetComponent<Parent>();
		goalParent = start.GetComponent<Parent>();*/
		gameObject.GetComponent<MeshRenderer>().enabled = false;
		if(type == Type.A)	e0 = new Vector3(1,0,0);
		if(type == Type.B)	e0 = new Vector3(-1,0,0);
		velocity = v0*e0;

	}

	void Update()
	{
		//色変更
		Color color;
		if(PeopleManager.Instance.mode == Mode.X){
			color = gameObject.GetComponent<Renderer>().material.color;
			if(Mathf.Abs(PeopleManager.Instance.X.transform.position.x - this.gameObject.transform.position.x) < 1) gameObject.GetComponent<MeshRenderer>().enabled = true;
			else gameObject.GetComponent<MeshRenderer>().enabled = false;
		}
		else if(PeopleManager.Instance.mode == Mode.Y){
			color = gameObject.GetComponent<Renderer>().material.color;
			if(Mathf.Abs(PeopleManager.Instance.Y.transform.position.y - this.gameObject.transform.position.y) < 1) gameObject.GetComponent<MeshRenderer>().enabled = true;
			else gameObject.GetComponent<MeshRenderer>().enabled = false;
		}
		else if(PeopleManager.Instance.mode == Mode.Z){
			color = gameObject.GetComponent<Renderer>().material.color;
			if(Mathf.Abs(PeopleManager.Instance.Z.transform.position.z - this.gameObject.transform.position.z) < 1) gameObject.GetComponent<MeshRenderer>().enabled = true;
			else gameObject.GetComponent<MeshRenderer>().enabled = false;
		}
		else if(PeopleManager.Instance.mode == Mode.Off){
			color = gameObject.GetComponent<Renderer>().material.color;
			gameObject.GetComponent<MeshRenderer>().enabled = true;
			
		}
		
		pos = this.transform.position;

		//目的地に到達したら削除→周期的境界条件に変更
		if(type == Type.A && pos.x > Parameters.Instance.Lx){
			this.transform.position = new  Vector3(-Parameters.Instance.Lx, pos.y, pos.z);
			this.pos = new Vector3(-Parameters.Instance.Lx, pos.y, pos.z);
		}	
		else if(type == Type.B && pos.x < -Parameters.Instance.Lx){
			this.transform.position = new  Vector3(Parameters.Instance.Lx, pos.y, pos.z);
			this.pos = new Vector3(Parameters.Instance.Lx, pos.y, pos.z);
		}	
		
		//物理量計算パート
		xi = new Vector3(Random.Range(-xiAmp,xiAmp), Random.Range(-xiAmp,xiAmp),Random.Range(-xiAmp,xiAmp));
		e = velocity/velocity.magnitude;
		
		f_ph = new Vector3(0,0,0); f_soc = new Vector3(0,0,0);
		foreach(People people in PeopleManager.Instance.peopleList){
			if(people == this)	continue;
			else{
				r = this.radius + people.radius;
				d = (this.pos - people.pos).magnitude;
				if((r-d)/BAlpha < -5 || d == 0)	continue;
				n = (this.pos - people.pos) / d;
				// t = new Vector3(-n.y, n.x);
				t = Vector3.Normalize(Vector3.ProjectOnPlane(people.velocity - this.velocity, n));
				// Debug.Log(d); Debug.Log(people); Debug.Log(people.pos); Debug.Log(people.transform.position);
				deltaV = Vector3.Dot(people.velocity - this.velocity, t);
				cos_phiAlphaBeta = -Vector3.Dot(n,e);
				// Socio-phychological Force
				if(this.type == people.type)	f_soc += Aalpha_same * Mathf.Exp((r-d)/BAlpha) * (lambdaAlpha + (1-lambdaAlpha)*(1+cos_phiAlphaBeta)/2) * n;
				else	f_soc += Aalpha_opp * Mathf.Exp((r-d)/BAlpha) * (lambdaAlpha + (1-lambdaAlpha)*(1+cos_phiAlphaBeta)/2) * n;
				// physical force
				f_ph += k* Theta(r-d) * n + kappa * Theta(r-d) * deltaV * t;
			}
			
		}
		//interactions from boundary(y = 10)
		r = this.radius;
		d = Parameters.Instance.Ly - this.pos.y;
		n = new Vector3(0,-1,0);
		t = Vector3.ProjectOnPlane(- this.velocity, n);
		f_b_soc = Aalpha_opp * Mathf.Exp((r-d)/BAlpha) * n;
		f_b_ph = k* Theta(r-d) * n + kappa * Theta(r-d) * t;
		//interactions from boundary(y = -10)
		r = this.radius;
		d = this.pos.y + Parameters.Instance.Ly;
		n = new Vector3(0,1,0);
		t = Vector3.ProjectOnPlane(- this.velocity, n);
		f_b_soc += Aalpha_opp * Mathf.Exp((r-d)/BAlpha) * n;
		// if(float.IsNaN(Vector3.Dot(f_b_soc, new Vector3(1,1,1)))) Debug.LogWarning("y=-10");
		f_b_ph += k* Theta(r-d) * n + kappa * Theta(r-d) * t;
		//interactions from boundary (z = 10)
		r = this.radius;
		d = Parameters.Instance.Lz - this.pos.z;
		n = new Vector3(0,0,-1);
		t = Vector3.ProjectOnPlane(- this.velocity, n);
		f_b_soc += Aalpha_opp * Mathf.Exp((r-d)/BAlpha) * n;
		// if(float.IsNaN(Vector3.Dot(f_b_soc, new Vector3(1,1,1)))) Debug.LogWarning("z=10");
		f_b_ph += k* Theta(r-d) * n + kappa * Theta(r-d) * t;
		//interactions from boundary (z = -10)
		r = this.radius;
		d = this.pos.z + Parameters.Instance.Lz;
		n = new Vector3(0,0,1);
		t = Vector3.ProjectOnPlane(- this.velocity, n);
		f_b_soc += Aalpha_opp * Mathf.Exp((r-d)/BAlpha) * n;
		// if(float.IsNaN(Vector3.Dot(f_b_soc, new Vector3(1,1,1)))) Debug.LogWarning("z=-10");
		f_b_ph += k* Theta(r-d) * n + kappa * Theta(r-d) * t;
		
 		// if(float.IsNaN(Vector3.Dot(f_soc,new Vector3(1,1,1))))	Debug.Log("f_soc calc is wrong");
 		// if(float.IsNaN(Vector3.Dot(f_ph,new Vector3(1,1,1))))	Debug.Log("f_ph calc is wrong");
		// if(float.IsNaN(Vector3.Dot(f_b_soc,new Vector3(1,1,1))))	Debug.Log("f_b_soc calc is wrong");	
		// if(float.IsNaN(Vector3.Dot(f_b_ph,new Vector3(1,1,1))))	Debug.Log("f_b_ph calc is wrong");	
		
		//オイラー法による速度の計算
		velocity += ( (v0*e0 + xi - velocity)/tau + Vector3.ClampMagnitude(f_soc + f_ph + f_b_soc + f_b_ph, 100f) )* Time.deltaTime;
		pos += velocity * Time.deltaTime;
		
		//位置を記録する。
		// if(id*3 < Recorder.Instance.positionRecordArray.Length){
			// Recorder.Instance.positionRecordArray[PeopleManager.Instance.frameCount,id*3+1] = pos.x.ToString();
			// Recorder.Instance.positionRecordArray[PeopleManager.Instance.frameCount,id*3+2] = pos.y.ToString();
			// Recorder.Instance.positionRecordArray[PeopleManager.Instance.frameCount,id*3+3] = pos.z.ToString();
			
		// }
		this.transform.position = pos;
	}

	float Theta(float z)
	{
		if(z >= 0)	return z;
		else return 0f;
	}

	
	



}
