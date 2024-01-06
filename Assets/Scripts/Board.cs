using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_TextMode, m_TextCountDown;
    [SerializeField] GameObject m_PencilOn, m_PencilOff;
    [SerializeField] GameObject m_PopupSelectMode;
    [SerializeField] GameObject m_PauseActive, m_ResumeActive;
    [SerializeField] private TextMeshProUGUI[] listNumTexts;

    private const int EASY = 0, MEDIUM = 1, HARD = 2, EXPERT = 3;
    private int _currentMode = 0;
    // Time Countdown
    private bool _isPause = false;
    private float timecountdown = 0;
    // Pencil
    private bool _isPencilActive = true;
    private bool _endGame = false;

    private Node _currentHovered = null;
    private List<Node> _currentRow = null, _currentColumn = null, _currentGroup = null;

    public List<Node> m_ListNode = new();
    
    public void ParseData(int modeGame)
    {
        OnClickTurnOffPopUp();
        OnClickPencil();
        int lvl = Random.Range(0, 200);
        string level = "Level_" + lvl;
        string mode = "";
        switch (modeGame)
        {
            case EASY:
                {
                    mode = "easy/";
                    break;
                }
            case MEDIUM:
                {
                    mode = "medium/";
                    break;
                }
            case HARD:
                {
                    mode = "hard/";
                    break;
                }
            case EXPERT:
                {
                    mode = "expert/";
                    break;
                }
        }

        string path = "DB/" + mode + level;
        string dataParse = Resources.Load<TextAsset>(path).ToString();
        JSONNode jN = JSON.Parse(dataParse);
        JSONArray jA = jN["blocks"].AsArray;

        List<DataNode> listData = new();

        for (int i = 0; i < jA.Count; ++i)
        {
            JSONNode jB = jA[i].AsObject;
            int _x = -1, _y;

            if (i % 9 <= 2)
            {
                _x = 0;
            }
            else if (i % 9 < 6)
            {
                _x = 1;
            }
            else
            {
                _x = 2;
            }

            if (i / 9 <= 2)
            {
                _y = 0;
            }
            else if (i / 9 < 6)
            {
                _y = 1;
            }
            else
            {
                _y = 2;
            }


            DataNode dN = new(
                i,
                _y * 3 + _x,
                jB["num"].AsInt,
                jB["canEdit"].AsBool,
                jB["addNum"].AsInt,
                new bool[9], 
                i / 9,
                i % 9
            ); 
            listData.Add(dN);
        }
        for (int i = 0; i < listData.Count; ++i)
        {
            m_ListNode[i].SetData(listData[i]);
            m_ListNode[i].OnNodeClick += HandleNodeClick;
            //Debug.Log("Row: "+ listData[i].X + " Column: " + listData[i].Y + " BoxID: " + listData[i].IdBox);
        }
        m_TextMode.text = mode[..^1].ToUpper();
        _currentMode = modeGame;


        for (int i = 0; i < listNumTexts.Length; i++)
        {
            int index = i + 1;
            listNumTexts[i].GetComponent<Button>().onClick.AddListener(() => HandleBtnNumClick(index));
        }
    }
    public void OnClickTurnOnPopUp()
    {
        m_PopupSelectMode.SetActive(true);
    }
    public void OnClickTurnOffPopUp()
    {
        m_PopupSelectMode.SetActive(false);
    }
    public void RestartCurrentMode()
    {
        ParseData(_currentMode);
    }
    public void TimeCountDown()
    {
        if (!_isPause)
        {
            timecountdown += Time.deltaTime;
            m_TextCountDown.text = string.Format("{0:00}:{1:00}:{2:00}", Mathf.FloorToInt(timecountdown / 3600), Mathf.FloorToInt(timecountdown / 60 % 60), timecountdown % 60);
        }
    }
    public void OnClickPause()
    {
        m_PauseActive.SetActive(_isPause);
        m_ResumeActive.SetActive(!_isPause);
        _isPause = !_isPause;
    }
    public void OnClickPencil()
    {
        m_PencilOn.SetActive(!_isPencilActive);
        m_PencilOff.SetActive(_isPencilActive);
        _isPencilActive = !_isPencilActive;
    }
    public void OnClickErase()
    {
        if (_currentHovered != null)
        {
            if (!_isPause)
            {
                if (!_isPencilActive)
                {
                    _currentHovered.SetNumber("");
                }
                else
                {
                    _currentHovered.SetSmallNumber("");
                }
            }
        }
    }

    private void HandleNodeClick(Node clickedNode)
    {
        if (clickedNode.GetDataNode.CanEdit && !_isPause) {
            if (_currentHovered != null)
            {
                SetColorList(_currentRow, true);
                SetColorList(_currentColumn, true);
                SetColorList(_currentGroup, true);
                _currentHovered.SetColor();
            }

            _currentHovered = clickedNode;
            int row = clickedNode.GetDataNode.X;
            int column = clickedNode.GetDataNode.Y;
            int group = clickedNode.GetDataNode.IdBox;
            _currentRow = GetAllRow(row);
            _currentColumn = GetAllColumn(column);
            _currentGroup = GetAllGroup(group);
            SetColorList(_currentRow);
            SetColorList(_currentColumn);
            SetColorList(_currentGroup);
            clickedNode.SetColor("hover");
        }
    }
    private void HandleBtnNumClick(int index)
    {
        if (!_isPause && _currentHovered != null)
        {
            if (!_isPencilActive)
            {
                //string value = listNumTexts[index].text;
                int row = _currentHovered.GetDataNode.X;
                int column = _currentHovered.GetDataNode.Y;
                int group = _currentHovered.GetDataNode.IdBox;
                if (IsFillInPosition(index, row, column, group))
                {
                    _currentHovered.SetNumber(index);
                }
                //_currentHovered.SetNumber(index);
            }
            else
            {
                //string value=  listNumTexts[index].text;
                _currentHovered.SetSmallNumber(index);
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        ParseData(EASY);
    }

    // Update is called once per frame
    void Update()
    {
        TimeCountDown();
    }

    private void DoCheck()
    {
        while (!_endGame)
        {
            if (_currentHovered != null)
            {
                for (int i = 0; i < m_ListNode.Count; i++)
                {
                    if (m_ListNode[i].GetDataNode.CanEdit && m_ListNode[i].GetDataNode.Num > 0)
                    {
                        m_ListNode[i].SetColor("error");
                    } 
                }
                //int num = _currentHovered.GetDataNode.Num;
                //foreach (var node in _currentColumn)
                //{
                //    if (node.GetDataNode.Num == num && num > 0)
                //    {
                //        node.SetColor("error");
                //    }
                //}

                //foreach (var node in _currentRow)
                //{
                //    if (node.GetDataNode.Num == num && num > 0)
                //    {
                //        node.SetColor("error");
                //    }
                //}

                //foreach (var node in _currentGroup)
                //{
                //    if (node.GetDataNode.Num == num && num > 0)
                //    {
                //        node.SetColor("error");
                //    }
                //}
            }

            if (HasEmptyToFill())
            {
                _endGame = true;
            }
        }
    }

    private List<Node> GetAllRow(int row)
    {
        List<Node> nodes = new();
        for (int i = 0; i < m_ListNode.Count; ++i) { 
            if (m_ListNode[i].GetDataNode.X == row)
            {
                nodes.Add(m_ListNode[i]);
            }
        }
        return nodes;
    }
    private List<Node> GetAllColumn(int column)
    {
        List<Node> nodes = new();
        for (int i = 0; i < m_ListNode.Count; ++i)
        {
            if (m_ListNode[i].GetDataNode.Y == column)
            {
                nodes.Add(m_ListNode[i]);
            }
        }
        return nodes;
    }
    private List<Node> GetAllGroup(int group)
    {
        List<Node> nodes = new();
        for (int i = 0; i < m_ListNode.Count; ++i)
        {
            if (m_ListNode[i].GetDataNode.IdBox == group)
            {
                nodes.Add(m_ListNode[i]);
            }
        }
        return nodes;
    }

    private void SetColorList(List<Node> nodes, bool mode = false)
    {
        if (!mode)
        {
            foreach (var node in nodes)
            {
                node.SetColor("borderhover");
            }
            return;
        }

        foreach (var node in nodes)
        {
            node.SetColor();
            //Debug.Log(node.GetDataNode.IdBox);
        }
    }

    private bool HasEmptyToFill()
    {
        for (int i = 0; i < m_ListNode.Count; ++i)
        {
            if (m_ListNode[i].GetDataNode.Num == 0)
            {
                return true;
            }
        }
        return false;
    }

    private bool IsFillInPosition(int num, int row, int column, int group)
    {
        return IsFillInColumn(num, column) && IsFillInRow(num, row) && IsFillInGroup(num, group);
    }

    private bool IsFillInRow(int num, int row)
    {
        List<Node> nodes = GetAllRow(row);
        foreach(var node in nodes)
        {
            if(node.GetDataNode.Num == num)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsFillInColumn(int num, int column)
    {
        List<Node> nodes = GetAllColumn(column);
        foreach (var node in nodes)
        {
            if (node.GetDataNode.Num == num)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsFillInGroup(int num, int group)
    {
        List<Node> nodes = GetAllGroup(group);
        foreach (var node in nodes)
        {
            if (node.GetDataNode.Num == num)
            {
                return false;
            }
        }
        return true;
    }
}
