using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using static System.Console;
using System;

public class ReSimulation : MonoBehaviour
{
	
	public GameObject peopleA, peopleB;
	public GameObject Aarea, Barea;
	public string[] typeRecordArray;
	public string[,] positionRecordArray;
	public GameObject[] peopleList;
	
	public int frameCount = 0;
	int maxPeople = 6000;
	int maxSeconds = 180;
	float currentTime;
    void Awake()
    {
		currentTime = 0;
		typeRecordArray = new string[maxPeople];
		positionRecordArray = new string[maxSeconds * 60, maxPeople+1];
		peopleList = new GameObject[maxPeople];
		//csv file loading...
	   
		using (StreamReader reader = new StreamReader(@"Assets\\scripts\\result.csv"))
		{
			string line = reader.ReadLine();
			string[] dataArray = line.Split(',');
			for(int i = 0; i < dataArray.Length; i++){
				typeRecordArray[i] = dataArray[i];
			}
			for(int i = 0; i < positionRecordArray.GetLength(0); i++){
				line = reader.ReadLine();
				if(line is null)	break;
				dataArray = line.Split(',');				
				for(int j = 0; j < Mathf.Min(positionRecordArray.GetLength(1), dataArray.Length) ; j++){
					positionRecordArray[i,j] = dataArray[j];
				}
			}
		}
    }

    void Update()
    {
		currentTime += Time.deltaTime;
		frameCount++;
		//本来の時間に到達するまで、フレームを進めない。
		var second = 0f;
		//シミュレーションの秒数が読み取れなかった場合は、読み取れる行になるまで飛ばす。
		while(!(frameCount >= positionRecordArray.GetLength(1)) && !(float.TryParse(positionRecordArray[frameCount, 0], out second)) )	frameCount++;
		if(frameCount >= positionRecordArray.GetLength(1))	Application.Quit();
		//再現している形における経過時間が、記録されている時間に到達していなければ、位置の更新などを行わない。
		if( currentTime < second)	return;
		
		Debug.Log(second);
        for(int i = 1; i < positionRecordArray.GetLength(1)-3; i+=3){
			// try{
				bool notNull = float.TryParse(positionRecordArray[frameCount, i], out var x) & float.TryParse(positionRecordArray[frameCount, i+1], out var y) & float.TryParse(positionRecordArray[frameCount, i+2], out var z);
				if( notNull ){
					//positionの記録があればそれに従い、生成or位置の更新を行う必要がある。
					Vector3 pos = new Vector3(x,y,z);
					if(peopleList[i/3] is null){
						//typeに合わせた歩行者を、位置を指定しながら生成する。
						Debug.Log(typeRecordArray[i/3+1]);
						if(typeRecordArray[i/3+1] == "A"){
							GameObject people = Instantiate(peopleA, pos, Quaternion.identity);
							peopleList[i/3] = people;
						}else if(typeRecordArray[i/3+1] == "B"){
							GameObject people = Instantiate(peopleB, pos, Quaternion.identity);
							peopleList[i/3] = people;
						}
					}else{
						//生成済みであれば位置の更新を行うのみ
						// Debug.Log("Update");
						peopleList[i/3].transform.position = pos;
					}
				}
				else if(!(peopleList[i/3] is null)){
					//歩行者のobjectが残っていてかつ位置の記録が存在しない → objectの破壊が必要
					Debug.Log("destroy");
					Destroy(peopleList[i/3]);
					peopleList[i/3] = null;
				}
				// Debug.Log("Update finish");
			// } catch(ArgumentOutOfRangeException argumentOutOfException){
				// return;
			// }
		}
    }
}