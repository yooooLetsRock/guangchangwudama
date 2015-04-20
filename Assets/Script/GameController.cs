using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

	//大妈的预制件
	public GameObject auntsPre;
	//撤销按钮
	public GameObject UndoButton;
	//驱散技能按钮
	public GameObject SkillQuSanButton;
	//三人团技能按钮
	public GameObject SkillSanRenTuanButton;
	//聚集技能按钮
	public GameObject SkillJuJiButtton;
	//不同等级的大妈团的贴图
	public Sprite[] auntsSprite;
	//步数
	public Text stepNumber;

	private Mission mission;
	private Image QuSanImage;
	private Image SanRenTuanImage;
	private Image JuJiImage;
	private Text QuSanInfo;
	private Text SanRenTuanInfo;
	private Text JuJiInfo;
	private bool grouped;
	private bool isPutting;
	private bool UIClickable;
	private bool skillQuSan;
	private bool skillSanRenTuan;
	private bool skillJuJi;
	private int curIndex;
	private Queue<int> waitToPut;
	private Aunts[] aunts = new Aunts[64];

	// Use this for initialization
	void Start () {
		QuSanImage = SkillQuSanButton.GetComponent<Image> ();
		QuSanInfo = SkillQuSanButton.transform.FindChild("QuSanInfo").GetComponent<Text> ();
		SanRenTuanImage = SkillSanRenTuanButton.GetComponent<Image> ();
		SanRenTuanInfo = SkillSanRenTuanButton.transform.FindChild("SanRenTuanInfo").GetComponent<Text> ();
		JuJiImage = SkillJuJiButtton.GetComponent<Image> ();
		JuJiInfo = SkillJuJiButtton.transform.FindChild("JuJiInfo").GetComponent<Text> ();
		mission = new Mission ();
		InitAunts ();
		waitToPut = new Queue<int> ();
	}

	public void SetDownDaMa (int index) {
		if (UIClickable) {
			//执行驱散一个大妈团的技能
			if (skillQuSan) {
				if(aunts [index].GetAuntsType () > 0) {
					AfterQuSan(index);
					skillQuSan = false;
				}
			}
			else if(skillSanRenTuan) {
				if(aunts [index].GetAuntsType () == 0) {
					StartCoroutine(SetCurrentDaMa(index, 2, 3));
					SanRenTuanInfo.text = "三人团";
					SanRenTuanImage.color = new Vector4(0.867f, 0.667f, 0.867f, 1);
				}
			}
			else if(skillJuJi) {
				if(aunts [index].GetAuntsType () == 0) {
					StartCoroutine(SetCurrentDaMa(index));
				}
			}
			else {
				if (aunts [index].GetAuntsType () == 0) {
					waitToPut.Enqueue (index);
					if (!isPutting) {
						StartCoroutine (DaMaInQueue ());
						isPutting = true;
					}
				}
			}
		}
	}

	//技能：驱散一个舞团
	public void SkillQuSan() {
		if (UIClickable && !skillSanRenTuan && !skillJuJi) {
			if (!skillQuSan) {
				skillQuSan = true;
				QuSanImage.color = new Vector4(1, 0, 1, 1);
				QuSanInfo.text = "请选择要驱散的大妈团或点击技能取消";
			}
			else {
				skillQuSan = false;
				QuSanImage.color = new Vector4(0.867f, 0.667f, 0.867f, 1);
				QuSanInfo.text = "驱散";
			}
		}
	}

	//技能：放置一个三人舞团
	public void SkillSanRenTuan() {
		if (UIClickable && !skillQuSan && !skillJuJi) {
			if (!skillSanRenTuan) {
				skillSanRenTuan = true;
				SanRenTuanImage.color = new Vector4(1, 0, 1, 1);
				SanRenTuanInfo.text = "请选择要放置的位置或点击技能取消";
			}
			else {
				skillSanRenTuan = false;
				SanRenTuanImage.color = new Vector4(0.867f, 0.667f, 0.867f, 1);
				SanRenTuanInfo.text = "三人团";
			}
		}
	}

	//技能：放置并聚集四周的大妈
	public void SkillJuJi() {
		if (UIClickable && !skillQuSan && !skillSanRenTuan) {
			if (!skillJuJi) {
				skillJuJi = true;
				JuJiImage.color = new Vector4(1, 0, 1, 1);
				JuJiInfo.text = "请选择要放置的位置或点击技能取消";
			}
			else {
				skillJuJi = false;
				JuJiImage.color = new Vector4(0.867f, 0.667f, 0.867f, 1);
				JuJiInfo.text = "聚集";
			}
		}
	}

	//撤销（只限一步且上步未发生合成）
	public void UndoStep() {
		if (UIClickable) {
			if (!grouped) {
				//显示当前格子颜色
				Text number;
				Text level;
				number = GameObject.Find ("blank" + curIndex).transform.FindChild ("Number").GetComponent<Text> ();
				level = GameObject.Find ("blank" + curIndex).transform.FindChild ("Level").GetComponent<Text> ();
				aunts [curIndex].ClearAunts ();
				Destroy(GameObject.Find("aunt" + curIndex));
				Debug.Log(curIndex);
				number.text = "";
				level.text = "";
				mission.ChangeRound (-1);
				mission.ChangeAunts (-1);
				stepNumber.text = mission.GetRound ().ToString ();
				grouped = true;
				UndoButton.SetActive (false);
			}
		}
	}

	//重新开始
	public void ReStart() {
		if (UIClickable) {
			InitAunts ();
			Text number;
			Text level;
			for (int i = 9; i < 55; i++) {
				if (aunts [i].GetAuntsType () == 0) {
					number = GameObject.Find ("blank" + i).transform.FindChild ("Number").GetComponent<Text> ();
					level = GameObject.Find ("blank" + i).transform.FindChild ("Level").GetComponent<Text> ();
					Destroy(GameObject.Find("aunt" + i));
					number.text = "";
					level.text = "";
				}
			}
		}
	}

	//初始化关卡
	private void InitAunts () {
		stepNumber.text = mission.GetRound().ToString ();
		for (int i = 0; i < 8; i++) {
			aunts[i] = new Aunts(-1);
		}
		for (int i = 56; i < 64; i++) {
			aunts[i] = new Aunts(-1);
		}
		for (int i = 8; i < 56; i++) {
			if((i % 8 == 0) || (i % 8 == 7)) {
				aunts[i] = new Aunts(-1);
			}
			else {
				aunts[i] = new Aunts();
			}
		}
		mission.SetQiShi (100);
		QuSanInfo.text = "驱散";
		QuSanImage.color = new Vector4(0.867f, 0.667f, 0.867f, 1);
		SanRenTuanInfo.text = "三人团";
		SanRenTuanImage.color = new Vector4(0.867f, 0.667f, 0.867f, 1);
		JuJiInfo.text = "聚集";
		JuJiImage.color = new Vector4(0.867f, 0.667f, 0.867f, 1);
		UIClickable = true;
		grouped = true;
		isPutting = false;
		skillQuSan = false;
		skillSanRenTuan = false;
		skillJuJi = false;
		curIndex = -1;
		stepNumber.text = "0";
		UndoButton.SetActive (false);
	}

	//将大妈放入队列以供放置
	private IEnumerator DaMaInQueue() {
		yield return new WaitForSeconds(0.1f);
		if (waitToPut.Count > 0) {
			StartCoroutine(SetCurrentDaMa (waitToPut.Dequeue ()));
			StartCoroutine (DaMaInQueue ());
		} else {
			isPutting = false;
		}
	}

	//放置大妈并判断是否合并
	private IEnumerator SetCurrentDaMa(int index, int setLevel = 1, int setNumber = 1) {
		//在格子上放置大妈
		curIndex = index;
		mission.ChangeRound (1);
		mission.ChangeAunts (setNumber);
		stepNumber.text = mission.GetRound().ToString ();
		aunts [index].SetAuntsType (1);
		aunts [index].SetNumber (setNumber);
		aunts [index].SetLevel (setLevel);
		//在该格上生成的大妈图片
		GameObject thisAunt;
		Image thisImage;
		Text number;
		Text level;
		thisAunt = (GameObject)Instantiate (auntsPre);
		thisAunt.name = "aunt" + index;
		thisAunt.transform.SetParent(GameObject.Find ("blank" + index).transform);
		thisAunt.transform.localPosition = Vector3.zero;
		thisAunt.transform.localScale = Vector3.one;
		thisImage = thisAunt.GetComponent<Image> ();
		number = GameObject.Find ("blank" + index).transform.FindChild("Number").GetComponent<Text>();
		level = GameObject.Find ("blank" + index).transform.FindChild("Level").GetComponent<Text>();
		thisImage.sprite = auntsSprite [aunts [index].GetLevel () - 1];
		number.text = aunts[index].GetNumber().ToString();
		level.text = aunts[index].GetLevel().ToString();
		grouped = false;
		yield return new WaitForSeconds(0.1f);
		if (skillJuJi) {
			//如果是聚集技能则聚集周围四格的大妈
			AfterJuJi(index);
			skillJuJi = false;
		} else {
			//如果不是聚集技能则判断是否集合
			HeBingDaMa (index, thisImage, level, number);
		}

		//如果发生了集合则不能撤销
		if(grouped) {
			UndoButton.SetActive(false);
		} else {
			UndoButton.SetActive(true);
		}
	}

	//驱散技能施放之后数据变化
	private void AfterQuSan(int index) {
		//待清空的格子的number,level;
		Text auntNumber;
		Text auntLevel;
		mission.ChangeAunts(-aunts[index].GetNumber());
		aunts [index].ClearAunts ();
		auntNumber = GameObject.Find ("blank" + index).transform.FindChild("Number").GetComponent<Text>();
		auntLevel = GameObject.Find ("blank" + index).transform.FindChild("Level").GetComponent<Text>();
		Destroy(GameObject.Find ("aunt" + index));
		auntNumber.text = "";
		auntLevel.text = "";
		QuSanImage.color = new Vector4(0.867f, 0.667f, 0.867f, 1);
		QuSanInfo.text = "驱散";
		grouped = true;
		UndoButton.SetActive (false);
		mission.ChangeRound (1);
		stepNumber.text = mission.GetRound().ToString ();
	}

	//聚集技能施放之后数据变化
	private void AfterJuJi(int index) {
		CanBeComposed (index, index - 1);
		CanBeComposed (index, index - 8);
		CanBeComposed (index, index + 1);
		CanBeComposed (index, index + 8);
		Image image;
		Text number;
		Text level;
		image = GameObject.Find ("aunt" + index).GetComponent<Image> ();
		number = GameObject.Find ("blank" + index).transform.FindChild("Number").GetComponent<Text> ();
		level = GameObject.Find ("blank" + index).transform.FindChild("Level").GetComponent<Text> ();
		image.sprite = auntsSprite [aunts [index].GetLevel () - 1];
		number.text = aunts[index].GetNumber().ToString();
		level.text = aunts[index].GetLevel().ToString();
		JuJiInfo.text = "聚集";
		JuJiImage.color = new Vector4(0.867f, 0.667f, 0.867f, 1);
		grouped = true;
	}

	//如果有大妈则合并，index为放置的坐标，target为待合并的坐标
	private void CanBeComposed(int index, int target) {
		if (aunts [target].GetAuntsType() > 0) {
			//待清空的格子的number,level;
			Text auntNumber;
			Text auntLevel;
			aunts[index].GetTogether(aunts[target]);
			aunts[target].ClearAunts();
			Destroy(GameObject.Find("aunt" + target));
			auntNumber = GameObject.Find ("blank" + target).transform.FindChild("Number").GetComponent<Text>();
			auntLevel = GameObject.Find ("blank" + target).transform.FindChild("Level").GetComponent<Text>();
			auntNumber.text = "";
			auntLevel.text = "";
		}
	}

	//大妈合并的算法
	private void HeBingDaMa(int index, Image thisImage, Text level, Text number) {
		//待合并格子数
		int count;
		//用于存放待合并格子的下标
		int[] needHeBing = new int[8];
		//待清空的格子的number,level;
		Text auntNumber;
		Text auntLevel;
		for (int curLevel = aunts[index].GetLevel (); curLevel <= 8; curLevel++) {
			count = 0;
			//=====================================================向左判断============================================================//
			if (aunts [index].AuntsIsEqual (aunts [index - 1])) {
				needHeBing [count] = index - 1;
				count++;
				if (aunts [index].AuntsIsEqual (aunts [index - 2])) {
					needHeBing [count] = index - 2;
					count++;
				} else if (aunts [index].AuntsIsEqual (aunts [index - 9])) {
					needHeBing [count] = index - 9;
					count++;
				} else if (aunts [index].AuntsIsEqual (aunts [index + 7])) {
					if (!aunts [index].AuntsIsEqual (aunts [index + 8])) {
						needHeBing [count] = index + 7;
						count++;
					}
				}
			}
			//=====================================================向上判断============================================================//
			if (aunts [index].AuntsIsEqual (aunts [index - 8])) {
				needHeBing [count] = index - 8;
				count++;
				if (aunts [index].AuntsIsEqual (aunts [index - 9])) {
					if (!aunts [index].AuntsIsEqual (aunts [index - 1])) {
						needHeBing [count] = index - 9;
						count++;
					}
				} else if (aunts [index].AuntsIsEqual (aunts [index - 16])) {
					needHeBing [count] = index - 16;
					count++;
				} else if (aunts [index].AuntsIsEqual (aunts [index - 7])) {
					needHeBing [count] = index - 7;
					count++;
				}
			}
			//=====================================================向右判断============================================================//
			if (aunts [index].AuntsIsEqual (aunts [index + 1])) {
				needHeBing [count] = index + 1;
				count++;
				if (aunts [index].AuntsIsEqual (aunts [index - 7])) {
					if (!aunts [index].AuntsIsEqual (aunts [index - 8])) {
						needHeBing [count] = index - 7;
						count++;
					}
				} else if (aunts [index].AuntsIsEqual (aunts [index + 2])) {
					needHeBing [count] = index + 2;
					count++;
				} else if (aunts [index].AuntsIsEqual (aunts [index + 9])) {
					needHeBing [count] = index + 9;
					count++;
				}
			}
			//=====================================================向下判断============================================================//
			if (aunts [index].AuntsIsEqual (aunts [index + 8])) {
				needHeBing [count] = index + 8;
				count++;
				if (aunts [index].AuntsIsEqual (aunts [index + 7])) {
					needHeBing [count] = index + 7;
					count++;
				} else if (aunts [index].AuntsIsEqual (aunts [index + 16])) {
					needHeBing [count] = index + 16;
					count++;
				} else if (aunts [index].AuntsIsEqual (aunts [index + 9])) {
					if (!aunts [index].AuntsIsEqual (aunts [index + 1])) {
						needHeBing [count] = index + 9;
						count++;
					}
				}
			}
			
			//合并格子
			if (count > 1) {
				grouped = true;
				for (int i = 0; i < count; i++) {
					aunts [index].GetTogether (aunts [needHeBing [i]]);
					aunts [needHeBing [i]].ClearAunts ();
					//删除待清空格子的大妈图片
					Destroy(GameObject.Find ("aunt" + needHeBing [i]));
					auntNumber = GameObject.Find ("blank" + needHeBing [i]).transform.FindChild("Number").GetComponent<Text>();
					auntLevel = GameObject.Find ("blank" + needHeBing [i]).transform.FindChild("Level").GetComponent<Text>();
					auntNumber.text = "";
					auntLevel.text = "";
				}
				thisImage.sprite = auntsSprite[aunts[index].GetLevel() - 1];
				number.text = aunts[index].GetNumber().ToString();
				level.text = aunts[index].GetLevel().ToString();
			}
			else {
				if(skillSanRenTuan) {
					grouped = true;
					skillSanRenTuan = false;
				}
				break;
			}
		}
	}
}
