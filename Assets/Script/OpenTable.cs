using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Mono.Data.Sqlite;

public class OpenTable : MonoBehaviour {

	private int bestCheer = 0;
	private int bestNumber = 0;
	private int bestCompose = 0;

	// Use this for initialization
	void Start () {
		LoadSQL ();
	}

	void LoadSQL() {

#if UNITY_EDITOR
		//通过路径找到第三方数据库
		string appDBPath = Application.dataPath + "/Plugins/Android/assets/" + "Chart";
		DbAccess db = new DbAccess("URI=file:" + appDBPath);
		//如果运行在Android设备中
#elif UNITY_ANDROID
		//将第三方数据库拷贝至Android可找到的地方
		string appDBPath = Application.persistentDataPath + "/" + "Chart";
		
		//如果已知路径没有地方放数据库，那么我们从Unity中拷贝
		if(!File.Exists(appDBPath))
			
		{
			//用www先从Unity中下载到数据库
			WWW loadDB = new WWW("jar:file://" + Application.dataPath + "!/assets/" + "Chart"); 
			
			//拷贝至规定的地方
			File.WriteAllBytes(appDBPath, loadDB.bytes);
			
		}
		
		//在这里重新得到db对象。
		DbAccess db = new DbAccess("URI=file:" + appDBPath);
		
#endif	
		
		using (SqliteDataReader sqReader = db.SelectWhere("Chart",new string[]{"BestCheer","BestNumber","BestCompose"},new string[]{"_id"},new string[]{"="},new string[]{"1"}))
		{
			
			while (sqReader.Read())
			{
				//从数据库中找到对应字段
				bestCheer = sqReader.GetInt32(sqReader.GetOrdinal("BestCheer"));
				bestNumber = sqReader.GetInt32(sqReader.GetOrdinal("BestNumber"));
				bestCompose = sqReader.GetInt32(sqReader.GetOrdinal("BestCompose"));

				Debug.Log("cheer: " + bestCheer + "\n" + "number: " + bestNumber + "\n" + "compose: " + bestCompose);
			}
			
			sqReader.Close();
		}
		
		db.CloseSqlConnection();
	}
}