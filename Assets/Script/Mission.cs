using UnityEngine;
using System.Collections;

public class Mission : Object {

	//回合数
	private int _round;
	//喝彩值
	private int _cheer;
	//最大集群的最大人数
	private int _groupNum;
	//大妈数量
	private int _aunts;
	//气势值
	private int _qishi;
	
	public Mission(int round = 0, int cheer = 0, int groups = 0, int aunts = 0, int qishi = 100) {
		_round = round;
		_cheer = cheer;
		_groupNum = groups;
		_aunts = aunts;
		_qishi = qishi;
	}

	//初始化关卡
	public void InitMission() {
		_round = 0;
		_cheer = 0;
		_groupNum = 0;
		_aunts = 0;
		_qishi = 0;
	}

	//改变回合数，一般加1或减1
	public void ChangeRound(int i) {
		_round += i;
	}

	//改变喝彩值
	public void ChangeCheer(int i) {
		_cheer += i;
	}
	
	//改变集群数
	public void ChangeGroupNum(int i) {
		_groupNum = i;
	}
	
	//改变大妈数量
	public void ChangeAunts(int i) {
		_aunts += i;
	}

	//改变气势值
	public void ChangeQiShi(int i) {
		_qishi += i;
	}

	public void SetQiShi(int i) {
		_qishi = i;
	}

	//获取回合数
	public int GetRound() {
		return _round;
	}
	
	//获取喝彩值
	public int GetCheer() {
		return _cheer;
	}
	
	//获取集群数
	public int GetGroupsNum() {
		return _groupNum;
	}
	
	//获取大妈数量
	public int GetAunts() {
		return _aunts;
	}
	
	//获取气势值
	public int GetQiShi() {
		return _qishi;
	}
}
