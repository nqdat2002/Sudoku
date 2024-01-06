using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI m_Num, m_NumPencil;
    [SerializeField] GameObject m_Node;

    private Color _needFillColor = new(0.7169812f, 0.7169812f, 0.7169812f, 1f);
    private Color _baseColor = Color.white;
    private Color _hoverColor = Color.green;
    private Color _borderColor = Color.cyan;
    private Color _errorColor = Color.red;

    private DataNode _dataNode;
    public DataNode GetDataNode {  get { return _dataNode; }}

    public event System.Action<Node> OnNodeClick;
    public void OnClick()
    {
        OnNodeClick?.Invoke(this);
    }
    public void SetData(DataNode dataNode)
    {
        _dataNode = dataNode;
        if (_dataNode.Num > 0)
        {
            m_Num.text = _dataNode.Num.ToString();
        }
        else
        {
            m_Num.text = "";
        }
        m_NumPencil.text = "";
        //Debug.Log(_dataNode.X + " " + _dataNode.Y);
    }

    public void SetColor(string type = "default")
    {
        switch(type)
        {
            case "default":
                m_Node.GetComponent<Image>().color = _baseColor;
                break;
            case "hover":
                m_Node.GetComponent<Image>().color = _hoverColor;
                break;
            case "borderhover":
                m_Node.GetComponent<Image>().color = _borderColor;
                break;
            case "error":
                m_Node.GetComponent<Image>().color = _errorColor;
                break;
        }
    }

    public void SetNumber(int value)
    {
        m_Num.text = value.ToString();
        _dataNode.Num = value;
    }
    public void SetNumber(string value)
    {
        m_Num.text = value;
        _dataNode.Num = 0;
    }
    public void SetSmallNumber(int value)
    {
        m_NumPencil.text = value.ToString();
    }
    public void SetSmallNumber(string value)
    {
        m_NumPencil.text = value;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
