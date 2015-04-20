using UnityEngine;
using System.Collections;

public class Aunts : Object {

	private int _number;
	private int _level;
	private int _index;
	private int _type;

	public Aunts(int type = 0) {
		_number = 0;
		_level = 0;
		_index = 0;
		_type = type;
	}

	//将B舞团拷贝给A舞团
	public void CopyAunts(Aunts auntA, Aunts auntB) {
		auntA.SetAuntsType (auntB.GetAuntsType());
		auntA.SetNumber (auntB.GetNumber());
		auntA.SetLevel (auntB.GetLevel());
		auntA.SetIndex (auntB.GetIndex());
	}

	public int GetNumber() {
		return _number;
	}

	public int GetLevel() {
		return _level;
	}
	
	public int GetIndex() {
		return _index;
	}
	
	public int GetAuntsType() {
		return _type;
	}

	public void SetNumber(int number) {
		_number = number;
	}
	
	public void SetLevel(int level) {
		_level = level;
	}
	
	public void SetIndex(int index) {
		_index = index;
	}
	
	public void SetAuntsType(int type) {
		_type = type;
	}

	public void ClearAunts() {
		_number = 0;
		_level = 0;
		_index = 0;
		_type = 0;
	}

	public bool AuntsIsEqual(Aunts target) {
		return (_level == target.GetLevel ());
	}

	public void GetTogether(Aunts target) {
		_number += target.GetNumber ();
		if (_number == 1) {
			SetLevel (1);
		} else if (_number <= 3) {
			SetLevel (2);
		} else if (_number <= 9) {
			SetLevel (3);
		} else if (_number <= 27) {
			SetLevel (4);
		} else if (_number <= 81) {
			SetLevel (5);
		} else if (_number <= 243) {
			SetLevel (6);
		} else if (_number <= 729) {
			SetLevel (7);
		}
	}
}