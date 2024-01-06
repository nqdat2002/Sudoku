using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataNode 
{
    private int id;
    private int idBox;
    private int num;
    private bool canEdit;
    private int addNum;
    private bool [] hintNum;
    private int x, y;

    public DataNode(int id, int idBox, int num, bool canEdit, int addNum, bool [] hintNum, int x, int y)
    {
        this.id = id;
        this.idBox = idBox;
        this.num = num;
        this.canEdit = canEdit;
        this.addNum = addNum;
        this.hintNum = hintNum;
        this.x = x;
        this.y = y;
    }

    public int Id {  get { return id; } set { id = value; } }
    public int IdBox {  get { return idBox; } set { idBox = value; } }
    public int Num { get { return num; } set {  num = value; } }
    public bool CanEdit { get { return canEdit; } set { canEdit = value; } }
    public int AddNum { get {  return addNum; } set {  addNum = value; } }
    public bool [] HintNum { get { return hintNum; } set { hintNum = value; } }
    public int X {  get { return x; } set { x = value; } }
    public int Y { get { return y; } set {  y = value; } }
}
