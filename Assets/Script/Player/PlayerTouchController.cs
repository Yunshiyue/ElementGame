using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTouchController : MonoBehaviour
{
	/// <summary>
	/// 定义的一个手指类
	/// </summary>
	class MyFinger
	{
		public int id = -1;
		public Touch touch;

		static private List<MyFinger> fingers = new List<MyFinger>();
		/// <summary>
		/// 手指容器
		/// </summary>
		static public List<MyFinger> Fingers
		{
			get
			{
				if (fingers.Count == 0)
				{
					for (int i = 0; i < 5; i++)
					{
						MyFinger mf = new MyFinger();
						mf.id = -1;
						fingers.Add(mf);
					}
				}
				return fingers;
			}
		}

	}

	// 小圈圈：用来实时显示手指触摸的位置
	GameObject[] marks = new GameObject[5];
	public GameObject markPerfab = null;

	// 粒子效果：来所显示手指手动的大概路径
	ParticleSystem[] particles = new ParticleSystem[5];
	public ParticleSystem particlePerfab = null;


	// Use this for initialization
	void Start()
	{
		// init marks and particles
		for (int i = 0; i < MyFinger.Fingers.Count; i++)
		{
			GameObject mark = Instantiate(markPerfab, Vector3.zero, Quaternion.identity) as GameObject;
			mark.transform.parent = this.transform;
			mark.SetActive(false);
			marks[i] = mark;

			ParticleSystem particle = Instantiate(particlePerfab, Vector3.zero, Quaternion.identity) as ParticleSystem;
			particle.transform.parent = this.transform;
			particle.Pause();
			particles[i] = particle;
		}
	}

	// Update is called once per frame
	void Update()
	{
		Touch[] touches = Input.touches;

		// 遍历所有的已经记录的手指
		// --掦除已经不存在的手指
		foreach (MyFinger mf in MyFinger.Fingers)
		{
			if (mf.id == -1)
			{
				continue;
			}
			bool stillExit = false;
			foreach (Touch t in touches)
			{
				if (mf.id == t.fingerId)
				{
					stillExit = true;
					break;
				}
			}
			// 掦除
			if (stillExit == false)
			{
				mf.id = -1;
			}
		}
		// 遍历当前的touches
		// --并检查它们在是否已经记录在AllFinger中
		// --是的话更新对应手指的状态，不是的放放加进去
		foreach (Touch t in touches)
		{
			bool stillExit = false;
			// 存在--更新对应的手指
			foreach (MyFinger mf in MyFinger.Fingers)
			{
				if (t.fingerId == mf.id)
				{
					stillExit = true;
					mf.touch = t;
					break;
				}
			}
			// 不存在--添加新记录
			if (!stillExit)
			{
				foreach (MyFinger mf in MyFinger.Fingers)
				{
					if (mf.id == -1)
					{
						mf.id = t.fingerId;
						mf.touch = t;
						break;
					}
				}
			}
		}

		// 记录完手指信息后，就是响应相应和状态记录了
		for (int i = 0; i < MyFinger.Fingers.Count; i++)
		{
			MyFinger mf = MyFinger.Fingers[i];
			if (mf.id != -1)
			{
				if (mf.touch.phase == TouchPhase.Began)
				{
					marks[i].SetActive(true);
					marks[i].transform.position = GetWorldPos(mf.touch.position);

					particles[i].transform.position = GetWorldPos(mf.touch.position);
				}
				else if (mf.touch.phase == TouchPhase.Moved)
				{
					marks[i].transform.position = GetWorldPos(mf.touch.position);

					if (!particles[i].isPlaying)
					{
						particles[i].loop = true;
						particles[i].Play();
					}
					particles[i].transform.position = GetWorldPos(mf.touch.position);
				}
				else if (mf.touch.phase == TouchPhase.Ended)
				{
					marks[i].SetActive(false);
					marks[i].transform.position = GetWorldPos(mf.touch.position);

					particles[i].loop = false;
					particles[i].Play();
					particles[i].transform.position = GetWorldPos(mf.touch.position);
				}
				else if (mf.touch.phase == TouchPhase.Stationary)
				{
					if (particles[i].isPlaying)
					{
						particles[i].Pause();
					}
					particles[i].transform.position = GetWorldPos(mf.touch.position);
				}
			}
			else
			{
				;
			}
		}

		// exit
		if (Input.GetKeyDown(KeyCode.Home) || Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}

		//		// test
		//		if(Input.GetMouseButtonDown(0))
		//		{
		//			GameObject mark = Instantiate(markPerfab, Vector3.zero, Quaternion.identity) as GameObject;
		//			mark.transform.parent = this.transform;
		//			mark.transform.position = GetWorldPos(Input.mousePosition);
		//
		//			ParticleSystem particle = Instantiate(particlePerfab, Vector3.zero, Quaternion.identity) as ParticleSystem;
		//			particle.transform.parent = this.transform;
		//			particle.transform.position = GetWorldPos(Input.mousePosition);
		//			particle.loop = false;
		//			particle.Play();
		//		}
	}

	/// <summary>
	/// 显示相关高度数据
	/// </summary>
	void OnGUI()
	{
		GUILayout.Label("支持的手指的数量：" + MyFinger.Fingers.Count);
		GUILayout.BeginHorizontal(GUILayout.Width(Screen.width));
		for (int i = 0; i < MyFinger.Fingers.Count; i++)
		{
			GUILayout.BeginVertical();
			MyFinger mf = MyFinger.Fingers[i];
			GUILayout.Label("手指" + i.ToString());
			if (mf.id != -1)
			{
				GUILayout.Label("Id： " + mf.id);
				GUILayout.Label("状态： " + mf.touch.phase.ToString());
			}
			else
			{
				GUILayout.Label("没有发现！");
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndHorizontal();
	}

	public Vector3 GetWorldPos(Vector2 screenPos)
	{
		return Camera.main.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, Camera.main.nearClipPlane + 10));
	}
}
