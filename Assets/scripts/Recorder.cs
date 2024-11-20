// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using System.IO;
// using System.Text;

// public class Recorder : MonoBehaviour
// {
	// int maxPeople = 6000;
	// int maxSeconds = 300;
	// public string[] typeRecordArray;
	// public string[,] positionRecordArray;
	
	// public static Recorder Instance;
	// public float currentTime;
	
	// void Awake()
	// {
		// Instance = this;
		// typeRecordArray = new string[maxPeople];
		// positionRecordArray = new string[maxSeconds * 60, maxPeople];
		// currentTime = 0;
	// }
	
	// void Update()
	// {
		// currentTime += Time.deltaTime;
		// positionRecordArray[PeopleManager.Instance.frameCount, 0] = currentTime.ToString();
		// if(PeopleManager.Instance.frameCount >= maxSeconds * 60 || PeopleManager.Instance.peopleCount*3 >= maxPeople){
			// Debug.Log(PeopleManager.Instance.frameCount);
			// Debug.Log(PeopleManager.Instance.peopleCount);
			// Debug.Log("app quit");
			// Application.Quit();
			// return;
		// }	
	// }
	
	// void OnApplicationQuit()
	// {
		// using (var fileStream = new FileStream("Assets\\scripts\\result.csv", FileMode.OpenOrCreate))
		// {
			// using (var streamWriter = new StreamWriter(fileStream))
			// {
				// Debug.Log("save data");
				// string typeData = string.Join(",", typeRecordArray);
				// Debug.Log(typeData);
				// streamWriter.WriteLine(typeData);
				// Debug.Log("type saved");
				// for (int i = 0; i < Mathf.Min(positionRecordArray.GetLength(1),PeopleManager.Instance.frameCount); i++){
				// for(int j = 0; j < maxPeople; j++){
					// streamWriter.Write("{0},", positionRecordArray[i,j]);
				// }
				// streamWriter.WriteLine();
		// }
		// sw.Flush();
		// Debug.Log("file closed");
			// }
		// }
	// }
// }
